using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.DataModel.Jobs
{
	/// <summary>
	/// A container event argument that is used when signalling that a job has changed in the jobstore.
	/// </summary>
	public class JobActionRequiredEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JobActionRequiredEventArgs"/> class.
		/// </summary>
		/// <param name="job">The job.</param>
		public JobActionRequiredEventArgs(JobData job)
		{
			Job = job;
		}

		/// <summary>
		/// Gets the job information.  This reflects the updated/new job values.
		/// </summary>
		public JobData Job { get; private set; }
	}
}
