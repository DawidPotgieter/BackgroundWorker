using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Logic.DataModel.Scheduling;
using System.Collections.ObjectModel;
using BackgroundWorkerService.Logic.Configuration;
using Common.Logging;
using BackgroundWorkerService.Logic.DataModel.Alerts;

namespace BackgroundWorkerService.Logic.Implementation.JobStore
{
	/// <summary>
	/// An in memory implementation of <see cref="IJobStore"/>.  No limits are placed on the size and if the service is killed, all data is lost.
	/// </summary>
	public class RamJobStore : IJobStore
	{
		private Dictionary<long, JobData> jobs;
		private Dictionary<long, JobExecutionHistory> jobExecutionHistories;
		private Dictionary<long, Alert> alerts;
		private LinkedList<long> jobExecutionHistoryIdQueue;
		private ISettingsProvider settings;
		private ILog logger;
		private long maxHistoryRecords = 10000;

		/// <summary>
		/// Initializes a new instance of the <see cref="RamJobStore"/> class.
		/// </summary>
		public RamJobStore()
			: this(ConfigurationSettings.SettingsProvider, LogManager.GetCurrentClassLogger())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RamJobStore"/> class.
		/// </summary>
		/// <param name="settingsProvider">The settings provider.</param>
		/// <param name="logger">The logging provider.</param>
		public RamJobStore(ISettingsProvider settingsProvider, ILog logger)
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
				if (jobExecutionHistoryIdQueue == null)
				{
					jobExecutionHistoryIdQueue = new LinkedList<long>();
				}
				if (alerts == null)
				{
					alerts = new Dictionary<long, Alert>();
				}
				settings = settingsProvider;

				if (settings.JobStoreSettingsProviderType != null)
				{
					var jobStoreSettingsProvider = Helpers.Utils.CreateInstanceWithRequiredInterface<IRamJobStoreSettingsProvider>(settings.JobStoreSettingsProviderType.AssemblyQualifiedName, typeof(IRamJobStoreSettingsProvider).AssemblyQualifiedName);
					if (jobStoreSettingsProvider != null)
					{
						maxHistoryRecords = jobStoreSettingsProvider.MaxHistoryRecords;
					}
				}

