using System;
using System.Collections.Generic;
using System.Text;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	/// <summary>
	/// An enumeration describing the different states that jobs can be in.
	/// </summary>
	public enum JobStatus
	{
		/// <summary>
		/// Not queued or processed yet. The next time queueing is done, this job will get queued if there is queue space.
		/// </summary>
		Ready = 0,
		/// <summary>
		/// The job has been queued by the job store.  The job has not been assigned to a thread to run.
		/// </summary>
		Queuing = 1,
		/// <summary>
		/// The job is queued in memory by the background service and awaiting an available thread to start executing or awaiting a scheduled trigger.
		/// </summary>
		Queued = 2,
		/// <summary>
		/// The job is queued in memory by the background service and awaiting an available thread to start executing. This is used with scheduled jobs to trigger them firing immediately.
		/// For non-scheduled jobs, this value is not used.
		/// </summary>
		ScheduleRunNow = 4,
		/// <summary>
		/// The job is busy executing.
		/// </summary>
		Executing = 8,
		/// <summary>
		/// Job was executed without errors.
		/// </summary>
		Done = 16,
		/// <summary>
		/// The job is in a pending state and will not be queued until explicitly set to Ready.  The system does not use this state, it is provided as a way to store jobs that have
		/// not run and you want it out of the way.
		/// </summary>
		Pending = 32,
		/// <summary>
		/// Fail status indicating that the consumer wants this job to be reset to Ready at some stage.
		/// </summary>
		FailRetry = -1,
		/// <summary>
		/// This status indicates that the job failed, but will automatically be queued for execution.
		/// </summary>
		FailAutoRetry = -2,
		/// <summary>
		/// The job failed to complete within the shutdown timeout specified while the service was being shutdown.
		/// </summary>
		ShutdownTimeout = -4,
		/// <summary>
		/// The job reached it's specified absolute timeout and was terminated.
		/// </summary>
		ExecutionTimeout = -8,
		/// <summary>
		/// Job failed, indicating a critical or unhandled failure.
		/// </summary>
		Fail = -16,
		/// <summary>
		/// The job is set to deleted - this can be used for archiving, but is also used to indicate jobs that have failed type loading.
		/// </summary>
		Deleted = -32,
	}
}
