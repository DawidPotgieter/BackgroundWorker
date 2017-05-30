using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "GetJobsResponse", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class GetJobsResponse
	{
		[DataMember(Name = "Jobs", IsRequired = true)]
		public List<JobData> Jobs { get; set; }
	}
}
