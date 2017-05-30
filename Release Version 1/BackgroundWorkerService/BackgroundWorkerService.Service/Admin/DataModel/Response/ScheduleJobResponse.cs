using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "ScheduleJobResponse", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class ScheduleJobResponse
	{
		[DataMember(Name = "Job", IsRequired = true)]
		public JobData Job { get; set; }
	}
}
