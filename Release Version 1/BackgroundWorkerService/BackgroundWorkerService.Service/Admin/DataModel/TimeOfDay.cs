using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "TimeOfDay", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class TimeOfDay
	{
		internal TimeOfDay(int hour, int minute, int second)
		{
			Hour = hour;
			Minute = minute;
			Second = second;
		}

		internal TimeOfDay(TimeSpan timeOfDay)
		{
			Hour = timeOfDay.Hours;
			Minute = timeOfDay.Minutes;
			Second = timeOfDay.Seconds;
		}

		internal TimeOfDay(Logic.DataModel.Scheduling.TimeOfDay timeOfDay)
		{
			Hour = timeOfDay.Hour;
			Minute = timeOfDay.Minute;
			Second = timeOfDay.Second;
		}

		internal Logic.DataModel.Scheduling.TimeOfDay AsInternalTimeOfDay()
		{
			return new Logic.DataModel.Scheduling.TimeOfDay
			{
				Hour = this.Hour,
				Minute = this.Minute,
				Second = this.Second,
			};
		}

		[DataMember(Name = "Hour", IsRequired = true)]
		public int Hour { get; set; }

		[DataMember(Name = "Minute", IsRequired = true)]
		public int Minute { get; set; }

		[DataMember(Name = "Second", IsRequired = true)]
		public int Second { get; set; }
	}
}
