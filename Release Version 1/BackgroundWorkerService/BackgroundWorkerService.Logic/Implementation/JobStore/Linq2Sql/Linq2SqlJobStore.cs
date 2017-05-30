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

namespace BackgroundWorkerService.Logic.Implementation.JobStore.Linq2Sql
{
	/// <summary>
	/// A concrete implementation of <see cref="IJobStore"/> that uses Linq2Sql to store jobs in a SQL Server database.
	/// </summary>
	public class Linq2SqlJobStore : IJobStore
	{
		private ISettingsProvider settings;
		private ILoggingProvider logger;
		private Random randomizer;
		private List<JobData> jobsRequiringAction = new List<JobData>();
		private List<JobData> deletedJobs = new List<JobData>();
		private BackgroundWorker eventFireWorker = new BackgroundWorker();
		private string connectionString;

		/// <summary>
		/// Initializes a new instance of the <see cref="Linq2SqlJobStore"/> class.
		/// </summary>
		public Linq2SqlJobStore()
			: this(ConfigurationSettings.SettingsProvider, ConfigurationSettings.LoggingProvider, Utils.GetConfigurationSection<Linq2SqlJobStoreConfigurationSection>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Linq2SqlJobStore"/> class.
		/// </summary>
		/// <param name="settingsProvider">The settings provider.</param>
		/// <param name="loggingProvider">The logging provider.</param>
		/// <param name="jobStoreSettingsProvider">The job store settings provider.</param>
		public Linq2SqlJobStore(ISettingsProvider settingsProvider, ILoggingProvider loggingProvider, ILinq2SqlJobStoreSettingsProvider jobStoreSettingsProvider)
		{
			connectionString = jobStoreSettingsProvider.ConnectionString;

			randomizer = new Random((int)DateTime.Now.TimeOfDay.TotalSeconds);
			logger = loggingProvider;
			settings = settingsProvider;

			eventFireWorker.DoWork += new DoWorkEventHandler(eventFireWorker_DoWork);
			eventFireWorker.RunWorkerAsync();
		}

		/// <summary>
		/// When jobs are updated or created, we don't want the events to be raised on the same thread as this can prolong the time that tables and rows are locked.
		/// Instead, the events are raised on a seperate background thread.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
		void eventFireWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				var jobActionRequired = JobActionRequired;
				if (jobsRequiringAction.Count > 0 && jobActionRequired != null)
				{
					jobActionRequired(this, new JobActionRequiredEventArgs(jobsRequiringAction[0]));
					jobsRequiringAction.RemoveAt(0);
				}

				var jobDeleted = JobDeleted;
				if (deletedJobs.Count > 0 && jobDeleted != null)
				{
					jobDeleted(this, new JobActionRequiredEventArgs(deletedJobs[0]));
					deletedJobs.RemoveAt(0);
				}

				System.Threading.Thread.Sleep(100);
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

			BackgroundWorkerJob newJob = new BackgroundWorkerJob
			{
				AbsoluteTimeout = absoluteTimeout,
				CreatedDate = DateTime.Now,
				Data = data,
				MetaData = metaData,
				QueueId = queueId,
				StatusId = (int?)jobStatus ?? (int)JobStatus.Ready,
				Type = jobType.AssemblyQualifiedName,
				Instance = null, //This is just a default to show that no instance has picked this up
				Application = application,
				Group = group,
				Name = name,
				Description = description,
				SuppressHistory = suppressHistory,
				DeleteWhenDone = deleteWhenDone,
			};
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					//In order to get a table lock, we have to resort to SQL
					context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs").FirstOrDefault();
					context.BackgroundWorkerJobs.InsertOnSubmit(newJob);
					try
					{
						context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
						JobData job = GetJobData(newJob);
						jobsRequiringAction.Add(job);
						return job;
					}
					catch (Exception ex)
					{
						logger.LogException(string.Format("Failed to create '{0}' with data '{1}'", jobType.AssemblyQualifiedName, data), ex);
					}
					finally
					{
						ts.Complete();
					}
				}
			}
			return null;
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

