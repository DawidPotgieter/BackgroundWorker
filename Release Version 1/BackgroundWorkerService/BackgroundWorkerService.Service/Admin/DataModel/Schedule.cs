using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "Schedule", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class Schedule
	{
		internal Schedule(Logic.DataModel.Scheduling.ISchedule schedule)
		{
			ScheduleType = schedule.GetType().AssemblyQualifiedName;
		}

		internal Logic.DataModel.Scheduling.ISchedule AsInternalSchedule()
		{
			Logic.DataModel.Scheduling.ISchedule schedule = (Logic.DataModel.Scheduling.ISchedule)Logic.Helpers.Utils.CreateInstanceWithRequiredInterface(this.ScheduleType, typeof(Logic.DataModel.Scheduling.ISchedule).Name);
			return schedule;
		}

		[DataMember(Name = "ScheduleType", IsRequired = true)]
		public string ScheduleType { get; set; }
	}
}
