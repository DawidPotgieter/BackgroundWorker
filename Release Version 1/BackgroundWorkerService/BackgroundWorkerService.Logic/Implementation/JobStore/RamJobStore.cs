using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Logic.DataModel.Scheduling;
using System.Collections.ObjectModel;
using BackgroundWorkerService.Logic.Configuration;

namespace BackgroundWorkerService.Logic.Implementation.JobStore
{
	/// <summary>
	/// An in memory implementation of <see cref="IJobStore"/>.  No limits are placed on the size and if the service is killed, all data is lost.
	/// </summary>
	public class RamJobStore : IJobStore
	{
		private static Dictionary<long, JobData> jobs;
		private static Dictionary<long, JobExecutionHistory> jobExecutionHistories;
		private ISettingsProvider settings;
		private ILoggingProvider logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="RamJobStore"/> class.
		/// </summary>
		public RamJobStore()
			: this(ConfigurationSettings.SettingsProvider, ConfigurationSettings.LoggingProvider)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RamJobStore"/> class.
		/// </summary>
		/// <param name="settingsProvider">The settings provider.</param>
		/// <param name="loggingProvider">The logging provider.</param>
		public RamJobStore(ISettingsProvider settingsProvider, ILoggingProvider loggingProvider)
		{
			lock (this)
			{
				if (jobs == null)
				{
					jobs = new Dictionary<long, JobData>();
				}
				if (jobExecutionHistories == null)
				{
					jobExecutionHistories = new Dictionary<long, JobExecutionHistory>();
				}
				settings = settingsProvider;
				logger = loggingProvider;
			}
		}

		/// <summary>
		/// Occurs when a job is created or updated or the status changes.
		/// </summary>
		public event EventHandler<JobActionRequiredEventArgs> JobActionRequired;
		/// <summary>
		/// Occurs when a job is deleted.
		/// </summary>
		public event EventHandler<JobActionRequiredEventArgs> JobDeleted;

		/// <summary>
		/// Creates a new job in the job store.
		/// </summary>
		/// <param name="jobType">Type of the job. Cannot be null.</param>
		/// <param name="data">The data.</param>
		/// <param name="metaData">The meta data.</param>
		/// <param name="queueId">The queue id.</param>
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
		public JobData CreateJob(Type jobType, string data, string metaData, byte queueId, string name = null, string description = null, string application = null, string group = null, TimeSpan? absoluteTimeout = null, JobStatus? jobStatus = JobStatus.Ready, DateTime? createdDate = null, bool suppressHistory = false, bool deleteWhenDone = false)
		{
			if (jobType == null)
			{
				throw new ArgumentException("jobType cannot be null.");
			}

			JobData job = null;
			lock (jobs)
			{
				job = new JobData
				{
					JobType = jobType,
					Data = data,
					MetaData = metaData,
					QueueId = queueId,
					Schedule = null,
					AbsoluteTimeout = absoluteTimeout,
					Status = jobStatus ?? JobStatus.Ready,
					CreatedDate = createdDate ?? DateTime.Now,
					Id = (jobs.Count > 0 ? jobs.Keys.Max() + 1 : 1),
					Instance = Environment.MachineName,
					Application = application,
					Group = group,
					Name = name,
					Description = description,
					SuppressHistory = suppressHistory,
					DeleteWhenDone = deleteWhenDone,
				};
				jobs.Add(job.Id, job);
			}
			var jobActionRequired = JobActionRequired;
			if (jobActionRequired != null)
			{
				jobActionRequired(this, new JobActionRequiredEventArgs(job));
			}
			
			return job;
		}

		/// <summary>
		/// Schedules (create with schedule) the job in the job store.
		/// </summary>
		/// <param name="jobType">Type of the job.</param>
		/// <param name="data">The data.</param>
		/// <param name="metaData">The meta data.</param>
		/// <param name="queueId">The queue id.</param>
		/// <param name="schedule">The schedule.</param>
		/// <param name="name">The name.</param>
		/// <param name="description">The description.</param>
		/// <param name="application">The application.</param>
		/// <param name="group">The group.</param>
		/// <param name="absoluteTimeout">The absolute timeout.</param>
		/// <param name="jobStatus">The job status.</param>
		/// <param name="createdDate">The created date. Jobs are ordered by creation date for execution so it's possible to prioritize jobs for queueing.</param>
		/// <param name="suppressHistory">if set to <c>true</c> no execution history records are created.</param>
		/// <param name="deleteWhenDone">if set to <c>true</c> the job is deleted after a successfull (status Done) execution.</param>
		/// <returns></returns>
		public JobData ScheduleJob(Type jobType, string data, string metaData, byte queueId, ISchedule schedule, string name = null, string description = null, string application = null, string group = null, TimeSpan? absoluteTimeout = null, JobStatus? jobStatus = JobStatus.Ready, DateTime? createdDate = null, bool suppressHistory = false, bool deleteWhenDone = false)
		{
			if (jobType == null)
			{
				throw new ArgumentException("jobType cannot be null.");
			}

			JobData job = null;
			lock (jobs)
			{
				job = new JobData
				{
					JobType = jobType,
					Data = data,
					MetaData = metaData,
					QueueId = queueId,
					Schedule = schedule,
					AbsoluteTimeout = absoluteTimeout,
					Status = jobStatus ?? JobStatus.Ready,
					CreatedDate = createdDate ?? DateTime.Now,
					Id = (jobs.Count > 0 ? jobs.Keys.Max() + 1 : 1),
					Instance = Environment.MachineName,
					Application = application,
					Group = group,
					Name = name,
					Description = description,
					SuppressHistory = suppressHistory,
					DeleteWhenDone = deleteWhenDone,
				};
				jobs.Add(job.Id, job);
			}
			var jobActionRequired = JobActionRequired;
			if (jobActionRequired != null)
			{
				jobActionRequired(this, new JobActionRequiredEventArgs(job));
			}

			return job;
		}


