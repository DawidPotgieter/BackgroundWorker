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

namespace BackgroundWorkerService.Logic
{
	/// <summary>
	/// This is the main brains of the background worker service.  This "service" controls all the queuing, execution an scheduling of jobs.
	/// If you are thinking of using this in a web application, you need to store this service in the appdomain.
	/// </summary>
	public class Service
	{
		private IJobManager jobManager;
		private Dictionary<int, IExecutionQueue> jobQueues;
		private Dictionary<long, JobData> scheduledJobs = new Dictionary<long, JobData>();
		private Timer jobQueueTimer;
		private Timer scheduleTimer;
		private Timer eventTimer;
		private bool checkForJobEnqueue;
		private object checkingForJobEnqueueMonitor = new object();

		private ServiceStatus serviceStatus = ServiceStatus.Stopped;

		/// <summary>
		/// Initializes a new instance of the <see cref="Service"/> class.
		/// </summary>
		public Service()
		{
			Initialize(new JobManager());
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Service"/> class.
		/// </summary>
		/// <param name="jobManager">The job manager.</param>
		public Service(IJobManager jobManager)
		{
			Initialize(jobManager);
		}

		private void Initialize(IJobManager jobManager)
		{
			checkForJobEnqueue = false;

			this.jobManager = jobManager;
			jobManager.JobActionRequired += new EventHandler<JobActionRequiredEventArgs>(jobManager_ActionRequired);
			jobManager.JobDeleted += new EventHandler<JobActionRequiredEventArgs>(jobManager_JobDeleted);

			jobQueues = new Dictionary<int, IExecutionQueue>();

			//This timer has a user configurable period (i.e. every 1 minute the jobstore will be polled for jobs)
			jobQueueTimer = new Timer();
			jobQueueTimer.Elapsed += new ElapsedEventHandler(jobQueueTimer_Elapsed);

			//The code for this timer is the same, but this timer handles events raised by the job store on a a seperate thread.
			eventTimer = new Timer();
			eventTimer.Elapsed += new ElapsedEventHandler(eventTimer_Elapsed);

			//The schedule timer is used to check scheduled jobs for possible enqueuing.
			scheduleTimer = new Timer();
			scheduleTimer.Elapsed += new ElapsedEventHandler(scheduleTimer_Elapsed);
		}

		/// <summary>
		/// Occurs when an event notification occurs. i.e. New Job Arrived, Job Finished etc.  Used by the ui's to see the last status message.
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
				foreach (QueueDefinition queueDefinition in SettingsProvider.Queues)
				{
					IExecutionQueue queue = (IExecutionQueue)Utils.CreateInstanceWithRequiredInterface(queueDefinition.Type, typeof(IExecutionQueue).Name);
					if (queue != null)
					{
						jobQueues.Add(queueDefinition.Id, queue);
						jobQueues[queueDefinition.Id].Id = queueDefinition.Id;
						jobQueues[queueDefinition.Id].ThreadCount = queueDefinition.ThreadCount;
						jobQueues[queueDefinition.Id].JobFinishedExecuting += new EventHandler<JobFinishedExecutingEventArgs>(Queue_JobFinishedExecuting);
					}
				}

				jobQueueTimer.Interval = SettingsProvider.PollFrequency.TotalMilliseconds;
				jobQueueTimer.Start();

				//To change the accuracy of scheduled jobs, this interval can be changed. However, be aware that each scheduled jobs' next occurence gets calculated on every tick.
				scheduleTimer.Interval = 1000;
				scheduleTimer.Start();

				checkForJobEnqueue = true;
				eventTimer.Interval = 100;
				eventTimer.Start();

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
			jobQueueTimer.Stop();
			scheduleTimer.Stop();
			eventTimer.Stop();
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
				jobQueueTimer.Start();
				scheduleTimer.Start();
				eventTimer.Start();
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
				jobQueueTimer.Stop();
				scheduleTimer.Stop();
				eventTimer.Stop();

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

				scheduledJobs.Clear();

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

		/// <summary>
		/// Sets the status of in and memory (queued) job to run now.
		/// </summary>
		/// <param name="jobId"></param>
		/// <remarks>
		///	This is a little hacky, but there really isn't any other performant way to force scheduled jobs to run with the way the service is implemented.
		/// </remarks>
		/// <returns></returns>
		public bool SetScheduledJobToRunNow(long jobId)
		{
			lock (scheduledJobs)
			{
				if (scheduledJobs.ContainsKey(jobId))
				{
					scheduledJobs[jobId].Status = JobStatus.ScheduleRunNow;
					return true;
				}
				return false;
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

		private static ILoggingProvider logger;
		/// <summary>
		/// Gets the logging provider.
		/// </summary>
		/// <value>The logging provider.</value>
		internal static ILoggingProvider Logger
		{
			get
			{
				if (logger == null)
				{
					logger = BackgroundWorkerService.Logic.Configuration.ConfigurationSettings.LoggingProvider;
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

		void jobQueueTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			jobQueueTimer.Stop();
			try
			{
				CheckForJobEnqueue();
			}
			catch { }
			finally
			{
				if (serviceStatus == ServiceStatus.Running)
				{
					jobQueueTimer.Start();
				}
			}
		}

		void scheduleTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			scheduleTimer.Stop();
			try
			{
				if (scheduledJobs.Count > 0)
				{
					List<JobData> scheduledJobList = scheduledJobs.Select(j => j.Value).ToList();
					foreach (JobData scheduledJob in scheduledJobList)
					{
						DateTime lastFiredTime = scheduledJob.LastEndTime ?? scheduledJob.Schedule.StartDateTime;
						DateTime? nextFireTime = scheduledJob.Schedule.GetNextOccurrence(lastFiredTime);
						if ((
							(nextFireTime != null //It's quite possible that a schedule has no more occurrences.  Important to note that it will be kept around.
								&& nextFireTime.Value <= DateTime.Now //Anything in the past should be checked.  Since the timer only checks periodically, you have to make sure anything that is in the past has a chance to fire
								&& nextFireTime > lastFiredTime //Don't run if the next occurrence is before the last execution time.  Shouldn't happen but it's a safe check.
								&& scheduledJob.Schedule.CanOccurAt(DateTime.Now)) //It's possible that the next scheduled time is already in the past (if it hasn't executed before). The scheduled nextoccurrence is correct, but we have to ensure that it's allowed to run now
							|| (scheduledJob.Status == JobStatus.ScheduleRunNow))) //This status overrides any possible schdule times
						{
							lock (scheduledJobs)
							{
								if (EnqueueJobForExecution(scheduledJob))
								{
									TriggerNotify(string.Format("Job {0} of type '{1}' has been queued for execution by a schedule trigger.", scheduledJob.Id, scheduledJob.JobType.AssemblyQualifiedName));
									if (scheduledJobs.ContainsKey(scheduledJob.Id))
									{
										scheduledJobs.Remove(scheduledJob.Id);
									}
								}
								else
								{
									if (scheduledJob.Status != JobStatus.Queued && scheduledJob.Status != JobStatus.ScheduleRunNow)
									{
										//To lower processing, remove any jobs that won't run with the current status.  If the status changes later, it will be reloaded into memory.
										scheduledJobs.Remove(scheduledJob.Id);
									}
								}
							}
						}
					}
				}
			}
			catch { }
			finally
			{
				if (serviceStatus == ServiceStatus.Running)
				{
					scheduleTimer.Start();
				}
			}
		}

		void eventTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			eventTimer.Stop();
			try
			{
				if (checkForJobEnqueue)
				{
					CheckForJobEnqueue();
				}
			}
			catch { }
			finally
			{
				if (serviceStatus == ServiceStatus.Running)
				{
					eventTimer.Start();
				}
			}
		}

		void jobManager_ActionRequired(object sender, JobActionRequiredEventArgs e)
		{
			if (serviceStatus == ServiceStatus.Running)
			{
				TriggerNotify(string.Format("A new job with id {0} of type {1} has been created or updated.", e.Job.Id, e.Job.JobType.AssemblyQualifiedName));
			}
			//Since scheduled jobs are kept in memory for performance, any time a job is updated, the in memory list is checked.
			lock (scheduledJobs)
			{
				if (scheduledJobs.ContainsKey(e.Job.Id))
				{
					scheduledJobs[e.Job.Id] = e.Job;
				}
			}
			checkForJobEnqueue = true;
		}

		void jobManager_JobDeleted(object sender, JobActionRequiredEventArgs e)
		{
			//The job that was deleted by the jobstore could possibly be a scheduled job that is still in memory awaiting scheduling.
			//RemoveScheduledJob will remove it if it exists.
			RemoveScheduledJob(e.Job.Id);
			TriggerNotify(string.Format("Job with id {0} of type {1} has been deleted.", e.Job.Id, e.Job.JobType.AssemblyQualifiedName));
		}

		void Queue_JobFinishedExecuting(object sender, JobFinishedExecutingEventArgs e)
		{
			TriggerNotify(string.Format("Job {0} execution complete.", e.JobId));
			checkForJobEnqueue = true;
		}

		/// <summary>
		/// This method runs through all the queues and checks for open space.  If there is open space and there are ready jobs for that queue, it gets enqueued.
		/// </summary>
		private void CheckForJobEnqueue()
		{
			checkForJobEnqueue = false;

			lock (checkingForJobEnqueueMonitor)
			{
				if (serviceStatus == ServiceStatus.Running)
				{
					foreach (var queue in jobQueues)
					{
						if (queue.Value.ActiveThreads < queue.Value.ThreadCount)
						{
							var readyJob = jobManager.JobStore.QueueNextNonScheduledReadyJob(queue.Value.Id);
							while (readyJob != null && (queue.Value.ActiveThreads < queue.Value.ThreadCount))
							{
								if (readyJob.Schedule == null //Scheduled jobs are handled seperately.
									&& EnqueueJobForExecution(readyJob))
								{
									TriggerNotify(string.Format("Job {0} of type '{1}' has been queued for execution.", readyJob.Id, readyJob.JobType.AssemblyQualifiedName));
								}
								else
								{
									//Queue Full
									jobManager.JobStore.DequeueJob(readyJob);
								}
								readyJob = jobManager.JobStore.QueueNextNonScheduledReadyJob(queue.Value.Id);
							}
							if (readyJob != null)
							{
								//Queue Full
								jobManager.JobStore.DequeueJob(readyJob);
							}
						}
					}

					var readyScheduledJob = jobManager.JobStore.QueueNextScheduledReadyJob();
					while (readyScheduledJob != null)
					{
						lock (scheduledJobs)
						{
							if (jobManager.JobStore.SetJobStatus(readyScheduledJob.Id, readyScheduledJob.Status, JobStatus.Queued))
							{
								readyScheduledJob.Status = JobStatus.Queued;
								scheduledJobs.Add(readyScheduledJob.Id, readyScheduledJob);
							}
						}
						TriggerNotify(string.Format("Scheduled Job {0} of type '{1}'.", readyScheduledJob.Id, readyScheduledJob.JobType.AssemblyQualifiedName));
						readyScheduledJob = jobManager.JobStore.QueueNextScheduledReadyJob();
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
					if (jobData.Status == JobStatus.Ready || jobData.Status == JobStatus.Queuing || jobData.Status == JobStatus.Queued || jobData.Status == JobStatus.ScheduleRunNow)
					{
						IJob jobInstance = Utils.CreateInstanceWithRequiredInterface(jobData.JobType, typeof(IJob)) as IJob;
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
				else
				{
				}
			}
		}

		/// <summary>
		/// Waits for all running jobs to complete up to the shutdown timeout limit (if specified).
		/// </summary>
		private void WaitForAllRunningJobsToComplete()
		{
			string message = "Service with id '" + SettingsProvider.InstanceName + "' is waiting for running jobs to complete....";
			Logger.LogMessage(message);
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
			//First dequeue all scheduled in memory jobs.
			lock (scheduledJobs)
			{
				var scheduledJobsList = scheduledJobs.ToList();
				foreach (var queuedJob in scheduledJobsList)
				{
					jobManager.JobStore.DequeueJob(queuedJob.Value);
				}
				scheduledJobs.Clear();
			}
			//In case there are any other queued jobs for this worker instance, try to dequeue those.
			var queuedJobs = jobManager.JobStore.GetJobs(0, int.MaxValue, null, new JobStatus[] { JobStatus.Queuing, JobStatus.Queued }).Where(j => j.Instance == SettingsProvider.InstanceName);
			foreach (var queuedJob in queuedJobs)
			{
				jobManager.JobStore.DequeueJob(queuedJob);
			}
		}

		/// <summary>
		/// When the service stops unexpectedly - like a power cut, certain jobs will be in the incorrect states.  Some can be updated and automatically corrected.
		/// </summary>
		private void CheckAbnormallyTerminatedJobs()
		{
			var jobs = jobManager.JobStore.GetJobs(0, int.MaxValue, null, new JobStatus[] { JobStatus.Queuing, JobStatus.Queued, JobStatus.Executing }).Where(j => j.Instance == SettingsProvider.InstanceName);
			//Jobs that had been picked up by this instance that are queueing or queued, can be safely set back to ready.
			foreach (var queuedJob in jobs.Where(j => j.Status == JobStatus.Queued || j.Status == JobStatus.Queuing))
			{
				jobManager.JobStore.DequeueJob(queuedJob);
			}
			//Jobs that had been executing is more tricky.  What we'll do is non scheduled jobs that got killed will be set to shutdowntimeout, while scheduled jobs will be set to ready
			foreach (var executingJob in jobs.Where(j => j.Status == JobStatus.Executing))
			{
				if (executingJob.Schedule != null)
				{
					jobManager.JobStore.SetJobStatus(executingJob.Id, JobStatus.Executing, JobStatus.Ready, "The previous execution of this job was abnormally terminated.");
				}
				else
				{
					jobManager.JobStore.SetJobStatus(executingJob.Id, JobStatus.Executing, JobStatus.ShutdownTimeout, "The previous execution of this job was abnormally terminated.");
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

		private void TriggerNotify(string message)
		{
			if (Notify != null)
			{
				Notify(this, new NotificationEventArgs(message));
			}
		}

		/// <summary>
		/// Removes the specified job from the in-memory scheduled job list.
		/// </summary>
		/// <param name="jobId"></param>
		private void RemoveScheduledJob(long jobId)
		{
			lock (scheduledJobs)
			{
				if (scheduledJobs.ContainsKey(jobId))
				{
					scheduledJobs.Remove(jobId);
				}
			}
		}
	}
}
