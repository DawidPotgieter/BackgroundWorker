using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.DataModel.Internal.Service
{
	/// <summary>
	/// Enumeration describing the statuses that the Service can be in.  Do not confuse this with the Windows Service that is a context around the service.
	/// </summary>
	internal enum ServiceStatus
	{
		/// <summary>
		/// The service is running and jobs will be queued and executed.
		/// </summary>
		Running,
		/// <summary>
		/// The service is not running and no jobs are queued or will be queued or executed while in this state.
		/// </summary>
		Stopped,

		/// <summary>
		/// The service is running, already queued jobs stay queued, but will not be executed.  No new jobs will be enqueued.
		/// </summary>
		Paused,

		/// <summary>
		/// The service is busy shutting down gracefully.
		/// </summary>
		Stopping,
	}
}