			BackgroundWorkerJob newJob = new BackgroundWorkerJob
			{
				AbsoluteTimeout = absoluteTimeout,
				CreatedDate = DateTime.Now,
				Data = data,
				MetaData = metaData,
				QueueId = queueId,
				StatusId = (int?)jobStatus ?? (int)JobStatus.Ready,
				Type = jobType.AssemblyQualifiedName,
				ScheduleType = schedule.GetType().AssemblyQualifiedName,
				Schedule = Utils.SerializeObject(schedule, schedule.GetType()),
				Instance = null, //This is just a default to show that no instance has picked this up
				Application = application,
				Group = group,
				Name = name,
				Description = description,
				SuppressHistory = suppressHistory,
				DeleteWhenDone = deleteWhenDone,
			};
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					//In order to get a table lock, we have to resort to SQL
					context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs").FirstOrDefault();
					context.BackgroundWorkerJobs.InsertOnSubmit(newJob);
					try
					{
						context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
						JobData job = GetJobData(newJob);
						jobsRequiringAction.Add(job);
						return job;
					}
					catch (Exception ex)
					{
						logger.LogException(string.Format("Failed to create '{0}' with data '{1}'", jobType.AssemblyQualifiedName, data), ex);
					}
					finally
					{
						ts.Complete();
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Deletes the job permanently from the job store.
		/// </summary>
		/// <param name="jobId">The job id.</param>
		/// <param name="deleteHistory">If set to true, the history will also be deleted.</param>
		/// <returns></returns>
		public bool DeleteJob(long jobId, bool deleteHistory = false)
		{
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					//In order to get a table lock, we have to resort to SQL
					context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs").FirstOrDefault();
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
							return true;
						}
						catch (Exception ex)
						{
							logger.LogException(string.Format("Failed to delete job '{0}'.", jobId), ex);
						}
						finally
						{
							ts.Complete();
							deletedJobs.Add(GetJobData(job));
						}
					}
				}
			}
			return false;
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
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					//In order to get a table lock, we have to resort to SQL
					var storedJob = context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs WITH (TABLOCKX) WHERE Id = {0}", job.Id).FirstOrDefault();
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
						if (job.Schedule != null)
						{
							storedJob.ScheduleType = job.Schedule.GetType().AssemblyQualifiedName;
							storedJob.Schedule = Utils.SerializeObject(job.Schedule, job.Schedule.GetType());
						}
						try
						{
							context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
							if (triggerJobActionRequired)
							{
								jobsRequiringAction.Add(GetJobData(storedJob));
							}
							return true;
						}
						catch (Exception ex)
						{
							logger.LogException(string.Format("Failed to submit changes to the database."), ex);
						}
						finally
						{
							ts.Complete();
						} 
						return false;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Creates a job execution history record from the job data.
		/// </summary>
		/// <param name="job">The job data.</param>
		/// <returns></returns>
		public bool CreateJobExecutionHistory(JobData job)
		{
			BackgroundWorkerJobExecutionHistory history = new BackgroundWorkerJobExecutionHistory
			{
				AbsoluteTimeout = job.AbsoluteTimeout,
				CreatedDate = job.CreatedDate,
				Instance = job.Instance,
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
					return true;
				}
				catch (Exception ex)
				{
					logger.LogException(string.Format("Failed to submit changes to the database."), ex);
				}
			}
			return false;
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
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					try
					{
						//In order to get a table lock, we have to resort to SQL
						context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs WITH (TABLOCKX)");
						var query = context.BackgroundWorkerJobs.OrderBy(j => j.CreatedDate).AsQueryable();
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

						return jobs.AsReadOnly();
					}
					catch (Exception ex)
					{
						logger.LogException(string.Format("Failed to get jobs from the database."), ex);
					}
					finally
					{
						ts.Complete();
					}
				}
			}
			return null;
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
			using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
			{
				try
				{
					var query = context.BackgroundWorkerJobExecutionHistories.OrderByDescending(j => j.Id).AsQueryable();
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
							});
						}
						else
						{
							storedJobHistory.StatusId = (int)JobStatus.Deleted;
							storedJobHistory.ErrorMessage = string.Format("Could not load type '{0}.'", storedJobHistory.Type);
						}
					}

					context.SubmitChanges();

					return jobHistories.AsReadOnly();
				}
				catch (Exception ex)
				{
					logger.LogException(string.Format("Failed to get job histories from the database."), ex);
				}
			}
			return null;
		}


		/// <summary>
		/// Queues the next non scheduled job that is in ready state in the job store.
		/// </summary>
		/// <param name="queueId">The queue id to find the next job for.</param>
		/// <returns></returns>
		public JobData QueueNextNonScheduledReadyJob(int? queueId)
		{
			try
			{
				JobData jobData;
				bool tryQueueNextJobSuccess = QueueNextNonScheduledReadyJob(queueId, out jobData);
				while (!tryQueueNextJobSuccess)
				{
					tryQueueNextJobSuccess = QueueNextNonScheduledReadyJob(queueId, out jobData);
				}
				return jobData;
			}
			catch (Exception ex)
			{
				logger.LogException("QueueNextNonScheduledReadyJob failed.", ex);
			}
			return null;
		}

		/// <summary>
		/// Queues the next scheduled job that is in ready state in the job store.
		/// </summary>
		/// <returns></returns>
		public JobData QueueNextScheduledReadyJob()
		{
			try
			{
				JobData jobData;
				bool tryScheduleNextJobSuccess = QueueNextScheduledReadyJob(out jobData);
				while (!tryScheduleNextJobSuccess)
				{
					tryScheduleNextJobSuccess = QueueNextScheduledReadyJob(out jobData);
				}
				return jobData;
			}
			catch (Exception ex)
			{
				logger.LogException("QueueNextScheduledReadyJob failed.", ex);
			}
			return null;
		}

		/// <summary>
		/// Sets job in status <see cref="JobStatus.Queuing"/> or <see cref="JobStatus.Queued"/> to status <see cref="JobStatus.Ready"/>.
		/// </summary>
		/// <param name="job">The job to dequeue.</param>
		public void DequeueJob(JobData job)
		{
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					try
					{
						//In order to get a table lock, we have to resort to SQL
						var storedJob = context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs WITH (TABLOCKX) WHERE Id = {0}", job.Id).FirstOrDefault();
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
						logger.LogException("DequeueJob failed.", ex);
					}
				}
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
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					try
					{
						//In order to get a table lock, we have to resort to SQL
						var storedJob = context.ExecuteQuery<BackgroundWorkerJob>("SELECT TOP 1 * FROM BackgroundWorkerJobs WITH (TABLOCKX) WHERE Id = {0}", jobId).FirstOrDefault();
						if (storedJob != null && (oldStatus.HasValue ? storedJob.StatusId == (int)oldStatus.Value : true))
						{
							storedJob.StatusId = (int)newStatus;
							if (errorMessage != null)
							{
								storedJob.LastErrorMessage = errorMessage;
							}
							if (metaData != null)
							{
								storedJob.MetaData = metaData;
							}

							context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
							jobsRequiringAction.Add(GetJobData(storedJob));
							return true;
						}
					}
					catch (Exception ex)
					{
						logger.LogException(string.Format("Failed to submit changes to the database."), ex);
					}
					finally
					{
						ts.Complete();
					}
				}
			}
			return false;
		}

		private bool QueueNextNonScheduledReadyJob(int? queueId, out JobData jobData)
		{
			jobData = null;
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
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
						string queryString = string.Format("SELECT TOP 1 * FROM BackgroundWorkerJobs WITH (TABLOCKX) WHERE (ScheduleType IS NULL OR ScheduleType = '') AND StatusId = {0}{1} ORDER BY [CreatedDate] Asc", "{0}", queueQueryString);
						var storedJob = context.ExecuteQuery<BackgroundWorkerJob>(queryString, (int)JobStatus.Ready).FirstOrDefault();
						if (storedJob != null && storedJob.StatusId == (int)JobStatus.Ready)
						{
							Debug.WriteLine(string.Format("L2SQL : Queueing job {0}", storedJob.Id));
							storedJob.StatusId = (int)JobStatus.Queuing;
							storedJob.Instance = settings.InstanceName;
							context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
							Type jobType = Type.GetType(storedJob.Type);
							if (jobType != null)
							{
								Debug.WriteLine(string.Format("L2SQL : Queued job {0}", storedJob.Id));
								jobData = GetJobData(storedJob);
							}
							else
							{
								storedJob.StatusId = (int)JobStatus.Deleted;
								storedJob.LastErrorMessage = string.Format("Could not load type '{0}.'", storedJob.Type);
								context.SubmitChanges();
							}
							if (jobType == null)
							{
								return false;
							}
						}
					}
					finally
					{
						ts.Complete();
					}
				}
			}
			return true;
		}

		private bool QueueNextScheduledReadyJob(out JobData jobData)
		{
			jobData = null;
			using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				using (Linq2SqlJobStoreDalDataContext context = new Linq2SqlJobStoreDalDataContext(connectionString))
				{
					try
					{
						//In order to get a table lock, we have to resort to SQL
						string queryString = string.Format("SELECT TOP 1 * FROM BackgroundWorkerJobs WITH (TABLOCKX) WHERE (ScheduleType IS NOT NULL AND ScheduleType <> '') AND StatusId = {0} ORDER BY [CreatedDate] Asc", "{0}");
						var storedJob = context.ExecuteQuery<BackgroundWorkerJob>(queryString, (int)JobStatus.Ready).FirstOrDefault();
						if (storedJob != null && storedJob.StatusId == (int)JobStatus.Ready)
						{
							Debug.WriteLine(string.Format("L2SQL : Queueing job {0}", storedJob.Id));
							storedJob.StatusId = (int)JobStatus.Queuing;
							storedJob.Instance = settings.InstanceName;
							context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
							Type jobType = Type.GetType(storedJob.Type);
							if (jobType != null)
							{
								Debug.WriteLine(string.Format("L2SQL : Queued job {0}", storedJob.Id));
								jobData = GetJobData(storedJob);
							}
							else
							{
								storedJob.StatusId = (int)JobStatus.Deleted;
								storedJob.LastErrorMessage = string.Format("Could not load type '{0}.'", storedJob.Type);
								context.SubmitChanges();
							}
							if (jobType == null)
							{
								return false;
							}
						}
					}
					finally
					{
						ts.Complete();
					}
				}
			}
			return true;
		}

		private JobData GetJobData(BackgroundWorkerJob job)
		{
			Type jobType = Type.GetType(job.Type);
			var jobData = new JobData
			{
				Id = job.Id,
				Instance = job.Instance,
				AbsoluteTimeout = job.AbsoluteTimeout,
				CreatedDate = job.CreatedDate,
				Data = job.Data,
				MetaData = job.MetaData,
				QueueId = job.QueueId,
				Status = (JobStatus)job.StatusId,
				LastEndTime = job.LastExecutionEndDateTime,
				LastStartTime = job.LastExecutionStartDateTime,
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
