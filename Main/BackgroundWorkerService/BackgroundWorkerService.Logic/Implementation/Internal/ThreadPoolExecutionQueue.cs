using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces.Internal;
using System.Threading;
using BackgroundWorkerService.Logic.DataModel.Internal.Jobs;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using System.Diagnostics;

namespace BackgroundWorkerService.Logic.Implementation.Internal
{
	/// <summary>
	/// An implementation of <see cref="IExecutionQueue"/> that uses the .Net ThreadPool to execute jobs on.
	/// This queue does not support job termination or timeouts on individual jobs.
	/// A note on the ThreadPool :
	/// Even though you can specify a threadpool of say 100, it doesn't mean you'll get 100 threads.  The threadpool is controlled by the .Net framework
	/// and is usually by default on the order of 25 threads (set at machine config level).  If you set the ThreadCount on this queue to higher than that,
	/// the jobs will simply be dumped on the threadpool queue.  For better visibility, don't set the threadcount too high.
	/// </summary>
	internal class ThreadPoolExecutionQueue : IExecutionQueue
	{
		public ThreadPoolExecutionQueue()
		{
		}

		public event EventHandler<JobFinishedExecutingEventArgs> JobFinishedExecuting;

		public byte Id { get; set; }

		private object activeThreads = (uint)0;
		public uint ActiveThreads
		{
			get
			{
				return (uint)activeThreads;
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
			lock (activeThreads)
			{
				if (IsStopping || ((uint)activeThreads >= ThreadCount))
				{
					return false;
				}

				JobExecutionContext jobExecutionContext = new JobExecutionContext(jobContext, this);
				activeThreads = (uint)activeThreads + 1;
				if (ThreadPool.QueueUserWorkItem(ExecuteJob, jobExecutionContext))
				{
					return true;
				}
				else
				{
					activeThreads = (uint)activeThreads - 1;
				}
				return false;
			}
		}

		static void ExecuteJob(Object jobExecutionContextObject)
		{
			JobExecutionContext jobExecutionContext = (JobExecutionContext)jobExecutionContextObject;
			JobContext jobContext = jobExecutionContext.JobContext;
			Debug.WriteLine(DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss:fffffff") + " : " + jobContext.JobData.Id + " ExecuteJob Enter. Queue = " + jobExecutionContext.ExecutionQueue.Id);
			try
			{
				IJobExecutorFactory jobExecutorFactory = new JobExecutorFactory();
				IJobExecutor jobExecutor = jobExecutorFactory.GetJobExecutor(jobContext);
				jobExecutor.ExecuteJob(jobContext);
				ThreadPoolExecutionQueue queue = (ThreadPoolExecutionQueue)jobExecutionContext.ExecutionQueue;
				lock (queue.activeThreads)
				{
					queue.activeThreads = (uint)queue.activeThreads - 1;
					var jobFinishedEvent = queue.JobFinishedExecuting;
					if (jobFinishedEvent != null)
					{
						jobFinishedEvent(queue, new JobFinishedExecutingEventArgs(jobContext.JobData.Id));
					}
				}
			}
			catch (ThreadAbortException)
			{
				string message = string.Format("Job has exceeded the ShutdownTimeout and was terminated abnormally.", jobContext.JobData.Id);
				jobContext.JobManager.JobStore.SetJobStatuses(new long[] { jobContext.JobData.Id }, JobStatus.Executing, JobStatus.ShutdownTimeout, message);
			}
			Debug.WriteLine(DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss:fffffff") + " : " + jobContext.JobData.Id + " ExecuteJob Exit. Queue = " + jobExecutionContext.ExecutionQueue.Id);
		}

		public bool ShutdownRunningJobs()
		{
			//Can't do anything when using he threadpool
			return false;
		}
	}
}
