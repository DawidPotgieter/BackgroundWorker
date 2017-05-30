using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using System.Collections.ObjectModel;
using BackgroundWorkerService.Logic.DataModel.Scheduling;

namespace BackgroundWorkerService.Logic.Interfaces
{
	/// <summary>
	/// The interface that must be implemented by any JobManager.
	/// </summary>
	public interface IJobManager
	{
		/// <summary>
		/// Occurs when the job store raises a JobActionRequired event.
		/// </summary>
		event EventHandler<JobActionRequiredEventArgs> JobActionRequired;

		/// <summary>
		/// Occurs when the job stoer raises a JobDeleted event.
		/// </summary>
		event EventHandler<JobActionRequiredEventArgs> JobDeleted;

		/// <summary>
		/// Gets the job store that is used to persist jobs.
		/// </summary>
		IJobStore JobStore { get; }

		/// <summary>
		/// Gets the logger for this job manager.
		/// </summary>
		ILoggingProvider Logger { get; }
	}
}
