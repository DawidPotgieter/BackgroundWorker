using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Logic.DataModel.Internal.Jobs;

namespace BackgroundWorkerService.Logic.Interfaces.Internal
{
	/// <summary>
	/// The interface that must be implemented by ExecutionQueues.
	/// </summary>
	internal interface IExecutionQueue : IExecutionQueueBase
	{
		/// <summary>
		/// Occurs when a job finished executing.
		/// </summary>
		event EventHandler<JobFinishedExecutingEventArgs> JobFinishedExecuting;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is stopping.  This is set to let the queue know whether it can start executing more jobs.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is stopping; otherwise, <c>false</c>.
		/// </value>
		bool IsStopping { get; set; }

		/// <summary>
		/// Enqueues the specified job context.  
		/// </summary>
		/// <param name="jobContext">The job context.</param>
		/// <returns><c>false</c> if the queue is full or the job couldn't be enqueued.</returns>
		bool Enqueue(JobContext jobContext);

		/// <summary>
		/// Shutdowns the running jobs. If it cannot shutdown running jobs, it simply returns false.
		/// </summary>
		/// <returns></returns>
		bool ShutdownRunningJobs();

		/// <summary>
		/// Gets or sets the queue id.
		/// </summary>
		/// <value>
		/// The queue id.
		/// </value>
		new byte Id { get; set; }

		/// <summary>
		/// Gets or sets the max thread count.
		/// </summary>
		/// <value>
		/// The max thread count.
		/// </value>
		new uint ThreadCount { get; set; }
	}
}
