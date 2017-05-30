using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.DataModel.Scheduling
{
	/// <summary>
	/// Contains the time of day.  Even though timespan could be used, this makes more sense logically.
	/// </summary>
	public class TimeOfDay
	{
		private int hour;
		/// <summary>
		/// Gets or sets the hour.
		/// </summary>
		/// <value>
		/// The hour.
		/// </value>
		public int Hour
		{
			get
			{
				return hour;
			}
			set
			{
				if (value < 0)
					hour = 0;
				else if (value > 23)
					hour = 23;
				else
					hour = value;
			}
		}

		private int minute;
		/// <summary>
		/// Gets or sets the minute.
		/// </summary>
		/// <value>
		/// The minute.
		/// </value>
		public int Minute
		{
			get
			{
				return minute;
			}
			set
			{
				if (value < 0)
					minute = 0;
				else if (value > 59)
					minute = 59;
				else
					minute = value;
			}
		}

		private int second;
		/// <summary>
		/// Gets or sets the second.
		/// </summary>
		/// <value>
		/// The second.
		/// </value>
		public int Second
		{
			get
			{
				return second;
			}
			set
			{
				if (value < 0)
					second = 0;
				else if (value > 59)
					second = 59;
				else
					second = value;
			}
		}

		/// <summary>
		/// Gets the timeofday as timespan.
		/// </summary>
		public TimeSpan AsTimeSpan
		{
			get
			{
				return new TimeSpan(hour, minute, second);
			}
		}

		/// <summary>
		/// Gets the timeofday as datetime for today.
		/// </summary>
		public DateTime ForToday
		{
			get
			{
				DateTime value = DateTime.Now.Date;
				value = value.Add(AsTimeSpan);
				return value;
			}
		}
	}
}
