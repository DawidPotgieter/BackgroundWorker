using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "JobData", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class JobData
	{
		internal JobData(Logic.DataModel.Jobs.JobData jobData)
		{
			this.AbsoluteTimeout = jobData.AbsoluteTimeout;
			this.JobType = jobData.JobType.AssemblyQualifiedName;
			this.CreatedDate = jobData.CreatedDate;
			this.Data = jobData.Data;
			this.LastEndTime = jobData.LastEndTime;
			this.LastErrorMessage = jobData.LastErrorMessage;
			this.Id = jobData.Id;
			this.Instance = jobData.Instance;
			this.QueueId = jobData.QueueId;
			this.MetaData = jobData.MetaData;
			this.LastStartTime = jobData.LastStartTime;
			this.Status = (JobStatus)(int)jobData.Status;
			this.Application = jobData.Application;
			this.Group = jobData.Group;
			this.Name = jobData.Name;
			this.Description = jobData.Description;
			this.DeleteWhenDone = jobData.DeleteWhenDone;
			this.SuppressHistory = jobData.SuppressHistory;
			if (jobData.Schedule != null)
			{
				if (jobData.Schedule.GetType() == typeof(Logic.DataModel.Scheduling.CalendarSchedule))
				{
					this.CalendarSchedule = new CalendarSchedule((Logic.DataModel.Scheduling.CalendarSchedule)jobData.Schedule);
				}
				else
				{
					throw new ArgumentException("Can only handle CalendarSchedule through the webservice interface at the moment.");
				}
			}
		}

		internal Logic.DataModel.Jobs.JobData AsInternalJobData()
		{
			Type jobType = Type.GetType(this.JobType);
			if (jobType == null) throw new ArgumentException(string.Format("JobType '{0}' could not be resolved", this.JobType));

			return new Logic.DataModel.Jobs.JobData
			{
				Id = this.Id,
				AbsoluteTimeout = this.AbsoluteTimeout,
				JobType = jobType,
				CreatedDate = this.CreatedDate,
				Data = this.Data,
				LastEndTime = this.LastEndTime,
				LastErrorMessage = this.LastErrorMessage,
				LastStartTime = this.LastStartTime,
				Instance = this.Instance,
				Application = this.Application,
				Group = this.Group,
				Name = this.Name,
				Description = this.Description,
				MetaData = this.MetaData,
				QueueId = this.QueueId,
				Status = (Logic.DataModel.Jobs.JobStatus)(int)this.Status,
				DeleteWhenDone = this.DeleteWhenDone,
				SuppressHistory = this.SuppressHistory,
				Schedule = this.CalendarSchedule != null ? this.CalendarSchedule.AsInternalSchedule() : null,
			};
		}

		/// <summary>
		/// Gets the auto generated job id.
		/// </summary>
		/// <value>The auto generated job id.</value>
		[DataMember(Name = "Id", IsRequired = true)]
		public long Id { get; set; }
		/// <summary>
		/// Gets the job data.
		/// </summary>
		/// <value>The job data.</value>
		[DataMember(Name = "Data", IsRequired = true)]
		public string Data { get; set; }
		/// <summary>
		/// Gets the job meta data.
		/// </summary>
		/// <value>The job meta data.</value>
		[DataMember(Name = "MetaData", IsRequired = true)]
		public string MetaData { get; set; }
		/// <summary>
		/// Gets the error message (if any) associated with this job.
		/// </summary>
		/// <value>ErrorMessage will be set if the job did not return a success result.</value>
		[DataMember(Name = "LastErrorMessage", IsRequired = true)]
		public string LastErrorMessage { get; set; }
		/// <summary>
		/// Gets the name of the TimerJob Service instance that has picked up this job.
		/// </summary>
		[DataMember(Name = "Instance", IsRequired = true)]
		public string Instance { get; set; }
		/// <summary>
		/// Gets the DateTime when this job was created.
		/// </summary>
		[DataMember(Name = "CreatedDate", IsRequired = true)]
		public DateTime CreatedDate { get; set; }
		/// <summary>
		/// Gets the DateTime when this job started executing.
		/// </summary>
		/// <value>
		/// A null value means the job has not been started.  
		/// This value will be non-null if a job as started executing at least once.
		/// Value is reset when job starts to execute.
		/// </value>
		[DataMember(Name = "LastStartTime", IsRequired = true)]
		public DateTime? LastStartTime { get; set; }
		/// <summary>
		/// Gets the DateTime when the job execution finished.
		/// </summary>
		/// <value>
		/// A null value means the job has not once finished execution. 
		/// This value will be non-null if a job as finished executing at least once.
		/// A non-null value does not indicate success, simply that the job has finished executing at least once.
		/// Value is reset when job starts to execute.
		/// </value>
		[DataMember(Name = "LastEndTime", IsRequired = true)]
		public DateTime? LastEndTime { get; set; }
		/// <summary>
		/// Gets the time that this job is allowed to run, before being forcefully terminated.
		/// </summary>
		/// <value>
		/// A null value means that the job will never forcefully be terminated (except perhaps on service shutdown).
		/// </value>
		[DataMember(Name = "AbsoluteTimeout", IsRequired = true)]
		public TimeSpan? AbsoluteTimeout { get; set; }
		/// <summary>
		/// Gets the current status of this job.
		/// </summary>
		/// <value>
		/// This value changes depending on where in the execution lifecycle the job is currently.
		/// </value>
		[DataMember(Name = "Status", IsRequired = true)]
		public JobStatus Status { get; set; }

		[DataMember(Name = "QueueId", IsRequired = true)]
		public int QueueId { get; set; }

		[DataMember(Name = "JobType", IsRequired = true)]
		public string JobType { get; set; }

		[DataMember(Name = "CalendarSchedule", IsRequired = true)]
		public CalendarSchedule CalendarSchedule { get; set; }

		[DataMember(Name = "Application", IsRequired = true)]
		public string Application { get; set; }

		[DataMember(Name = "Group", IsRequired = true)]
		public string Group { get; set; }

		[DataMember(Name = "SuppressHistory", IsRequired = true)]
		public bool SuppressHistory { get; set; }

		[DataMember(Name = "DeleteWhenDone", IsRequired = true)]
		public bool DeleteWhenDone { get; set; }

		[DataMember(Name = "Name", IsRequired = true)]
		public string Name { get; set; }

		[DataMember(Name = "Description", IsRequired = true)]
		public string Description { get; set; }
	}
}
