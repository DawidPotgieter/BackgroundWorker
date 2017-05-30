using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.Interfaces
{
	/// <summary>
	/// This interface is provided as a way to expose certain queue information to service consumers.
	/// </summary>
	public interface IExecutionQueueBase
	{
		/// <summary>
		/// Gets the queue id.
		/// </summary>
		/// <value>
		/// The queue id.
		/// </value>
		byte Id { get; }

		/// <summary>
		/// Gets the active (executing) threads.
		/// </summary>
		uint ActiveThreads { get; }

		/// <summary>
		/// Gets the max thread count.
		/// </summary>
		/// <value>
		/// The max thread count.
		/// </value>
		uint ThreadCount { get; }
	}
}
