using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Request
{
	[DataContract(Name = "SetJobStatusesRequest", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class SetJobStatusesRequest
	{
		[DataMember(Name = "JobIds", IsRequired = true)]
		public long[] JobIds { get; set; }

		[DataMember(Name = "OldStatus", IsRequired = false)]
		public JobStatus? OldStatus { get; set; }

		[DataMember(Name = "NewStatus", IsRequired = true)]
		public JobStatus NewStatus { get; set; }

		[DataMember(Name = "ErrorMessage", IsRequired = false)]
		public string ErrorMessage { get; set; }
	}
}
