using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Request
{
	[DataContract(Name = "RunScheduledJobRequest", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class RunScheduledJobRequest
	{
		[DataMember(Name = "JobId", IsRequired = true)]
		public long JobId { get; set; }
	}
}
