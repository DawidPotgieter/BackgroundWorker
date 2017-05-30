using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Request
{
	[DataContract(Name = "GetJobsRequest", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class GetJobsRequest
	{
		[DataMember(Name = "Skip", IsRequired = true)]
		public uint Skip { get; set; }

		[DataMember(Name = "Take", IsRequired = true)]
		public uint Take { get; set; }

		[DataMember(Name = "JobIds", IsRequired = false)]
		public List<long> JobIds { get; set; }

		[DataMember(Name = "JobStatuses", IsRequired = false)]
		public List<JobStatus> JobStatuses { get; set; }

		[DataMember(Name = "QueueIds", IsRequired = false)]
		public List<byte> QueueIds { get; set; }

		[DataMember(Name = "TypeNames", IsRequired = false)]
		public List<string> TypeNames { get; set; }

		[DataMember(Name = "Applications", IsRequired = false)]
		public List<string> Applications { get; set; }

		[DataMember(Name = "Groups", IsRequired = false)]
		public List<string> Groups { get; set; }
	}
}
