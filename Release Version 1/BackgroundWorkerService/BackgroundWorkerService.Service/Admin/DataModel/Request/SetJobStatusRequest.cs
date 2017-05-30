using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Request
{
	[DataContract(Name = "SetJobStatusRequest", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class SetJobStatusRequest
	{
		[DataMember(Name = "JobId", IsRequired = true)]
		public long JobId { get; set; }

		[DataMember(Name = "OldStatus", IsRequired = false)]
		public JobStatus? OldStatus { get; set; }

		[DataMember(Name = "NewStatus", IsRequired = true)]
		public JobStatus NewStatus { get; set; }

		[DataMember(Name = "ErrorMessage", IsRequired = false)]
		public string ErrorMessage { get; set; }

		[DataMember(Name = "MetaData", IsRequired = false)]
		public string MetaData { get; set; }
	}
}
