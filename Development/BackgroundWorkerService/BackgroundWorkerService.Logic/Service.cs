using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.Implementation;
using BackgroundWorkerService.Logic.Interfaces.Internal;
using BackgroundWorkerService.Logic.DataModel.Internal.Jobs;
using BackgroundWorkerService.Logic.DataModel;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Logic.DataModel.Internal.Service;
using BackgroundWorkerService.Service;
using System.ComponentModel;
using BackgroundWorkerService.Logic.Helpers;
using BackgroundWorkerService.Logic.DataModel.Scheduling;
using Common.Logging;
using System.Reflection;
using System.IO;

namespace BackgroundWorkerService.Logic
{
	/// <summary>
	/// This is the main brains of the background worker service.  This "service" controls all the queuing, execution an scheduling of jobs.
	/// If you are thinking of using this in a web application, you need to store this instance in the appdomain.
	/// </summary>
	public class Service
	{
		private IJobManager jobManager;
		private Dictionary<int, IExecutionQueue> jobQueues;
		private Timer pollTimer;
		private ServiceStatus serviceStatus = ServiceStatus.Stopped;

		/// <summary>
		/// Initializes a new instance of the <see cref="Service"/> class.
		/// </summary>
		public Service() 
			: this(new JobManager())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Service"/> class.
		/// </summary>
		/// <param name="jobManager">The job manager.</param>
		public Service(IJobManager jobManager)
		{
			try
			{
				Initialize(jobManager);
			}
			catch (Exception ex)
			{
				try
				{
					//This is really a last ditch effort to provide some information when the service doesn't want to start - usually when the config is buggered.  Can't use the default
					//Logging here which is usually specified in config :(
					File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ServiceStartupErrors.txt"), "Fatal error starting up : " + ex.ToString());
				}
				catch { }
				throw;
			}
		}

		private void Initialize(IJobManager jobManager)
		{
			this.jobManager = jobManager;

			jobQueues = new Dictionary<int, IExecutionQueue>();

			//The poll timer is used to trigger scheduled jobs and ready jobs
			pollTimer = new Timer();
			pollTimer.Elapsed += new ElapsedEventHandler(pollTimer_Elapsed);
		}

		/// <summary>
		/// Occurs when an event notification occurs.  Used by the ui's to see the last status message.
		/// </summary>
		public event EventHandler<NotificationEventArgs> Notify;

		/// <summary>
		/// Starts this instance and initializes all the timers and queues.
		/// </summary>
		/// <returns></returns>
		public bool Start()
		{
			if (serviceStatus == ServiceStatus.Stopped)
			{
				CheckAbnormallyTerminatedJobs();
				CalculateJobsNextExecutionTimes();
				foreach (QueueDefinition queueDefinition in SettingsProvider.Queues)
				{
					IExecutionQueue queue = Utils.CreateInstanceWithRequiredInterface<IExecutionQueue>(queueDefinition.Type, typeof(IExecutionQueue).AssemblyQualifiedName);
					if (queue != null)
					{
						jobQueues.Add(queueDefinition.Id, queue);
						jobQueues[queueDefinition.Id].Id = queueDefinition.Id;
						jobQueues[queueDefinition.Id].ThreadCount = queueDefinition.ThreadCount;
						jobQueues[queueDefinition.Id].JobFinishedExecuting += new EventHandler<JobFinishedExecutingEventArgs>(Queue_JobFinishedExecuting);
					}
				}

				//To change the accuracy of scheduled jobs, this interval can be changed.
				pollTimer.Interval = SettingsProvider.PollFrequency.TotalMilliseconds;
				pollTimer.Start();

				serviceStatus = ServiceStatus.Running;
				return true;
			}
			else if (serviceStatus == ServiceStatus.Paused)
			{
				return Resume();
			}
			return false;
		}

		/// <summary>
		/// Pauses this instance.  This is different from Stopping, as queued jobs stay in memory, they just won't be executed.
		/// </summary>
		/// <returns></returns>
		public bool Pause()
		{
			serviceStatus = ServiceStatus.Paused;
			pollTimer.Stop();
			return true;
		}

		/// <summary>
		/// Resumes this instance if it was paused.
		/// </summary>
		/// <returns></returns>
		public bool Resume()
		{
			if (serviceStatus == ServiceStatus.Paused)
			{
				pollTimer.Start();
				serviceStatus = ServiceStatus.Running;
			}
			return true;
		}

