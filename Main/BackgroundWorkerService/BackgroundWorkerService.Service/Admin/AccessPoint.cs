using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BackgroundWorkerService.Logic;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.Implementation;
using BackgroundWorkerService.Service.Admin.DataModel;
using BackgroundWorkerService.Service.Admin.DataModel.Response;
using BackgroundWorkerService.Service.Admin.DataModel.Request;

namespace BackgroundWorkerService.Service.Admin
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class AccessPoint : IAccessPoint
	{
		private IJobManager jobManager;
		internal string Status = string.Empty;
		internal BackgroundWorkerService.Service.Service serviceInstance;

		internal AccessPoint(BackgroundWorkerService.Service.Service serviceInstance)
		{
			jobManager = serviceInstance.WorkerService.JobManager;
			this.serviceInstance = serviceInstance;
			this.serviceInstance.WorkerService.Notify += new EventHandler<NotificationEventArgs>(WorkerService_Notify);
		}

		void WorkerService_Notify(object sender, NotificationEventArgs e)
		{
			Status = e.Message;
		}

		public GetServiceSettingsResponse GetServiceSettings(GetServiceSettingsRequest request)
		{
			return new GetServiceSettingsResponse
			{
				Settings = new Settings(serviceInstance.SettingsProvider),
			};
		}

		public GetServiceStatusResponse GetServiceStatus(GetServiceStatusRequest request)
		{
			ServiceHost host = OperationContext.Current.Host as ServiceHost;
			return new GetServiceStatusResponse
			{
				ServiceStatus = new ServiceStatus(host),
			};
		}

		public GetJobsResponse GetJobs(GetJobsRequest request)
		{
			return new GetJobsResponse
			{
				Jobs = jobManager.JobStore.GetJobs(
					request.Skip,
					request.Take,
					request.UniqueIds != null ? request.UniqueIds.ToArray() : null,
					request.JobIds != null ? request.JobIds.ToArray() : null,
					request.JobStatuses != null ? request.JobStatuses.Select(s => (Logic.DataModel.Jobs.JobStatus)(int)s).ToArray() : null,
					request.QueueIds != null ? request.QueueIds.ToArray() : null,
					request.TypeNames != null ? request.TypeNames.ToArray() : null,
					request.Applications != null ? request.Applications.ToArray() : null,
					request.Groups != null ? request.Groups.ToArray() : null).Select(j => new JobData(j)).ToList(),
			};
		}

		public SetJobStatusesResponse SetJobStatuses(SetJobStatusesRequest request)
		{
			return new SetJobStatusesResponse
			{
				Success = jobManager.JobStore.SetJobStatuses(
					request.JobIds,
					(Logic.DataModel.Jobs.JobStatus?)(int?)request.OldStatus,
					(Logic.DataModel.Jobs.JobStatus)(int)request.NewStatus,
					request.ErrorMessage),
			};
		}

		public UpdateJobResponse UpdateJob(UpdateJobRequest request)
		{
			return new UpdateJobResponse
			{
				Success = jobManager.JobStore.UpdateJob(request.JobData.AsInternalJobData()),
			};
		}

		public DeleteJobResponse DeleteJob(DeleteJobRequest request)
		{
			return new DeleteJobResponse
			{
				Success = jobManager.JobStore.DeleteJob(request.JobId, request.DeleteHistory),
			};
		}

		public GetJobExecutionHistoriesResponse GetJobExecutionHistories(GetJobExecutionHistoriesRequest request)
		{
			return new GetJobExecutionHistoriesResponse
			{
				JobHistories = jobManager.JobStore.GetJobExecutionHistories(
					request.Skip,
					request.Take,
					request.Ids != null ? request.Ids.ToArray() : null,
					request.JobUniqueIds != null ? request.JobUniqueIds.ToArray() : null,
					request.JobIds != null ? request.JobIds.ToArray() : null,
					request.JobStatuses != null ? request.JobStatuses.Select(s => (Logic.DataModel.Jobs.JobStatus)(int)s).ToArray() : null,
					request.QueueIds != null ? request.QueueIds.ToArray() : null,
					request.TypeNames != null ? request.TypeNames.ToArray() : null,
					request.Applications != null ? request.Applications.ToArray() : null,
					request.Groups != null ? request.Groups.ToArray() : null).Select(j => new JobExecutionHistory(j)).ToList(),
			};
		}

		public CreateJobResponse CreateJob(CreateJobRequest request)
		{
			Type jobType = Type.GetType(request.Type);
			if (jobType == null) throw new ArgumentException(string.Format("JobType '{0}' could not be resolved", request.Type));
			return new CreateJobResponse
			{
				Job = new JobData(
					jobManager.JobStore.CreateJob(
						jobType,
						request.Data,
						request.MetaData,
						request.QueueId,
						request.CalendarSchedule != null ? request.CalendarSchedule.AsInternalSchedule() : null,
						request.UniqueId,
						request.Name,
						request.Description,
						request.Application,
						request.Group,
						request.AbsoluteTimeout,
						(Logic.DataModel.Jobs.JobStatus?)(int?)request.Status,
						request.CreatedDate,
						request.SuppressHistory,
						request.DeleteWhenDone)),
			};
		}


		public GetAlertsResponse GetAlerts(GetAlertsRequest request)
		{
			var alerts = jobManager.JobStore.GetAlerts(request.Skip, request.Take).Select(a => new Alert(a)).ToList();
			if (alerts == null) alerts = new List<Alert>();
			return new GetAlertsResponse
			{
				Alerts = alerts,
			};
		}

		public DeleteAlertsResponse DeleteAlerts(DeleteAlertsRequest request)
		{
			return new DeleteAlertsResponse
			{
				Success = jobManager.JobStore.DeleteAlerts(request.Ids != null ? request.Ids.ToArray() : null),
			};
		}
	}
}
