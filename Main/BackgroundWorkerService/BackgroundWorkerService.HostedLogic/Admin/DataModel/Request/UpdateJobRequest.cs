using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Request
{
	[DataContract(Name = "UpdateJobRequest", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class UpdateJobRequest
	{
		[DataMember(Name = "JobData", IsRequired = true)]
		public JobData JobData { get; set; }
	}
}