		/// <summary>
		/// Stops the service and does a clean shutdown (if possible) of running jobs.
		/// </summary>
		/// <returns></returns>
		public bool Stop(bool waitforRunningJobs)
		{
			if (serviceStatus != ServiceStatus.Stopped)
			{
				serviceStatus = DataModel.Internal.Service.ServiceStatus.Stopping;
				pollTimer.Stop();

				foreach (var queue in jobQueues.Values)
				{
					queue.IsStopping = true;
				}

				if (waitforRunningJobs)
				{
					WaitForAllRunningJobsToComplete();
				}

				ShutdownAllRunningJobs(waitforRunningJobs);

				DequeueJobs();

				var queueIds = jobQueues.Select(jq => jq.Key).ToList();
				foreach (int queueId in queueIds)
				{
					jobQueues[queueId].JobFinishedExecuting -= Queue_JobFinishedExecuting;
					jobQueues[queueId] = null;
				}

				jobQueues.Clear();

				serviceStatus = ServiceStatus.Stopped;

				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks whether there is at least one running job in any of the queues
		/// </summary>
		/// <returns></returns>
		public bool HasRunningJob
		{
			get
			{
				bool hasRunningJob = false;
				foreach (var jobQueue in jobQueues.Values)
				{
					if (jobQueue.ActiveThreads > 0)
					{
						hasRunningJob = true;
						break;
					}
				}
				return hasRunningJob;
			}
		}

		/// <summary>
		/// Gets the number of jobs currently running on all queues.
		/// </summary>
		public uint RunningJobsCount
		{
			get
			{
				uint count = 0;
				foreach (var jobQueue in jobQueues.Values)
				{
					count += jobQueue.ActiveThreads;
				}
				return count;
			}
		}

		private static ISettingsProvider settingsProvider;
		/// <summary>
		/// Gets the settings provider.
		/// </summary>
		/// <value>The settings provider.</value>
		internal static ISettingsProvider SettingsProvider
		{
			get
			{
				if (settingsProvider == null)
				{
					settingsProvider = BackgroundWorkerService.Logic.Configuration.ConfigurationSettings.SettingsProvider;
				}
				return settingsProvider;
			}
		}

		private static ILog logger;
		/// <summary>
		/// Gets the logging provider.
		/// </summary>
		/// <value>The logging provider.</value>
		internal static ILog Logger
		{
			get
			{
				if (logger == null)
				{
					logger = LogManager.GetCurrentClassLogger();
				}
				return logger;
			}
		}

		/// <summary>
		/// Gets the job manager for the service.
		/// </summary>
		public IJobManager JobManager
		{
			get
			{
				return jobManager;
			}
		}

		/// <summary>
		/// Gets the public job queue information.
		/// </summary>
		public IList<IExecutionQueueBase> JobQueues
		{
			get
			{
				return jobQueues.Values.Select(q => (IExecutionQueueBase)q).ToList().AsReadOnly();
			}
		}

		internal ServiceStatus ServiceStatus
		{
			get { return serviceStatus; }
		}

		void pollTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			pollTimer.Stop();
			try
			{
				//The logic when the schedule timer fires : 
				//Get all scheduled jobs that have a nextfiretime less than now
				//If the job can be scheduled to run now, add it to a list.
				//At the end, if there are jobs to schedule, do a bulk update of the list to get them all in ready state.

				DateTime now = DateTime.Now;
				var jobsScheduledToExecute = jobManager.JobStore.GetScheduleReadyJobs(now);
				var jobIdsToExecute = new List<long>();
				if (jobsScheduledToExecute != null && jobsScheduledToExecute.Count > 0)
				{
					foreach (var job in jobsScheduledToExecute)
					{
						if (job.Schedule != null && job.Schedule.CanOccurAt(now))
						{
							jobIdsToExecute.Add(job.Id);
						}
					}
					jobManager.JobStore.SetJobStatuses(jobIdsToExecute.ToArray(), JobStatus.Scheduled, JobStatus.Ready, null, null);
				}
			}
			catch(Exception ex)
			{
				logger.Info("PollTimer Elapsed threw an exception.", ex);
			}
			finally
			{
				if (serviceStatus == ServiceStatus.Running)
				{
					CheckForJobEnqueue();
					pollTimer.Start();
				}
			}
		}

		void Queue_JobFinishedExecuting(object sender, JobFinishedExecutingEventArgs e)
		{
			TriggerNotify(string.Format("Job {0} execution complete.", e.JobId));
		}

		/// <summary>
		/// This method runs through all the queues and checks for open space.  If there is open space and there are ready jobs for that queue, it gets enqueued.
		/// </summary>
		private void CheckForJobEnqueue()
		{
			if (serviceStatus == ServiceStatus.Running)
			{
				foreach (var queue in jobQueues)
				{
					if (queue.Value.ActiveThreads < queue.Value.ThreadCount)
					{
						var readyJobs = jobManager.JobStore.QueueReadyJobs(queue.Value.Id, queue.Value.ThreadCount - queue.Value.ActiveThreads);
						foreach (var readyJob in readyJobs)
						{
							if (EnqueueJobForExecution(readyJob))
							{
								TriggerNotify(string.Format("Job {0} of type '{1}' has been queued for execution.", readyJob.Id, readyJob.JobType.AssemblyQualifiedName));
							}
							else
							{
								//Queue Full
								jobManager.JobStore.DequeueJob(readyJob);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Creates an instance of the IJob and queues it on it's specified queue for execution.
		/// </summary>
		/// <param name="jobData">The job data.</param>
		/// <returns></returns>
		private bool EnqueueJobForExecution(JobData jobData)
		{
			if (serviceStatus == ServiceStatus.Running)
			{
				lock (jobData)
				{
					if (jobData.Status == JobStatus.Ready || jobData.Status == JobStatus.Queuing || jobData.Status == JobStatus.Queued)
					{
						IJob jobInstance = Utils.CreateInstanceWithRequiredInterface<IJob>(jobData.JobType, typeof(IJob));
						if (jobInstance != null)
						{
							if (jobQueues.ContainsKey(jobData.QueueId))
							{
								return jobQueues[jobData.QueueId].Enqueue(new JobContext(jobManager, jobData, jobInstance));
							}
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Shuts down all running jobs if that is possible.  If not, it will wait indefinitely.
		/// </summary>
		private void ShutdownAllRunningJobs(bool waitforJobs)
		{
			foreach (var queue in jobQueues.Values)
			{
				if (!queue.ShutdownRunningJobs() && !waitforJobs)
				{
					int seconds = 0;
					TriggerNotify(string.Format("Queue {0} of type {1} could not shut down running jobs.  In order to avoid catastrophic failure, the system will wait until this queue is empty.", queue.Id, queue.GetType().AssemblyQualifiedName));
					while (queue.ActiveThreads > 0)
					{
						System.Threading.Thread.Sleep(1000);
						seconds++;
						TriggerNotify(string.Format("Queue {0} of type {1} could not shut down running jobs.  In order to avoid catastrophic failure, the system will wait until this queue is empty. Waited : {2} seconds.", queue.Id, queue.GetType().AssemblyQualifiedName, seconds));
					}
				}
			}
		}

		/// <summary>
		/// Waits for all running jobs to complete up to the shutdown timeout limit (if specified).
		/// </summary>
		private void WaitForAllRunningJobsToComplete()
		{
			string message = "Service with id '" + SettingsProvider.InstanceName + "' is waiting for running jobs to complete....";
			Logger.Info(message);
			TriggerNotify(message);
			DateTime startTime = DateTime.Now;
			DateTime? timeoutTime = null;
			if (SettingsProvider.ShutdownTimeout.HasValue)
				timeoutTime = startTime.Add((TimeSpan)SettingsProvider.ShutdownTimeout);

			while (HasRunningJob)
			{
				if ((timeoutTime.HasValue) && (timeoutTime < DateTime.Now))
				{
					break;
				}
				else
				{
					message = "Wait for running jobs to complete... (" + (jobQueues.Values.Sum(q => q.ActiveThreads)) + "). Timeout : " + (timeoutTime.HasValue ? ((int)GetTimeoutTime((DateTime)timeoutTime).TotalSeconds).ToString() + " seconds" : "Never");
					TriggerNotify(message);
					System.Threading.Thread.Sleep(1000);
				}
			}
		}

		/// <summary>
		/// Dequeues all jobs that are in <see cref="JobStatus.Queuing"/> or <see cref="JobStatus.Queued"/> state.
		/// </summary>
		private void DequeueJobs()
		{
			//In case there are any queued jobs for this worker instance, try to dequeue those.
			var queuedJobs = jobManager.JobStore.GetJobs(0, int.MaxValue, null, null, new JobStatus[] { JobStatus.Queuing, JobStatus.Queued }).Where(j => j.Instance == null || j.Instance == SettingsProvider.InstanceName);
			foreach (var queuedJob in queuedJobs)
			{
				jobManager.JobStore.DequeueJob(queuedJob);
			}
			//Now try to set all ready jobs that should be scheduled back to scheduled status
			var scheduledReadyJobs = jobManager.JobStore.GetJobs(0, int.MaxValue, null, null, new JobStatus[] { JobStatus.Ready }).Where(j => (j.Instance == null || j.Instance == SettingsProvider.InstanceName) && j.Schedule != null);
			jobManager.JobStore.SetJobStatuses(scheduledReadyJobs.Select(j => j.Id).ToArray(), JobStatus.Ready, JobStatus.Scheduled, null, null); //Set instance to null here
		}

		/// <summary>
		/// When the service stops unexpectedly - like a power cut, certain jobs will be in the incorrect states.  Some can be updated and automatically corrected.
		/// </summary>
		private void CheckAbnormallyTerminatedJobs()
		{
			//Try to set all ready jobs that should be scheduled back to scheduled status
			var scheduledReadyOrTerminatedJobs = jobManager.JobStore.GetJobs(0, int.MaxValue, null, null, new JobStatus[] { JobStatus.Ready, JobStatus.ShutdownTimeout }).Where(j => (j.Instance == null || j.Instance == SettingsProvider.InstanceName) && j.Schedule != null);
			jobManager.JobStore.SetJobStatuses(scheduledReadyOrTerminatedJobs.Select(j => j.Id).ToArray(), JobStatus.Ready, JobStatus.Scheduled, null, null); //Set instance to null here
			jobManager.JobStore.SetJobStatuses(scheduledReadyOrTerminatedJobs.Select(j => j.Id).ToArray(), JobStatus.ShutdownTimeout, JobStatus.Scheduled, null, null); //Set instance to null here
			var jobs = jobManager.JobStore.GetJobs(0, int.MaxValue, null, null, new JobStatus[] { JobStatus.Queuing, JobStatus.Queued, JobStatus.Executing }).Where(j => j.Instance == null || j.Instance == SettingsProvider.InstanceName);
			//Jobs that had been picked up by this instance that are queueing or queued, can be safely set back to ready.
			foreach (var queuedJob in jobs.Where(j => j.Status == JobStatus.Queued || j.Status == JobStatus.Queuing))
			{
				jobManager.JobStore.DequeueJob(queuedJob);
			}
			//Jobs that had been executing is more tricky.  What we'll do is non scheduled jobs that got killed will be set to shutdowntimeout, while scheduled jobs will be set to scheduled
			foreach (var executingJob in jobs.Where(j => j.Status == JobStatus.Executing))
			{
				if (executingJob.Schedule != null)
				{
					jobManager.JobStore.SetJobStatuses(new long[] { executingJob.Id }, JobStatus.Executing, JobStatus.Scheduled, "The previous execution of this job was abnormally terminated.", null);  //Ensure we set the instance to null here
				}
				else
				{
					jobManager.JobStore.SetJobStatuses(new long[] { executingJob.Id }, JobStatus.Executing, JobStatus.ShutdownTimeout, "The previous execution of this job was abnormally terminated.");
				}
			}
		}

		/// <summary>
		/// Calculates the next execution times for scheduled jobs.  This can only be called before the service has started executing jobs (i.e. at service startup).
		/// </summary>
		private void CalculateJobsNextExecutionTimes()
		{
			var jobs = jobManager.JobStore.GetJobs(0, int.MaxValue).Where(j => j.Status == JobStatus.Scheduled || j.Schedule != null);
			var now = DateTime.Now;
			foreach (var job in jobs)
			{
				//The calculation here is different than when the service is running.  
				//The idea is that when the service starts up, we don't want jobs with schedules that didn't happen to fire.
				//Instead, only calculate fire times that is in the future.
				DateTime? nextFireTime = job.Schedule.GetNextOccurrence(now);
				if (nextFireTime != job.NextStartTime && (nextFireTime.HasValue ? job.Schedule.CanOccurAt(nextFireTime.Value) : true))
				{
					job.NextStartTime = nextFireTime;
					jobManager.JobStore.UpdateJob(job);
				}
			}
		}

		/// <summary>
		/// Gets the timeout time.
		/// </summary>
		/// <param name="timeout">The timeout.</param>
		/// <returns></returns>
		private TimeSpan GetTimeoutTime(DateTime timeout)
		{
			return (timeout.Subtract(DateTime.Now));
		}

		/// <summary>
		/// Triggers the notifification event to notify listeners about interesting events.
		/// </summary>
		/// <param name="message">The notification message.</param>
		private void TriggerNotify(string message)
		{
			if (Notify != null)
			{
				Notify(this, new NotificationEventArgs(message));
			}
		}
	}
}
