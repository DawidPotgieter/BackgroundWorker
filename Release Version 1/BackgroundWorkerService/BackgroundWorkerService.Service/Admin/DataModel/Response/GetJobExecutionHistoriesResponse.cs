using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "GetJobExecutionHistoriesResponse", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class GetJobExecutionHistoriesResponse
	{
		[DataMember(Name = "JobHistories", IsRequired = true)]
		public List<JobExecutionHistory> JobHistories { get; set; }
	}
}
