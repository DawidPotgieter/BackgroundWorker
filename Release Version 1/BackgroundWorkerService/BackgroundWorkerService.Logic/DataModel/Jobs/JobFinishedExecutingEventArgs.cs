using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.DataModel.Jobs
{
	/// <summary>
	/// Contains the information required when raising a JobFinishedExecuting event.
	/// </summary>
	public class JobFinishedExecutingEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JobFinishedExecutingEventArgs"/> class.
		/// </summary>
		/// <param name="jobId">The job id.</param>
		public JobFinishedExecutingEventArgs(long jobId)
		{
			JobId = jobId;
		}

		/// <summary>
		/// Gets the job id that finished executing.
		/// </summary>
		public long JobId { get; private set; }
	}
}
