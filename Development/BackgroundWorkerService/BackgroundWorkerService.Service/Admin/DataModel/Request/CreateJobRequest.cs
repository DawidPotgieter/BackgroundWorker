using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Request
{
	[DataContract(Name = "CreateJobRequest", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class CreateJobRequest
	{
		[DataMember(Name = "Type", IsRequired = true)]
		public string Type { get; set; }

		[DataMember(Name = "Data", IsRequired = true)]
		public string Data { get; set; }

		[DataMember(Name = "UniqueId", IsRequired = false)]
		public Guid? UniqueId { get; set; }

		[DataMember(Name = "MetaData", IsRequired = true)]
		public string MetaData { get; set; }

		[DataMember(Name = "QueueId", IsRequired = true)]
		public byte QueueId { get; set; }

		[DataMember(Name = "Application", IsRequired = false)]
		public string Application { get; set; }

		[DataMember(Name = "Group", IsRequired = false)]
		public string Group { get; set; }

		[DataMember(Name = "Name", IsRequired = false)]
		public string Name { get; set; }

		[DataMember(Name = "Description", IsRequired = false)]
		public string Description { get; set; }

		[DataMember(Name = "AbsoluteTimeout", IsRequired = false)]
		public TimeSpan? AbsoluteTimeout { get; set; }

		[DataMember(Name = "Status", IsRequired = false)]
		public JobStatus? Status { get; set; }

		[DataMember(Name = "CreatedDate", IsRequired = false)]
		public DateTime? CreatedDate { get; set; }

		[DataMember(Name = "SuppressHistory", IsRequired = false)]
		public bool SuppressHistory { get; set; }

		[DataMember(Name = "DeleteWhenDone", IsRequired = false)]
		public bool DeleteWhenDone { get; set; }

		[DataMember(Name = "CalendarSchedule", IsRequired = false)]
		public CalendarSchedule CalendarSchedule { get; set; }
	}
}
