using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Logic.DataModel.Scheduling;
using System.Collections.ObjectModel;
using System.Data.Linq;
using BackgroundWorkerService.Logic.Configuration;
using System.Transactions;
using System.ComponentModel;
using System.Diagnostics;
using BackgroundWorkerService.Logic.Helpers;
using BackgroundWorkerService.Logic.Interfaces.Internal;
using Common.Logging;
using BackgroundWorkerService.Logic.DataModel.Alerts;

namespace BackgroundWorkerService.Logic.Implementation.JobStore.Linq2Sql
{
	/// <summary>
	/// A concrete implementation of <see cref="IJobStore"/> that uses Linq2Sql to store jobs in a SQL Server database.
	/// </summary>
	public class Linq2SqlJobStore : IJobStore
	{
		private ISettingsProvider settings;
		private ILog logger;
		private Random randomizer;
		private string connectionString;
		private TimeSpan transactionTimeout;

		/// <summary>
		/// Initializes a new instance of the <see cref="Linq2SqlJobStore"/> class.
		/// </summary>
		public Linq2SqlJobStore()
			: this(ConfigurationSettings.SettingsProvider, LogManager.GetCurrentClassLogger(), (ILinq2SqlJobStoreSettingsProvider)Utils.CreateInstanceWithRequiredInterface(ConfigurationSettings.SettingsProvider.JobStoreSettingsProviderType.AssemblyQualifiedName, "ILinq2SqlJobStoreSettingsProvider"))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Linq2SqlJobStore"/> class.
		/// </summary>
		/// <param name="settingsProvider">The settings provider.</param>
		/// <param name="loggingProvider">The logging provider.</param>
		/// <param name="jobStoreSettingsProvider">The job store settings provider.</param>
		public Linq2SqlJobStore(ISettingsProvider settingsProvider, ILog logger, ILinq2SqlJobStoreSettingsProvider jobStoreSettingsProvider)
		{
			connectionString = jobStoreSettingsProvider.ConnectionString;
			transactionTimeout = jobStoreSettingsProvider.TransactionLockTimeout;

			randomizer = new Random((int)DateTime.Now.TimeOfDay.TotalSeconds);
			this.logger = logger;
			settings = settingsProvider;
		}