				this.logger = logger;
			}
		}

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
		public JobData CreateJob(Type jobType, string data, string metaData, byte queueId, ISchedule schedule = null, Guid? uniqueId = null, string name = null, string description = null, string application = null, string group = null, TimeSpan? absoluteTimeout = null, JobStatus? jobStatus = null, DateTime? createdDate = null, bool suppressHistory = false, bool deleteWhenDone = false)
		{
			if (jobType == null)
			{
				throw new ArgumentException("jobType cannot be null.");
			}

			JobData job = null;
			job = new JobData
			{
				JobType = jobType,
				Data = data,
				MetaData = metaData,
				QueueId = queueId,
				Schedule = schedule,
				UniqueId = uniqueId ?? Guid.NewGuid(),
				AbsoluteTimeout = absoluteTimeout,
				Status = jobStatus ?? (schedule == null ? JobStatus.Ready : JobStatus.Scheduled),
				CreatedDate = createdDate ?? DateTime.Now,
				Id = (jobs.Count > 0 ? jobs.Keys.Max() + 1 : 1),
				Instance = Environment.MachineName,
				Application = application,
				Group = group,
				Name = name,
				Description = description,
				SuppressHistory = suppressHistory,
				DeleteWhenDone = deleteWhenDone,
				NextStartTime = (schedule != null ? schedule.GetNextOccurrence() : null),
			};
			lock (jobs)
			{
				jobs.Add(job.Id, job);
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
				jobs.Remove(jobId);
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
		/// <returns></returns>
		public bool UpdateJob(JobData job)
		{
			lock (jobs)
			{
				jobs.Remove(job.Id);

				//This bit is a temporary hack.  Will change later to move this out of jobstore.
				if (job.Schedule != null)
				{
					job.NextStartTime = job.Schedule.GetNextOccurrence(job.LastStartTime.HasValue ? job.LastStartTime.Value : DateTime.Now);
				}

				jobs.Add(job.Id, job);

				return true;
			}
		}

		/// <summary>
		/// Creates a job execution history record from the job data.
		/// </summary>
		/// <param name="job">The job data.</param>
		/// <returns></returns>
		public JobExecutionHistory CreateJobExecutionHistory(JobData job)
		{
			lock (jobExecutionHistories)
			{
				JobExecutionHistory jobExecutionHistory = new JobExecutionHistory
				{
					JobId = job.Id,
					JobUniqueId = job.UniqueId,
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

				lock (jobExecutionHistoryIdQueue)
				{
					//This bit ensures that the history doesn't cause an out of memory exception.  At 100K records, it should be more than about 500Mb or Ram max.
					jobExecutionHistoryIdQueue.AddLast(jobExecutionHistory.Id);
					if (jobExecutionHistoryIdQueue.Count > maxHistoryRecords)
					{
						jobExecutionHistoryIdQueue.RemoveFirst();
					}
				}

				return jobExecutionHistory;
			}
		}

		public ReadOnlyCollection<JobData> QueueReadyJobs(byte? queueId, uint take)
		{
			lock (jobs)
			{
				List<JobData> jobsList = new List<JobData>();
				var query = jobs.Where(j => j.Value.Status == JobStatus.Ready).OrderBy(j => j.Value.LastStartTime).AsQueryable();
				if (queueId.HasValue)
				{
					query = query.Where(j => j.Value.QueueId == queueId.Value);
				}
				jobsList = query.Take((int)take).Select(j => j.Value).ToList();
				jobsList.ForEach(j => j.Status = JobStatus.Queuing);
				return jobsList.AsReadOnly();
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
		public ReadOnlyCollection<JobData> GetJobs(uint skip = 0, uint take = 1, Guid[] jobUniqueIds = null, long[] jobIds = null, JobStatus[] jobStatuses = null, byte[] queueIds = null, string[] typeNames = null, string[] applications = null, string[] groups = null)
		{
			lock (jobs)
			{
				var query = jobs.OrderBy(j => j.Value.CreatedDate).AsQueryable();
				if (jobUniqueIds != null && jobUniqueIds.Length > 0)
				{
					query = query.Where(j => jobUniqueIds.Contains(j.Value.UniqueId));
				}
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
		public ReadOnlyCollection<JobExecutionHistory> GetJobExecutionHistories(uint skip = 0, uint take = 1, long[] jobHistoryIds = null, Guid[] jobUniqueIds = null, long[] jobIds = null, JobStatus[] jobStatuses = null, byte[] queueIds = null, string[] typeNames = null, string[] applications = null, string[] groups = null)
		{
			lock (jobExecutionHistories)
			{
				var query = jobExecutionHistories.OrderByDescending(j => j.Value.Id).AsQueryable();
				if (jobHistoryIds != null && jobHistoryIds.Length > 0)
				{
					query = query.Where(j => jobHistoryIds.Contains(j.Value.Id));
				}
				if (jobUniqueIds != null && jobUniqueIds.Length > 0)
				{
					query = query.Where(j => jobUniqueIds.Contains(j.Value.JobUniqueId));
				}
				if (jobIds != null && jobIds.Length > 0)
				{
					query = query.Where(j => jobIds.Contains(j.Value.JobId));
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

		public bool SetJobStatuses(long[] jobIds, JobStatus? oldStatus, JobStatus newStatus, string errorMessage = null, string instance = "Not Specified")
		{
			lock (jobs)
			{
				var existingJobs = jobs.Values.Where(j => jobIds.Contains(j.Id));
				foreach(var existingJob in existingJobs)
				{
					if (oldStatus.HasValue ? existingJob.Status == oldStatus.Value : true)
					{
						existingJob.Status = newStatus;
						if (errorMessage != null)
						{
							existingJob.LastErrorMessage = errorMessage;
						}
						if (instance != "Not Specified")
						{
							existingJob.Instance = instance;
						}
					}
					return true;
				}
			}
			return false;
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

		public ReadOnlyCollection<JobData> GetScheduleReadyJobs(DateTime nextExecutionStartDateTimeAtOrBefore)
		{
			lock(jobs)
			{
				var storedJobs = jobs.Where(j => j.Value.Status == JobStatus.Scheduled && j.Value.NextStartTime <= nextExecutionStartDateTimeAtOrBefore);
				return storedJobs.Select(j => j.Value).ToList().AsReadOnly();
			}
		}

		public ReadOnlyCollection<Alert> GetAlerts(uint skip = 0, uint take = 1)
		{
			lock (alerts)
			{
				var storedAlertsQuery = alerts.OrderBy(a => a.Key).Skip((int)skip).Take((int)take).AsQueryable();
				return storedAlertsQuery.Select(a =>
					new Alert
					{
						Id = a.Value.Id,
						JobId = a.Value.JobId,
						JobHistoryId = a.Value.JobHistoryId,
						Message = a.Value.Message,
					}).ToList().AsReadOnly();
			}
		}

		public Alert CreateAlert(long jobId, long? jobHistoryId, string message)
		{
			lock (alerts)
			{
				Alert alert = new Alert
				{
					Id = (alerts.Count > 0 ? alerts.Keys.Max() + 1 : 1),
					JobId = jobId,
					JobHistoryId = jobHistoryId,
					Message = message,
				};
				alerts.Add(alert.Id, alert);
				return alert;
			}
		}

		public bool DeleteAlerts(long[] ids = null)
		{
			lock (alerts)
			{
				var storedAlertsQuery = alerts.Select(a => a.Key).AsQueryable();
				if (ids != null)
				{
					storedAlertsQuery = storedAlertsQuery.Where(a => ids.Contains(a));
				}
				foreach (var key in storedAlertsQuery.ToList())
				{
					alerts.Remove(key);
				}
				return true;
			}
		}
	}
}