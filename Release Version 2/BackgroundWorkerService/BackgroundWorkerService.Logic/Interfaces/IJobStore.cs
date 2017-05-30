using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Logic.DataModel.Scheduling;
using System.Collections.ObjectModel;
using BackgroundWorkerService.Logic.DataModel.Alerts;

namespace BackgroundWorkerService.Logic.Interfaces
{
	/// <summary>
	/// The interface that needs to be implemented by JobStore providers.
	/// </summary>
	public interface IJobStore
	{
		/// <summary>
		/// Creates a new job in the job store.
		/// </summary>
		/// <param name="jobType">Type of the job. Cannot be null.</param>
		/// <param name="data">The data.</param>
		/// <param name="metaData">The meta data.</param>
		/// <param name="queueId">The queue id.</param>
		/// <param name="schedule">The schedule.</param>
		/// <param name="uniqueId">The unique id that you want to assign for this job.  Usefull when deleting/re-creating a job that needs to be locatable.</param>
		/// <param name="name">The name.</param>
		/// <param name="description">The description.</param>
		/// <param name="application">The application name.</param>
		/// <param name="group">The group name.</param>
		/// <param name="absoluteTimeout">The absolute timeout.</param>
		/// <param name="jobStatus">The job status.</param>
		/// <param name="createdDate">The created date. Jobs are ordered by creation date for execution so it's possible to prioritize jobs for queueing.</param>
		/// <param name="suppressHistory">if set to <c>true</c> no execution history records are created.</param>
		/// <param name="deleteWhenDone">if set to <c>true</c> the job is deleted after a successfull (status Done) execution.</param>
		/// <returns></returns>
		/// <remarks>
		/// This must cause <see cref="JobActionRequired"/> event to be raised.
		/// </remarks>
		JobData CreateJob(Type jobType, string data, string metaData, byte queueId, ISchedule schedule = null, Guid? uniqueId = null, string name = null, string description = null, string application = null, string group = null, TimeSpan? absoluteTimeout = null, JobStatus? jobStatus = null, DateTime? createdDate = null, bool suppressHistory = false, bool deleteWhenDone = false);

		/// <summary>
		/// Deletes the job permanently from the job store.
		/// </summary>
		/// <param name="jobId">The job id.</param>
		/// <param name="deleteHistory">If set to true, the history will also be deleted.</param>
		/// <returns></returns>
		bool DeleteJob(long jobId, bool deleteHistory = false);

		/// <summary>
		/// Deletes the job permanently from the job store.
		/// </summary>
		/// <param name="job">The job.</param>
		/// <param name="deleteHistory">If set to true, the history will also be deleted.</param>
		/// <returns></returns>
		bool DeleteJob(JobData job, bool deleteHistory = false);

		/// <summary>
		/// Updates the job with the new values.
		/// </summary>
		/// <param name="job">The job.</param>
		/// <returns></returns>
		bool UpdateJob(JobData job);

		/// <summary>
		/// Creates a job execution history record from the job data.
		/// </summary>
		/// <param name="job">The job data.</param>
		/// <returns></returns>
		JobExecutionHistory CreateJobExecutionHistory(JobData job);

		/// <summary>
		/// Queues the next non scheduled job that is in ready state in the job store.
		/// </summary>
		/// <param name="queueId">The queue id to find the next job for.</param>
		/// <param name="take">The number of jobs to take.</param>
		/// <returns></returns>
		/// <remarks>
		/// This must be an "atomic" operation and the status of the job must be <see cref="JobStatus.Queuing"/> before it's returned.
		/// It must lock multiple instances of the service from queueing the same job, i.e. db providers must lock the table.
		/// Order jobs by CreatedDate before determining the next job to queue.
		/// </remarks>
		ReadOnlyCollection<JobData> QueueReadyJobs(byte? queueId, uint take);

		/// <summary>
		/// Gets a list of jobs with the specified filter criteria.  The nullable filters are only used if non-null values specified.
		/// </summary>
		/// <param name="skip">How many records to skip.</param>
		/// <param name="take">How many records to take.</param>
		/// <param name="jobIds">The job ids.</param>
		/// <param name="jobStatuses">The job statuses.</param>
		/// <param name="queueIds">The queue ids.</param>
		/// <param name="typeNames">The type names.</param>
		/// <param name="applications">The applications.</param>
		/// <param name="groups">The groups.</param>
		/// <returns></returns>
		/// <remarks>
		/// The query must first order the records by CreatedDate before doing paging operations.
		/// </remarks>
		ReadOnlyCollection<JobData> GetJobs(uint skip = 0, uint take = 1, Guid[] jobUniqueIds = null, long[] jobIds = null, JobStatus[] jobStatuses = null, byte[] queueIds = null, string[] typeNames = null, string[] applications = null, string[] groups = null);

		/// <summary>
		/// Gets the job execution histories with the specified criteria. The nullable filters are only used if non-null values specified.
		/// </summary>
		/// <param name="skip">The skip.</param>
		/// <param name="take">The take.</param>
		/// <param name="jobIds">The job ids.</param>
		/// <param name="jobStatuses">The job statuses.</param>
		/// <param name="queueIds">The queue ids.</param>
		/// <param name="typeNames">The type names.</param>
		/// <param name="applications">The applications.</param>
		/// <param name="groups">The groups.</param>
		/// <returns></returns>
		/// <remarks>
		/// The query must first order the records in descending order by Id before doing paging operations.
		/// </remarks>
		ReadOnlyCollection<JobExecutionHistory> GetJobExecutionHistories(uint skip = 0, uint take = 1, long[] jobHistoryIds = null, Guid[] jobUniqueIds = null, long[] jobIds = null, JobStatus[] jobStatuses = null, byte[] queueIds = null, string[] typeNames = null, string[] applications = null, string[] groups = null);

		bool SetJobStatuses(long[] jobIds, JobStatus? oldStatus, JobStatus newStatus, string errorMessage = null, string instance = "Not Specified");

		/// <summary>
		/// Sets job in status <see cref="JobStatus.Queuing"/> or <see cref="JobStatus.Queued"/> to status <see cref="JobStatus.Ready"/>.
		/// </summary>
		/// <param name="job">The job to dequeue.</param>
		void DequeueJob(JobData job);

		ReadOnlyCollection<JobData> GetScheduleReadyJobs(DateTime nextExecutionStartDateTimeAtOrBefore);

		ReadOnlyCollection<Alert> GetAlerts(uint skip = 0, uint take = 1);

		Alert CreateAlert(long jobId, long? jobHistoryId, string message);

		bool DeleteAlerts(long[] ids = null);
	}
}
