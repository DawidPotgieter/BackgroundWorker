using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces.Internal;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Logic.DataModel.Internal.Jobs;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace BackgroundWorkerService.Logic.Implementation.Internal
{
	/// <summary>
	/// An implementation of <see cref="IExecutionQueue"/> that uses a basically unlimited amount of threads to execute jobs on.
	/// This queue supports thread aborting on shutdown, as well as specific timeouts per job.
	/// Each executing job uses two threads, but one of the threads does almost no work.
	/// </summary>
	internal class TimedThreadExecutionQueue : IExecutionQueue
	{
		private LinkedList<JobExecutionContext> workers = new LinkedList<JobExecutionContext>();

		public TimedThreadExecutionQueue()
		{
		}

		public event EventHandler<JobFinishedExecutingEventArgs> JobFinishedExecuting;

		public byte Id { get; set; }

		public uint ActiveThreads
		{
			get 
			{
				return (uint)workers.Count;
			}
		}

		private uint threadCount = 20;
		public uint ThreadCount
		{
			get
			{
				return threadCount;
			}
			set
			{
				threadCount = value;
			}
		}

		public bool IsStopping { get; set; }

		public bool Enqueue(JobContext jobContext)
		{
			lock(this)
			{
				if (IsStopping || (ActiveThreads >= ThreadCount))
				{
					return false;
				}

				JobExecutionContext jobExecutionContext = new JobExecutionContext(jobContext, this);
				Thread thread = new Thread(StartMonitoredJob);
				jobExecutionContext.Thread = thread;
				thread.IsBackground = true;
				workers.AddLast(jobExecutionContext);
				thread.Start(jobExecutionContext);
				return true;
			}
		}

		static void StartMonitoredJob(Object jobExecutionContextObject)
		{
			JobExecutionContext jobExecutionContext = (JobExecutionContext)jobExecutionContextObject;
			JobContext jobContext = jobExecutionContext.JobContext;

			Thread thread = new Thread(ExecuteJob);

			thread.Start(jobExecutionContext);

			jobExecutionContext.Thread = thread; //replace the ref to the thread. This is the actual one we want to be able to stop.

			if (jobContext.JobData.AbsoluteTimeout.HasValue)
			{
				bool completedWithoutTimeout = thread.Join(jobContext.JobData.AbsoluteTimeout.Value);
				if (!completedWithoutTimeout)
				{
					string message = string.Format("Job has exceeded it's AbsoluteTimeout value of {0} second(s) and was terminated abnormally.", jobContext.JobData.AbsoluteTimeout.Value.TotalSeconds);
					jobContext.JobManager.JobStore.SetJobStatuses(new long[] { jobContext.JobData.Id }, JobStatus.Executing, JobStatus.ExecutionTimeout, message);
					jobExecutionContext.Thread.Abort();
					while (jobExecutionContext.Thread.IsAlive)
					{
						Thread.Sleep(1);
					}
					TimedThreadExecutionQueue queue = (TimedThreadExecutionQueue)jobExecutionContext.ExecutionQueue;
					lock (queue.workers)
					{
						queue.workers.Remove(jobExecutionContext);
					}
					var jobFinishedEvent = queue.JobFinishedExecuting;
					if (jobFinishedEvent != null)
					{
						jobFinishedEvent(queue, new JobFinishedExecutingEventArgs(jobContext.JobData.Id));
					}
				}
			}
			else
			{
				thread.Join();
			}
		}

		static void ExecuteJob(Object jobExecutionContextObject)
		{
			JobExecutionContext jobExecutionContext = (JobExecutionContext)jobExecutionContextObject;
			JobContext jobContext = jobExecutionContext.JobContext;

			Debug.WriteLine(DateTime.Now + " : " + jobContext.JobData.Id + " start thread. Queue = " + jobExecutionContext.ExecutionQueue.Id);

			try
			{
				IJobExecutorFactory jobExecutorFactory = new JobExecutorFactory();
				IJobExecutor jobExecutor = jobExecutorFactory.GetJobExecutor(jobContext);
				jobExecutor.ExecuteJob(jobContext);
				TimedThreadExecutionQueue queue = (TimedThreadExecutionQueue)jobExecutionContext.ExecutionQueue;
				lock (queue.workers)
				{
					queue.workers.Remove(jobExecutionContext);
				}
				var jobFinishedEvent = queue.JobFinishedExecuting;
				if (jobFinishedEvent != null)
				{
					jobFinishedEvent(queue, new JobFinishedExecutingEventArgs(jobContext.JobData.Id));
				}
			}
			catch (ThreadAbortException)
			{
				string message = string.Format("Job has exceeded the ShutdownTimeout and was terminated abnormally.", jobContext.JobData.Id);
				jobContext.JobManager.JobStore.SetJobStatuses(new long[] { jobContext.JobData.Id }, JobStatus.Executing, JobStatus.ShutdownTimeout, message);
			}

			Debug.WriteLine(DateTime.Now + " : " + jobContext.JobData.Id + " end thread. Queue = " + jobExecutionContext.ExecutionQueue.Id);
		}

		public bool ShutdownRunningJobs()
		{
			lock (workers)
			{
				foreach (var worker in workers)
				{
					worker.Thread.Abort();
					while (worker.Thread.IsAlive)
					{
						Thread.Sleep(1);
					}
				}
				workers.Clear();
			}
			return true;
		}
	}
}
