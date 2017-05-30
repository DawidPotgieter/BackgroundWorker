using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "CreateJobResponse", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class CreateJobResponse
	{
		[DataMember(Name = "Job", IsRequired = true)]
		public JobData Job { get; set; }
	}
}
