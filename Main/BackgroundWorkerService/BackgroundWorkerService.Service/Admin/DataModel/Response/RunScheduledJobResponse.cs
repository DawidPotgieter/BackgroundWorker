using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "RunScheduledJobResponse", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class RunScheduledJobResponse
	{
		[DataMember(Name = "Success", IsRequired = true)]
		public bool Success { get; set; }
	}
}
