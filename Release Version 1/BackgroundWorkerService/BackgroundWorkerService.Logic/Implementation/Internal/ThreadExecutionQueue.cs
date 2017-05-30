using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces.Internal;
using BackgroundWorkerService.Logic.DataModel.Internal.Jobs;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using System.Threading;
using System.Diagnostics;

namespace BackgroundWorkerService.Logic.Implementation.Internal
{
	/// <summary>
	/// An implementation of <see cref="IExecutionQueue"/> that uses a basically unlimited amount of threads to execute jobs on.
	/// This queue supports thread aborting on shutdown, but not specific timeouts per job.
	/// Each executing job uses one thread.
	/// </summary>
	internal class ThreadExecutionQueue : IExecutionQueue
	{
		private LinkedList<JobExecutionContext> workers = new LinkedList<JobExecutionContext>();

		public ThreadExecutionQueue()
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
			lock (this)
			{
				if (IsStopping || (ActiveThreads >= ThreadCount))
				{
					return false;
				}

				JobExecutionContext jobExecutionContext = new JobExecutionContext(jobContext, this);
				Thread thread = new Thread(ExecuteJob);
				jobExecutionContext.Thread = thread;
				workers.AddLast(jobExecutionContext);
				thread.Start(jobExecutionContext);
				return true;
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
				ThreadExecutionQueue queue = (ThreadExecutionQueue)jobExecutionContext.ExecutionQueue;
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
				jobContext.JobManager.JobStore.SetJobStatus(jobContext.JobData.Id, JobStatus.Executing, JobStatus.ShutdownTimeout, message);
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
