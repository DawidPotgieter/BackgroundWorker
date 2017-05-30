using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.datamodel
{
	public class SimpleSchedule
	{
		public DayOfWeek[] DaysOfWeek { get; set; }
		public TimeSpan? StartDailyAt { get; set; }
		public TimeSpan? RepeatInterval { get; set; }
		public string StartDateTime { get; set; }
	}
}