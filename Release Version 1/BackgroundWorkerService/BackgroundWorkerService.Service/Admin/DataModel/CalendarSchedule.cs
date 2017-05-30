using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "CalendarSchedule", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class CalendarSchedule : Schedule
	{
		internal CalendarSchedule(Logic.DataModel.Scheduling.CalendarSchedule schedule)
			: base(schedule)
		{
			this.StartDateTime = schedule.StartDateTime;
			this.DaysOfWeek = schedule.DaysOfWeek;
			this.DaysOfMonth = schedule.DaysOfMonth;
			this.DaysOfYear = schedule.DaysOfYear;
			this.EndDateTime = schedule.EndDateTime;
			this.StartDailyAt = new TimeOfDay(schedule.DailyStartTime);
			this.RepeatInterval = schedule.RepeatInterval;
		}

		internal new Logic.DataModel.Scheduling.ISchedule AsInternalSchedule()
		{
			Logic.DataModel.Scheduling.CalendarSchedule schedule = (Logic.DataModel.Scheduling.CalendarSchedule)Logic.Helpers.Utils.CreateInstanceWithRequiredInterface(this.ScheduleType, typeof(Logic.DataModel.Scheduling.ISchedule).Name);
			schedule
				.StartingAt(StartDateTime)
				.StartDailyAt(StartDailyAt.AsInternalTimeOfDay())
				.EndingAt(EndDateTime)
				.WithRepeatInterval(RepeatInterval)
				.OnDaysOfWeek(DaysOfWeek)
				.OnDaysOfMonth(DaysOfMonth)
				.OnDaysOfYear(DaysOfYear);
			return schedule;
		}

		[DataMember(Name = "StartDateTime", IsRequired = true)]
		public DateTime StartDateTime { get; set; }

		[DataMember(Name = "DaysOfWeek", IsRequired = false)]
		public HashSet<DayOfWeek> DaysOfWeek { get; set; }

		[DataMember(Name = "DaysOfMonth", IsRequired = false)]
		public HashSet<int> DaysOfMonth { get; set; }

		[DataMember(Name = "DaysOfYear", IsRequired = false)]
		public HashSet<int> DaysOfYear { get; set; }

		[DataMember(Name = "EndDateTime", IsRequired = false)]
		public DateTime? EndDateTime { get; set; }

		[DataMember(Name = "StartDailyAt", IsRequired = false)]
		public TimeOfDay StartDailyAt { get; set; }

		[DataMember(Name = "RepeatInterval", IsRequired = false)]
		public TimeSpan? RepeatInterval { get; set; }
	}
}
