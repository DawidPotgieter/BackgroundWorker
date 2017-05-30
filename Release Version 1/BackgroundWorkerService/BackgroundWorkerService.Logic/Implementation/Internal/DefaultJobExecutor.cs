using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces.Internal;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using System.Diagnostics;
using BackgroundWorkerService.Logic.DataModel.Internal.Jobs;
using System.Threading;

namespace BackgroundWorkerService.Logic.Implementation.Internal
{
	/// <summary>
	/// This is the default job executor.  It is used to manage the execution of a single instantiated job.
	/// </summary>
	internal class DefaultJobExecutor : IJobExecutor
	{
		/// <summary>
		/// Executes the job specified in the job context and updates the jobstore accordingly.
		/// </summary>
		/// <param name="jobContext">The job context.</param>
		public void ExecuteJob(JobContext jobContext)
		{
			if (jobContext == null)
			{
				throw new ArgumentNullException("jobContext");
			}
			if (jobContext.JobInstance == null)
			{
				throw new ArgumentNullException("jobContext.JobInstance");
			}
			if (jobContext.JobData == null)
			{
				throw new ArgumentNullException("jobContext.JobData");
			}
			if (jobContext.JobManager == null)
			{
				throw new ArgumentNullException("jobContext.JobManager");
			}
			if (!(jobContext.JobInstance is IJob))
			{
				throw new ArgumentException("BackgroundJobExecutor : jobContext.JobInstance must be of type IJob");
			}

			IJob backgroundJob = (IJob)jobContext.JobInstance;
			IJobManager jobManager = jobContext.JobManager;
			JobData jobData = jobContext.JobData;
			ILoggingProvider logger = jobContext.JobManager.Logger;

			Debug.WriteLine(DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss:fffffff") + " : " + jobContext.JobData.Id + " start execution.");

			try
			{
				jobData.LastStartTime = DateTime.Now;
				jobData.LastEndTime = null;
				jobData.LastErrorMessage = null;
				jobData.Status = JobStatus.Executing;
				jobManager.JobStore.UpdateJob(jobData);
				JobExecutionResult returnValue = backgroundJob.Execute(jobContext.JobData.Data, jobContext.JobData.MetaData);
				if (returnValue == null)
				{
					throw new ArgumentException("Execute(string jobData, string recoveryData) for job '" + jobContext.JobData.Id + "' returned null.");
				}
				jobData.LastEndTime = DateTime.Now;
				switch (returnValue.ResultStatus)
				{
					case JobResultStatus.Success:
						jobData.Status = JobStatus.Done;
						break;
					case JobResultStatus.FailAutoRetry:
						jobData.Status = JobStatus.FailAutoRetry;
						break;
					case JobResultStatus.FailRetry:
						jobData.Status = JobStatus.FailRetry;
						break;
					default:
						jobData.Status = JobStatus.Fail;
						break;
				}
				jobData.LastErrorMessage = (!string.IsNullOrEmpty(returnValue.ErrorMessage) ? returnValue.ErrorMessage : null);
				if (returnValue.MetaData != null && jobData.MetaData != returnValue.MetaData)
				{
					jobData.MetaData = returnValue.MetaData;
				}

				jobManager.JobStore.UpdateJob(jobData);
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception ex)
			{
				string message = Helpers.Utils.GetExceptionMessage(ex);
				jobData.Status = JobStatus.Fail;
				jobData.LastErrorMessage = message;
				jobData.LastEndTime = DateTime.Now;
				jobManager.JobStore.UpdateJob(jobData);
			}

			try
			{
				if (!jobData.SuppressHistory && jobData.Status != JobStatus.Ready) //Status check added for the extreme case where the thread was aborted
				{
					jobManager.JobStore.CreateJobExecutionHistory(jobData);
				}
			}
			catch (Exception ex)
			{
				logger.LogException("Could not create job execution history record.", ex);
			}

			Debug.WriteLine(DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss:fffffff") + " : " + jobContext.JobData.Id + " end execution.");

			if (jobData.DeleteWhenDone && jobData.Status == JobStatus.Done)
			{
				try
				{
					jobManager.JobStore.DeleteJob(jobData);
					return;
				}
				catch (Exception ex)
				{
					logger.LogException("Could not delete job that was set to 'DeleteWhenDone'.", ex);
				}
			}

			if (jobData.Status == JobStatus.FailAutoRetry)
			{
				try
				{
					jobManager.JobStore.SetJobStatus(jobData.Id, jobData.Status, JobStatus.Ready);
				}
				catch (Exception ex)
				{
					logger.LogException("Could not set job to ready after FailAutoRetry result.", ex);
				}
			}

			if (jobData.Schedule != null)
			{
				try
				{
					jobManager.JobStore.SetJobStatus(jobData.Id, jobData.Status, JobStatus.Ready);
				}
				catch (Exception ex)
				{
					logger.LogException("Could not set job to ready for next scheduled execution.", ex);
				}
			}
		}
	}
}
