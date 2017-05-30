using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Request
{
	[DataContract(Name = "ScheduleJobRequest", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class ScheduleJobRequest : CreateJobRequest
	{
		[DataMember(Name = "CalendarSchedule", IsRequired = true)]
		public CalendarSchedule CalendarSchedule { get; set; }
	}
}
