using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BackgroundWorkerService.Service.Admin.DataModel;
using BackgroundWorkerService.Service.Admin.DataModel.Request;
using BackgroundWorkerService.Service.Admin.DataModel.Response;

namespace BackgroundWorkerService.Service.Admin
{
	[ServiceContract(Namespace = "http://backgroundworkerservice")]
	public interface IAccessPoint
	{
		[OperationContract(Name = "GetServiceSettings", ReplyAction = "GetServiceSettings")]
		GetServiceSettingsResponse GetServiceSettings(GetServiceSettingsRequest request);

		[OperationContract(Name = "GetServiceStatus", ReplyAction = "GetServiceStatus")]
		GetServiceStatusResponse GetServiceStatus(GetServiceStatusRequest request);

		[OperationContract(Name = "GetJobs", ReplyAction = "GetJobs")]
		GetJobsResponse GetJobs(GetJobsRequest request);

		[OperationContract(Name = "SetJobStatus", ReplyAction = "SetJobStatus")]
		SetJobStatusResponse SetJobStatus(SetJobStatusRequest request);

		[OperationContract(Name = "UpdateJob", ReplyAction = "UpdateJob")]
		UpdateJobResponse UpdateJob(UpdateJobRequest request);

		[OperationContract(Name = "DeleteJob", ReplyAction = "DeleteJob")]
		DeleteJobResponse DeleteJob(DeleteJobRequest request);

		[OperationContract(Name = "GetJobExecutionHistories", ReplyAction = "GetJobExecutionHistories")]
		GetJobExecutionHistoriesResponse GetJobExecutionHistories(GetJobExecutionHistoriesRequest request);

		[OperationContract(Name = "CreateJob", ReplyAction = "CreateJob")]
		CreateJobResponse CreateJob(CreateJobRequest request);

		[OperationContract(Name = "ScheduleJob", ReplyAction = "ScheduleJob")]
		ScheduleJobResponse ScheduleJob(ScheduleJobRequest request);

		[OperationContract(Name = "RunScheduledJob", ReplyAction = "RunScheduledJob")]
		RunScheduledJobResponse RunScheduledJob(RunScheduledJobRequest request);
	}
}
