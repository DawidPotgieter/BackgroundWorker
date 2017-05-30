using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.DataModel.Jobs
{
	/// <summary>
	/// An enum describing the result of a call to <see cref="Interfaces.IJob.Execute"/>.
	/// </summary>
	public enum JobResultStatus
	{
		/// <summary>
		/// Job returned with a success result.
		/// </summary>
		Success = 16,
		/// <summary>
		/// Job returned with fail, but will automatically be re-queued for execution.
		/// </summary>
		FailAutoRetry = -2,
		/// <summary>
		/// Job returned with fail, but the consumer could reset the status to retry at some stage.
		/// </summary>
		FailRetry = -1,
		/// <summary>
		/// A critical failure occurred during job execution.  Use this status as the default return value if none is set by consumer code.
		/// </summary>
		Fail = -16,
	}
}