		/// <summary>
		/// Deletes the job permanently from the job store.
		/// </summary>
		/// <param name="jobId">The job id.</param>
		/// <param name="deleteHistory">If set to true, the history will also be deleted.</param>
		/// <returns></returns>
		public bool DeleteJob(long jobId, bool deleteHistory = false)
		{
			lock (jobs)
			{
				var job = jobs[jobId];
				jobs.Remove(jobId);
				var jobDeletedEvent = JobDeleted;
				if (jobDeletedEvent != null)
				{
					jobDeletedEvent(this, new JobActionRequiredEventArgs(job));
				}
				if (deleteHistory)
				{
					lock(jobExecutionHistories)
					{
						jobExecutionHistories.Remove(jobId);
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Deletes the job permanently from the job store.
		/// </summary>
		/// <param name="job">The job.</param>
		/// <param name="deleteHistory">If set to true, the history will also be deleted.</param>
		/// <returns></returns>
		public bool DeleteJob(JobData job, bool deleteHistory = false)
		{
			return DeleteJob(job.Id, deleteHistory);
		}

		/// <summary>
		/// Updates the job with the new values.
		/// </summary>
		/// <param name="job">The job.</param>
		/// <param name="triggerJobActionRequired">if set to <c>true</c>, cause <see cref="JobActionRequired"/> event to be raised.</param>
		/// <returns></returns>
		public bool UpdateJob(JobData job, bool triggerJobActionRequired = false)
		{
			lock (jobs)
			{
				jobs.Remove(job.Id);
				jobs.Add(job.Id, job);
				var jobActionRequired = JobActionRequired;
				if (triggerJobActionRequired && jobActionRequired != null)
				{
					jobActionRequired(this, new JobActionRequiredEventArgs(job));
				}

				return true;
			}
		}

		/// <summary>
		/// Creates a job execution history record from the job data.
		/// </summary>
		/// <param name="job">The job data.</param>
		/// <returns></returns>
		public bool CreateJobExecutionHistory(JobData job)
		{
			lock (jobExecutionHistories)
			{
				JobExecutionHistory jobExecutionHistory = new JobExecutionHistory
				{
					JobId = job.Id,
					AbsoluteTimeout = job.AbsoluteTimeout,
					CreatedDate = job.CreatedDate,
					Data = job.Data,
					EndTime = job.LastEndTime,
					ErrorMessage = job.LastErrorMessage,
					Instance = job.Instance,
					MetaData = job.MetaData,
					QueueId = job.QueueId,
					StartTime = job.LastStartTime,
					Status = job.Status,
					Id = (jobExecutionHistories.Count > 0 ? jobExecutionHistories.Keys.Max() + 1 : 1),
					Application = job.Application,
					Group = job.Group,
					Name = job.Name,
					Description = job.Description,
					JobType = job.JobType,
					Success = job.Status == JobStatus.Done,
				};

				jobExecutionHistories.Add(jobExecutionHistory.Id, jobExecutionHistory);
				return true;
			}
		}

		/// <summary>
		/// Queues the next non scheduled job that is in ready state in the job store.
		/// </summary>
		/// <param name="queueId">The queue id to find the next job for.</param>
		/// <returns></returns>
		public JobData QueueNextNonScheduledReadyJob(int? queueId)
		{
			lock (jobs)
			{
				JobData job = null;
				var query = jobs.Where(j => j.Value.Schedule == null).OrderBy(j => j.Value.CreatedDate).AsQueryable();
				if (queueId.HasValue)
				{
					query = query.Where(j => j.Value.QueueId == queueId.Value);
				}
				job = query.FirstOrDefault(j => j.Value.Status == JobStatus.Ready).Value;
				if (job != null)
				{
					lock (job)
					{
						job.Status = JobStatus.Queuing;
					}
				}
				return job;
			}
		}

		/// <summary>
		/// Queues the next scheduled job that is in ready state in the job store.
		/// </summary>
		/// <returns></returns>
		public JobData QueueNextScheduledReadyJob()
		{
			lock (jobs)
			{
				JobData job = null;
				var query = jobs.Where(j => j.Value.Schedule != null).OrderBy(j => j.Value.CreatedDate).AsQueryable();
				job = query.FirstOrDefault(j => j.Value.Status == JobStatus.Ready).Value;
				if (job != null)
				{
					lock (job)
					{
						job.Status = JobStatus.Queuing;
					}
				}
				return job;
			}
		}

		/// <summary>
		/// Sets job in status <see cref="JobStatus.Queuing"/> or <see cref="JobStatus.Queued"/> to status <see cref="JobStatus.Ready"/>.
		/// </summary>
		/// <param name="job">The job to dequeue.</param>
		public void DequeueJob(JobData job)
		{
			lock (jobs)
			{
				var existingJob = jobs.FirstOrDefault(j => j.Key == job.Id);
				if (existingJob.Value != null && (existingJob.Value.Status == JobStatus.Queuing || existingJob.Value.Status == JobStatus.Queued))
				{
					lock (existingJob.Value)
					{
						existingJob.Value.Status = JobStatus.Ready;
					}
				}
			}
		}

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
		public ReadOnlyCollection<JobData> GetJobs(uint skip = 0, uint take = 1, long[] jobIds = null, JobStatus[] jobStatuses = null, byte[] queueIds = null, string[] typeNames = null, string[] applications = null, string[] groups = null)
		{
			lock (jobs)
			{
				var query = jobs.OrderBy(j => j.Value.CreatedDate).AsQueryable();
				if (jobIds != null && jobIds.Length > 0)
				{
					query = query.Where(j => jobIds.Contains(j.Value.Id));
				}
				if (jobStatuses != null && jobStatuses.Length > 0)
				{
					query = query.Where(j => jobStatuses.Contains(j.Value.Status));
				}
				if (queueIds != null && queueIds.Length > 0)
				{
					query = query.Where(j => queueIds.Select(q => (int)q).Contains(j.Value.QueueId));
				}
				if (typeNames != null && typeNames.Length > 0)
				{
					query = query.Where(j => typeNames.Contains(j.Value.JobType.AssemblyQualifiedName));
				}
				if (applications != null && applications.Length > 0)
				{
					query = query.Where(j => applications.Contains(j.Value.Application));
				}
				if (groups != null && groups.Length > 0)
				{
					query = query.Where(j => groups.Contains(j.Value.Group));
				}
				return query.Skip((int)skip).Take((int)take).Select(j => j.Value).ToList().AsReadOnly();
			}
		}

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
		public ReadOnlyCollection<JobExecutionHistory> GetJobExecutionHistories(uint skip = 0, uint take = 1, long[] jobIds = null, JobStatus[] jobStatuses = null, byte[] queueIds = null, string[] typeNames = null, string[] applications = null, string[] groups = null)
		{
			lock (jobExecutionHistories)
			{
				var query = jobExecutionHistories.OrderByDescending(j => j.Value.Id).AsQueryable();
				if (jobIds != null && jobIds.Length > 0)
				{
					query = query.Where(j => jobIds.Contains(j.Value.Id));
				}
				if (jobStatuses != null && jobStatuses.Length > 0)
				{
					query = query.Where(j => jobStatuses.Contains(j.Value.Status));
				}
				if (queueIds != null && queueIds.Length > 0)
				{
					query = query.Where(j => queueIds.Select(q => (int)q).Contains(j.Value.QueueId));
				}
				if (typeNames != null && typeNames.Length > 0)
				{
					query = query.Where(j => typeNames.Contains(j.Value.JobType.AssemblyQualifiedName));
				}
				if (applications != null && applications.Length > 0)
				{
					query = query.Where(j => applications.Contains(j.Value.Application));
				}
				if (groups != null && groups.Length > 0)
				{
					query = query.Where(j => groups.Contains(j.Value.Group));
				}
				return query.Skip((int)skip).Take((int)take).Select(j => j.Value).ToList().AsReadOnly();
			}
		}

		/// <summary>
		/// Tries to set the job status of the specified job.
		/// </summary>
		/// <param name="jobId">The job id.</param>
		/// <param name="oldStatus">The old status.  This is used to ensure that job status change is what the caller expects. Use null if you don't care what the current job status is.</param>
		/// <param name="newStatus">The new status.</param>
		/// <param name="errorMessage">The optional error message.</param>
		/// <param name="metaData">The optional meta data.</param>
		/// <returns></returns>
		public bool SetJobStatus(long jobId, JobStatus? oldStatus, JobStatus newStatus, string errorMessage = null, string metaData = null)
		{
			lock (jobs)
			{
				var existingJob = jobs.Values.FirstOrDefault(j => j.Id == jobId);
				if (existingJob != null && (oldStatus.HasValue ? existingJob.Status == oldStatus.Value : true))
				{
					existingJob.Status = newStatus;
					if (errorMessage != null)
					{
						existingJob.LastErrorMessage = errorMessage;
					}
					if (metaData != null)
					{
						existingJob.MetaData = metaData;
					}
					var jobActionRequired = JobActionRequired;
					if (jobActionRequired != null)
					{
						jobActionRequired(this, new JobActionRequiredEventArgs(existingJob));
					}
					return true;
				}
			}
			return false;
		}
	}
}
