using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "JobExecutionHistory", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class JobExecutionHistory
	{
		internal JobExecutionHistory(Logic.DataModel.Jobs.JobExecutionHistory jobExecutionHistory)
		{
			this.AbsoluteTimeout = jobExecutionHistory.AbsoluteTimeout;
			this.JobType = jobExecutionHistory.JobType.AssemblyQualifiedName;
			this.JobId = jobExecutionHistory.JobId;
			this.Success = jobExecutionHistory.Success;
			this.CreatedDate = jobExecutionHistory.CreatedDate;
			this.Data = jobExecutionHistory.Data;
			this.EndTime = jobExecutionHistory.EndTime;
			this.ErrorMessage = jobExecutionHistory.ErrorMessage;
			this.Id = jobExecutionHistory.Id;
			this.Instance = jobExecutionHistory.Instance;
			this.QueueId = jobExecutionHistory.QueueId;
			this.MetaData = jobExecutionHistory.MetaData;
			this.StartTime = jobExecutionHistory.StartTime;
			this.Status = (JobStatus)(int)jobExecutionHistory.Status;
			this.Application = jobExecutionHistory.Application;
			this.Group = jobExecutionHistory.Group;
			this.Name = jobExecutionHistory.Name;
			this.Description = jobExecutionHistory.Description;
		}

		internal Logic.DataModel.Jobs.JobExecutionHistory AsInternalJobExecutionHistory()
		{
			Type jobType = Type.GetType(this.JobType);
			if (jobType == null) throw new ArgumentException(string.Format("JobType '{0}' could not be resolved", this.JobType));

			return new Logic.DataModel.Jobs.JobExecutionHistory
			{
				Id = this.Id,
				JobId = this.JobId,
				Success = this.Success,
				AbsoluteTimeout = this.AbsoluteTimeout,
				JobType = jobType,
				CreatedDate = this.CreatedDate,
				Data = this.Data,
				EndTime = this.EndTime,
				ErrorMessage = this.ErrorMessage,
				StartTime = this.StartTime,
				Instance = this.Instance,
				Application = this.Application,
				Group = this.Group,
				MetaData = this.MetaData,
				QueueId = this.QueueId,
				Name = this.Name,
				Description = this.Description,
				Status = (Logic.DataModel.Jobs.JobStatus)(int)this.Status,
			};
		}

		[DataMember(Name = "Id", IsRequired = true)]
		public long Id { get; set; }

		[DataMember(Name = "JobId", IsRequired = true)]
		public long JobId { get; set; }

		[DataMember(Name = "Data", IsRequired = true)]
		public string Data { get; set; }

		[DataMember(Name = "MetaData", IsRequired = true)]
		public string MetaData { get; set; }

		[DataMember(Name = "ErrorMessage", IsRequired = true)]
		public string ErrorMessage { get; set; }

		[DataMember(Name = "Instance", IsRequired = true)]
		public string Instance { get; set; }

		[DataMember(Name = "CreatedDate", IsRequired = true)]
		public DateTime CreatedDate { get; set; }

		[DataMember(Name = "StartTime", IsRequired = true)]
		public DateTime? StartTime { get; set; }

		[DataMember(Name = "EndTime", IsRequired = true)]
		public DateTime? EndTime { get; set; }

		[DataMember(Name = "AbsoluteTimeout", IsRequired = true)]
		public TimeSpan? AbsoluteTimeout { get; set; }

		[DataMember(Name = "Status", IsRequired = true)]
		public JobStatus Status { get; set; }

		[DataMember(Name = "QueueId", IsRequired = true)]
		public int QueueId { get; set; }

		[DataMember(Name = "Success", IsRequired = true)]
		public bool Success { get; set; }

		[DataMember(Name = "JobType", IsRequired = true)]
		public string JobType { get; internal set; }

		[DataMember(Name = "Application", IsRequired = true)]
		public string Application { get; set; }

		[DataMember(Name = "Group", IsRequired = true)]
		public string Group { get; set; }

		[DataMember(Name = "Name", IsRequired = true)]
		public string Name { get; set; }

		[DataMember(Name = "Description", IsRequired = true)]
		public string Description { get; set; }
	}
}
