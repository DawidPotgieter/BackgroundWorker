using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.Interfaces.Internal;
using System.Threading;

namespace BackgroundWorkerService.Logic.DataModel.Internal.Jobs
{
	/// <summary>
	/// Wrapper class containing all the information required to represent a Jobs execution context when scheduled on an execution queue.
	/// </summary>
	internal class JobExecutionContext
	{
		internal JobExecutionContext(JobContext jobContext, IExecutionQueue executionQueue)
		{
			JobContext = jobContext;
			ExecutionQueue = executionQueue;
		}

		internal JobExecutionContext(JobContext jobContext, IExecutionQueue executionQueue, Thread thread)
			: this(jobContext, executionQueue)
		{
			Thread = thread;
		}

		/// <summary>
		/// Gets or sets the execution queue that this execution context belongs to.
		/// </summary>
		/// <value>
		/// The execution queue.
		/// </value>
		internal IExecutionQueue ExecutionQueue { get; set; }

		/// <summary>
		/// Gets or sets the job context information.
		/// </summary>
		/// <value>
		/// The job context.
		/// </value>
		internal JobContext JobContext { get; set; }

		/// <summary>
		/// Gets or sets the thread that this execution context was launched on.
		/// </summary>
		/// <value>
		/// The thread.
		/// </value>
		/// <remarks>
		/// Can be null and is only used by some of the execution queue implementations.
		/// </remarks>
		internal Thread Thread { get; set; }
	}
}