		public JobData CreateJob(Type jobType, string data, string metaData, byte queueId, ISchedule schedule = null, Guid? uniqueId = null, string name = null, string description = null, string application = null, string group = null, TimeSpan? absoluteTimeout = null, JobStatus? jobStatus = null, DateTime? createdDate = null, bool suppressHistory = false, bool deleteWhenDone = false)
		{
			if (jobType == null)
			{
				throw new ArgumentException("jobType cannot be null.");
			}

			BackgroundWorkerJob newJob = new BackgroundWorkerJob
			{
				UniqueId = uniqueId ?? Guid.NewGuid(),
				AbsoluteTimeout = absoluteTimeout,
				CreatedDate = DateTime.Now,
				Data = data,
				MetaData = metaData,
				QueueId = queueId,
				StatusId = (int?)jobStatus ?? (schedule == null ? (int)JobStatus.Ready : (int)JobStatus.Scheduled),
				Type = jobType.AssemblyQualifiedName,
				ScheduleType = schedule != null ? schedule.GetType().AssemblyQualifiedName : null,
				Schedule = schedule != null ? Utils.SerializeObject(schedule, schedule.GetType()) : null,
				Instance = null, //This is just a default to show that no instance has picked this up
				Application = application,
				Group = group,
				Name = name,
				Description = description,
				SuppressHistory = suppressHistory,
				DeleteWhenDone = deleteWhenDone,
				NextExecutionStartDateTime = (schedule != null ? schedule.GetNextOccurrence() : null),
			};
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable, Timeout = transactionTimeout }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					context.BackgroundWorkerJobs.InsertOnSubmit(newJob);
					try
					{
						context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
						ts.Complete();
						JobData job = GetJobData(newJob);
						return job;
					}
					catch (Exception ex)
					{
						logger.Error(string.Format("Failed to create '{0}' with data '{1}'", jobType.AssemblyQualifiedName, data), ex);
					}
				}
			}
			return null;
		}

		public bool DeleteJob(long jobId, bool deleteHistory = false)
		{
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable, Timeout = transactionTimeout }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					var job = context.BackgroundWorkerJobs.FirstOrDefault(j => j.Id == jobId);
					if (job != null)
					{
						if (deleteHistory)
						{
							context.BackgroundWorkerJobExecutionHistories.DeleteAllOnSubmit(context.BackgroundWorkerJobExecutionHistories.Where(jh => jh.JobId == jobId));
						}
						context.BackgroundWorkerJobs.DeleteOnSubmit(job);
						try
						{
							context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
							ts.Complete();
							return true;
						}
						catch (Exception ex)
						{
							logger.Error(string.Format("Failed to delete job '{0}'.", jobId), ex);
						}
					}
				}
			}
			return false;
		}

		public bool DeleteJob(JobData job, bool deleteHistory = false)
		{
			return DeleteJob(job.Id, deleteHistory);
		}

		public bool UpdateJob(JobData job)
		{
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					//If the record we're trying to update is locked (some other thread or service is writing to it), this will return false.
					var storedJob = context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs WITH (ROWLOCK, XLOCK) WHERE Id = {0}", job.Id).FirstOrDefault();
					if (storedJob != null)
					{
						storedJob.AbsoluteTimeout = job.AbsoluteTimeout;
						storedJob.CreatedDate = job.CreatedDate;
						storedJob.Data = job.Data;
						storedJob.Instance = job.Instance;
						storedJob.LastErrorMessage = job.LastErrorMessage;
						storedJob.LastExecutionEndDateTime = job.LastEndTime;
						storedJob.LastExecutionStartDateTime = job.LastStartTime;
						storedJob.MetaData = job.MetaData;
						storedJob.QueueId = job.QueueId;
						storedJob.StatusId = (int)job.Status;
						storedJob.Application = job.Application;
						storedJob.Group = job.Group;
						storedJob.SuppressHistory = job.SuppressHistory;
						storedJob.DeleteWhenDone = job.DeleteWhenDone;
						storedJob.UniqueId = job.UniqueId;
						storedJob.NextExecutionStartDateTime = job.NextStartTime;
						storedJob.Name = job.Name;
						storedJob.Description = job.Description;
						if (job.Schedule != null)
						{
							storedJob.ScheduleType = job.Schedule.GetType().AssemblyQualifiedName;
							//This bit is a temporary hack.  Will change later to move this out of jobstore.
							string newSchedule = Utils.SerializeObject(job.Schedule, job.Schedule.GetType());
							if (newSchedule != storedJob.Schedule)
							{
								storedJob.NextExecutionStartDateTime = job.Schedule.GetNextOccurrence(storedJob.LastExecutionStartDateTime.HasValue ? storedJob.LastExecutionStartDateTime.Value : DateTime.Now);
							}
							storedJob.Schedule = newSchedule;
						}
						else
						{
							storedJob.ScheduleType = null;
							storedJob.Schedule = null;
							storedJob.NextExecutionStartDateTime = null;
						}
						try
						{
							context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
							ts.Complete();
							return true;
						}
						catch (Exception ex)
						{
							logger.Error(string.Format("Failed to submit changes to the database."), ex);
						}
						return false;
					}
				}
			}
			return false;
		}

		public JobExecutionHistory CreateJobExecutionHistory(JobData job)
		{
			BackgroundWorkerJobExecutionHistory history = new BackgroundWorkerJobExecutionHistory
			{
				AbsoluteTimeout = job.AbsoluteTimeout,
				CreatedDate = job.CreatedDate,
				Instance = job.Instance,
				JobUniqueId = job.UniqueId,
				Data = job.Data,
				EndDateTime = job.LastEndTime,
				ErrorMessage = job.LastErrorMessage,
				JobId = job.Id,
				MetaData = job.MetaData,
				QueueId = job.QueueId,
				StartDateTime = job.LastStartTime ?? DateTime.Now,
				StatusId = (int)job.Status,
				Success = job.Status == JobStatus.Done,
				Type = job.JobType.AssemblyQualifiedName,
				Application = job.Application,
				Group = job.Group,
				Name = job.Name,
				Description = job.Description,
			};
			using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
			{
				context.BackgroundWorkerJobExecutionHistories.InsertOnSubmit(history);
				try
				{
					context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
					return new JobExecutionHistory
					{
						AbsoluteTimeout = history.AbsoluteTimeout,
						Application = history.Application,
						CreatedDate = history.CreatedDate,
						Data = history.Data,
						Description = history.Description,
						EndTime = history.EndDateTime,
						ErrorMessage = history.ErrorMessage,
						Group = history.Group,
						Id = history.Id,
						Instance = history.Instance,
						JobId = history.JobId,
						JobType = Type.GetType(history.Type),
						JobUniqueId = history.JobUniqueId,
						MetaData = history.MetaData,
						Name = history.Name,
						QueueId = history.QueueId,
						StartTime = history.StartDateTime,
						Status = (JobStatus)history.StatusId,
						Success = history.Success ?? false,
					};
				}
				catch (Exception ex)
				{
					logger.Error(string.Format("Failed to submit changes to the database."), ex);
				}
			}
			return null;
		}

		public ReadOnlyCollection<JobData> GetJobs(uint skip = 0, uint take = 1, Guid[] jobUniqueIds = null, long[] jobIds = null, JobStatus[] jobStatuses = null, byte[] queueIds = null, string[] typeNames = null, string[] applications = null, string[] groups = null)
		{
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable, Timeout = transactionTimeout }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					try
					{
						//In order to get a table lock, we have to resort to SQL.  Yeah, I said table lock.  Want to get the real actual values here.
						context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs WITH (TABLOCKX)");
						var query = context.BackgroundWorkerJobs.OrderBy(j => j.CreatedDate).AsQueryable();
						if (jobUniqueIds != null && jobUniqueIds.Length > 0)
						{
							query = query.Where(j => jobUniqueIds.Contains(j.UniqueId));
						}
						if (jobIds != null && jobIds.Length > 0)
						{
							query = query.Where(j => jobIds.Contains(j.Id));
						}
						if (jobStatuses != null && jobStatuses.Length > 0)
						{
							query = query.Where(j => jobStatuses.Contains((JobStatus)j.StatusId));
						}
						if (queueIds != null && queueIds.Length > 0)
						{
							query = query.Where(j => queueIds.Select(q => (int)q).Contains(j.QueueId));
						}
						if (typeNames != null && typeNames.Length > 0)
						{
							query = query.Where(j => typeNames.Contains(j.Type));
						}
						if (applications != null && applications.Length > 0)
						{
							query = query.Where(j => applications.Contains(j.Application));
						}
						if (groups != null && groups.Length > 0)
						{
							query = query.Where(j => groups.Contains(j.Group));
						}

						List<JobData> jobs = new List<JobData>();
						var storedJobs = query.Skip((int)skip).Take((int)take).ToList();
						foreach (var storedJob in storedJobs)
						{
							Type jobType = Type.GetType(storedJob.Type);
							if (jobType != null)
							{
								var jobData = GetJobData(storedJob);
								jobs.Add(jobData);
							}
							else
							{
								storedJob.StatusId = (int)JobStatus.Deleted;
								storedJob.LastErrorMessage = string.Format("Could not load type '{0}.'", storedJob.Type);
							}
						}

						context.SubmitChanges();
						ts.Complete();

						return jobs.AsReadOnly();
					}
					catch (Exception ex)
					{
						logger.Error(string.Format("Failed to get jobs from the database."), ex);
					}
				}
			}
			return null;
		}

		public ReadOnlyCollection<JobExecutionHistory> GetJobExecutionHistories(uint skip = 0, uint take = 1, long[] jobHistoryIds = null, Guid[] jobUniqueIds = null, long[] jobIds = null, JobStatus[] jobStatuses = null, byte[] queueIds = null, string[] typeNames = null, string[] applications = null, string[] groups = null)
		{
			using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
			{
				try
				{
					var query = context.BackgroundWorkerJobExecutionHistories.OrderByDescending(j => j.Id).AsQueryable();
					if (jobHistoryIds != null && jobHistoryIds.Length > 0)
					{
						query = query.Where(j => jobHistoryIds.Contains(j.Id));
					}
					if (jobUniqueIds != null && jobUniqueIds.Length > 0)
					{
						query = query.Where(j => jobUniqueIds.Contains(j.JobUniqueId));
					}
					if (jobIds != null && jobIds.Length > 0)
					{
						query = query.Where(j => jobIds.Contains(j.JobId));
					}
					if (jobStatuses != null && jobStatuses.Length > 0)
					{
						query = query.Where(j => jobStatuses.Contains((JobStatus)j.StatusId));
					}
					if (queueIds != null && queueIds.Length > 0)
					{
						query = query.Where(j => queueIds.Select(q => (int)q).Contains(j.QueueId));
					}
					if (typeNames != null && typeNames.Length > 0)
					{
						query = query.Where(j => typeNames.Contains(j.Type));
					}
					if (applications != null && applications.Length > 0)
					{
						query = query.Where(j => applications.Contains(j.Application));
					}
					if (groups != null && groups.Length > 0)
					{
						query = query.Where(j => groups.Contains(j.Group));
					}

					List<JobExecutionHistory> jobHistories = new List<JobExecutionHistory>();
					var storedJobHistories = query.Skip((int)skip).Take((int)take).ToList();
					foreach (var storedJobHistory in storedJobHistories)
					{
						Type jobType = Type.GetType(storedJobHistory.Type);
						if (jobType != null)
						{
							jobHistories.Add(new JobExecutionHistory
							{
								Id = storedJobHistory.Id,
								JobId = storedJobHistory.JobId,
								Success = storedJobHistory.Success ?? false,
								AbsoluteTimeout = storedJobHistory.AbsoluteTimeout,
								CreatedDate = storedJobHistory.CreatedDate,
								Instance = storedJobHistory.Instance,
								Data = storedJobHistory.Data,
								EndTime = storedJobHistory.EndDateTime,
								ErrorMessage = storedJobHistory.ErrorMessage,
								MetaData = storedJobHistory.MetaData,
								QueueId = storedJobHistory.QueueId,
								StartTime = storedJobHistory.StartDateTime,
								Status = (JobStatus)storedJobHistory.StatusId,
								JobType = jobType,
								Application = storedJobHistory.Application,
								Group = storedJobHistory.Group,
								Name = storedJobHistory.Name,
								Description = storedJobHistory.Description,
								JobUniqueId = storedJobHistory.JobUniqueId,
							});
						}
						else
						{
							storedJobHistory.StatusId = (int)JobStatus.Deleted;
							storedJobHistory.ErrorMessage = string.Format("Could not load type '{0}.'", storedJobHistory.Type);
							context.SubmitChanges();
						}
					}

					return jobHistories.AsReadOnly();
				}
				catch (Exception ex)
				{
					logger.Error(string.Format("Failed to get job histories from the database."), ex);
				}
			}
			return null;
		}

		public ReadOnlyCollection<JobData> QueueReadyJobs(byte? queueId, uint take)
		{
			List<JobData> jobData = new List<JobData>();
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable, Timeout = transactionTimeout }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					try
					{
						//In order to get a table lock, we have to resort to SQL
						string queueQueryString = string.Empty;
						if (queueId.HasValue)
						{
							queueQueryString = string.Format(" AND QueueId = {0} ", queueId.Value);
						}
						string queryString = string.Format("SELECT TOP {2} * FROM BackgroundWorkerJobs WITH (ROWLOCK, XLOCK) WHERE StatusId = {0}{1} ORDER BY [LastExecutionStartDateTime] ASC", (int)JobStatus.Ready, queueQueryString, take);
						var storedJobs = context.ExecuteQuery<BackgroundWorkerJob>(queryString).ToList();
						foreach (var storedJob in storedJobs)
						{
							storedJob.StatusId = (int)JobStatus.Queuing;
							storedJob.Instance = settings.InstanceName;
							Type jobType = Type.GetType(storedJob.Type);
							if (jobType != null)
							{
								jobData.Add(GetJobData(storedJob));
							}
							else
							{
								storedJob.StatusId = (int)JobStatus.Deleted;
								storedJob.LastErrorMessage = string.Format("Could not load type '{0}.'", storedJob.Type);
							}
						}
						context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
						ts.Complete();
					}
					catch (Exception ex)
					{
						logger.Error(string.Format("Failed to submit changes to the database."), ex);
					}
				}
			}
			return jobData.AsReadOnly();
		}

		public bool SetJobStatuses(long[] jobIds, JobStatus? oldStatus, JobStatus newStatus, string errorMessage = null, string instance = "Not Specified")
		{
			if (jobIds == null || jobIds.Length == 0) return true;

			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable, Timeout = transactionTimeout }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					try
					{
						//Unfortunately, can't use a single update statement here, as we need to know the exact records that were updated to trigger the requiresaction event.
						var storedJobs = context.ExecuteQuery<BackgroundWorkerJob>(string.Format("SELECT * FROM BackgroundWorkerJobs WITH (ROWLOCK, XLOCK) WHERE Id IN ({0})", string.Join(",", jobIds))).ToList();
						foreach (var storedJob in storedJobs)
						{
							if (oldStatus.HasValue ? storedJob.StatusId == (int)oldStatus.Value : true)
							{
								storedJob.StatusId = (int)newStatus;
								if (errorMessage != null)
								{
									storedJob.LastErrorMessage = errorMessage;
								}
								if (instance != "Not Specified")
								{
									storedJob.Instance = instance;
								}
							}
						}
						context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);

						ts.Complete();
						return true;
					}
					catch (Exception ex)
					{
						logger.Error(string.Format("Failed to submit changes to the database."), ex);
					}
				}
			}
			return false;
		}

		public void DequeueJob(JobData job)
		{
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable, Timeout = transactionTimeout }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					try
					{
						//In order to get a table lock, we have to resort to SQL
						var storedJob = context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs WITH (ROWLOCK, XLOCK) WHERE Id = {0}", job.Id).FirstOrDefault();
						if (storedJob != null && (storedJob.StatusId == (int)JobStatus.Queuing || storedJob.StatusId == (int)JobStatus.Queued))
						{
							storedJob.StatusId = (int)JobStatus.Ready;
							storedJob.Instance = null;
							context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
							ts.Complete();
							return;
						}
					}
					catch (Exception ex)
					{
						logger.Error("DequeueJob failed.", ex);
					}
				}
			}
		}

		public ReadOnlyCollection<JobData> GetScheduleReadyJobs(DateTime nextExecutionStartDateTimeAtOrBefore)
		{
			using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
			{
				try
				{
					int scheduledStatusId = (int)JobStatus.Scheduled;
					var storedJobs = context.BackgroundWorkerJobs.Where(j => j.StatusId == scheduledStatusId && j.NextExecutionStartDateTime <= nextExecutionStartDateTimeAtOrBefore).OrderBy(j => j.LastExecutionStartDateTime);
					return storedJobs.Select(j => GetJobData(j)).ToList().AsReadOnly();
				}
				catch (Exception ex)
				{
					logger.Error("GetScheduleReadyJobs failed.", ex);
				}
			}
			return null;
		}

		public ReadOnlyCollection<Alert> GetAlerts(uint skip = 0, uint take = 1)
		{
			using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
			{
				try
				{
					var storedAlerts = context.BackgroundWorkerAlerts.OrderBy(a => a.Id).Skip((int)skip).Take((int)take);
					return storedAlerts.Select(a => 
						new Alert
						{
							Id = a.Id,
							JobId = a.JobId,
							JobHistoryId = a.JobHistoryId,
							Message = a.Message,
						}).ToList().AsReadOnly();
				}
				catch (Exception ex)
				{
					logger.Error("GetAlerts failed.", ex);
				}
			}
			return null;
		}

		public Alert CreateAlert(long jobId, long? jobHistoryId, string message)
		{
			using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
			{
				try
				{
					BackgroundWorkerAlert alert = new BackgroundWorkerAlert { JobId = jobId, JobHistoryId = jobHistoryId, Message = message };
					context.BackgroundWorkerAlerts.InsertOnSubmit(alert);
					context.SubmitChanges();
					return new Alert
					{
						Id = alert.Id,
						JobId = alert.JobId,
						JobHistoryId = alert.JobHistoryId,
						Message = alert.Message,
					};
				}
				catch (Exception ex)
				{
					logger.Error("CreateAlert failed.", ex);
				}
			}
			return null;
		}

		public bool DeleteAlerts(long[] ids = null)
		{
			using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
			{
				try
				{
					var storedAlertsQuery = context.BackgroundWorkerAlerts.AsQueryable();
					if (ids != null)
					{
						storedAlertsQuery = storedAlertsQuery.Where(a => ids.Contains(a.Id));
					}
					var storedAlerts = storedAlertsQuery.ToList();
					if (storedAlerts != null && storedAlerts.Count > 0)
					{
						context.BackgroundWorkerAlerts.DeleteAllOnSubmit(storedAlerts);
						context.SubmitChanges(ConflictMode.FailOnFirstConflict);
					}
					return true;
				}
				catch (Exception ex)
				{
					logger.Error("DeleteAlerts failed.", ex);
				}
			}
			return false;
		}

		private JobData GetJobData(BackgroundWorkerJob job)
		{
			Type jobType = Type.GetType(job.Type);
			var jobData = new JobData
			{
				Id = job.Id,
				UniqueId = job.UniqueId,
				Instance = job.Instance,
				AbsoluteTimeout = job.AbsoluteTimeout,
				CreatedDate = job.CreatedDate,
				Data = job.Data,
				MetaData = job.MetaData,
				QueueId = job.QueueId,
				Status = (JobStatus)job.StatusId,
				LastEndTime = job.LastExecutionEndDateTime,
				LastStartTime = job.LastExecutionStartDateTime,
				NextStartTime = job.NextExecutionStartDateTime,
				LastErrorMessage = job.LastErrorMessage,
				JobType = jobType,
				Application = job.Application,
				Group = job.Group,
				Name = job.Name,
				Description = job.Description,
				SuppressHistory = job.SuppressHistory,
				DeleteWhenDone = job.DeleteWhenDone,
			};
			if (!string.IsNullOrEmpty(job.ScheduleType))
			{
				jobData.Schedule = (ISchedule)Utils.DeserializeObject(job.Schedule, Type.GetType(job.ScheduleType));
			}
			return jobData;
		}
	}
}
