using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BackgroundWorkerService.Logic.DataModel.Scheduling
{
	/// <summary>
	/// This <see cref="ISchedule"/> implementation allows scheduling on specific days of week, month, year as well as a repeat interval (so, start at x time, repeat every 3 minutes).
	/// </summary>
	public class CalendarSchedule : ISchedule
	{
		private DateTime? cachedNextOccurrence = null;
		private DateTime? cacheParam = null;

		private HashSet<DayOfWeek> daysOfWeek = null;

		/// <summary>
		/// Gets or sets the days of week.  Null or Empty list means every day of the week.
		/// </summary>
		/// <value>
		/// The days of week.
		/// </value>
		[XmlArray("DaysOfWeek")]
		[XmlArrayItem("DayOfWeek")]
		public HashSet<DayOfWeek> DaysOfWeek
		{
			get 
			{ 
				return daysOfWeek; 
			}
			set
			{
				daysOfWeek = null;
				if (value != null)
				{
					daysOfWeek = new HashSet<DayOfWeek>(value);
				}
			}
		}

		private HashSet<int> daysOfMonth = null;
		/// <summary>
		/// Gets or sets the days of month. Null of Empty list means every day of the month.
		/// </summary>
		/// <value>
		/// The days of month.
		/// </value>
		[XmlArray("DaysOfMonth")]
		public HashSet<int> DaysOfMonth
		{
			get { return daysOfMonth; }
			set
			{
				daysOfMonth = null;
				if (value != null)
				{
					daysOfMonth = new HashSet<int>(value);
				}

				if (daysOfMonth != null)
				{
					daysOfMonth.RemoveWhere(dm => dm < 1 || dm > 31);
				}
			}
		}

		private HashSet<int> daysOfYear = null;
		/// <summary>
		/// Gets or sets the days of year. Null or Empty list means every day of year.
		/// </summary>
		/// <value>
		/// The days of year.
		/// </value>
		[XmlArray("DaysOfYear")]
		public HashSet<int> DaysOfYear
		{
			get { return daysOfYear; }
			set
			{
				daysOfYear = null;
				if (value != null)
				{
					daysOfYear = new HashSet<int>(value);
				}

				if (daysOfYear != null)
				{
					daysOfYear.RemoveWhere(dm => dm < 1 || dm > 365);
				}
			}
		}

		private DateTime? endDateTime = null;
		/// <summary>
		/// Gets or sets the end date time.  No occurrences will happen after this datetime.
		/// </summary>
		/// <value>
		/// The end date time.
		/// </value>
		public DateTime? EndDateTime
		{
			get { return endDateTime; }
			set { endDateTime = value; }
		}

		private DateTime startDateTime = DateTime.Now;
		/// <summary>
		/// Gets or sets the start date time. No occurrences will happen before this datatime.
		/// </summary>
		/// <value>
		/// The start date time.
		/// </value>
		public DateTime StartDateTime
		{
			get { return startDateTime; }
			set { startDateTime = value; }
		}

		private TimeOfDay dailyStartTime = new TimeOfDay { Hour = 0, Minute = 0, Second = 0 };
		/// <summary>
		/// Gets or sets the daily start time.  For non periodic (with repeatinterval) schedules, this will cause the next occurrence at this time.
		/// </summary>
		/// <value>
		/// The daily start time.
		/// </value>
		public TimeOfDay DailyStartTime
		{
			get { return dailyStartTime; }
			set
			{
				dailyStartTime = value;
				if (dailyStartTime == null)
				{
					dailyStartTime = new TimeOfDay { Hour = 0, Minute = 0, Second = 0 };
				}
			}
		}

		private TimeSpan? repeatInterval = null;
		/// <summary>
		/// Gets or sets the repeat interval.  If this is set, the next occurrence is calculated to happen on one of these interval multiples.
		/// </summary>
		/// <value>
		/// The repeat interval.
		/// </value>
		[XmlIgnore]
		public TimeSpan? RepeatInterval
		{
			get { return repeatInterval; }
			set 
			{ 
				repeatInterval = value;
				if (repeatInterval.HasValue && repeatInterval.Value.Ticks == 0)
					repeatInterval = null;
			}
		}

		/// <summary>
		/// Gets or sets the repeat interval as a string.
		/// </summary>
		/// <value>
		/// The repeat interval string.
		/// </value>
		/// <remarks>
		/// This is done for serialization purposes, because the default xml serializer doesn't handle nullable timespans.
		/// </remarks>
		public string RepeatIntervalString
		{
			get
			{
				if (repeatInterval == null) return null;
				return repeatInterval.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					RepeatInterval = null;
					return;
				}

				TimeSpan interval;
				if (TimeSpan.TryParse(value, out interval))
					RepeatInterval = interval;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarSchedule"/> class.
		/// </summary>
		public CalendarSchedule()
		{
			startDateTime = DateTime.Now;
			//This initializes the calender to default to every day of the week, month and year.
			this.OnDaysOfWeek(null).OnDaysOfMonth(null).OnDaysOfYear(null);
		}

		/// <summary>
		/// Called to set the <see cref="DaysOfWeek"/> value.
		/// </summary>
		/// <param name="daysOfWeek">The days of week.</param>
		/// <returns></returns>
		public CalendarSchedule OnDaysOfWeek(HashSet<DayOfWeek> daysOfWeek)
		{
			DaysOfWeek = daysOfWeek;	

			return this;
		}

		/// <summary>
		/// Called to set the <see cref="DaysOfMonth"/> value.
		/// </summary>
		/// <param name="daysOfMonth">The days of month.</param>
		/// <returns></returns>
		public CalendarSchedule OnDaysOfMonth(HashSet<int> daysOfMonth)
		{
			DaysOfMonth = daysOfMonth;

			return this;
		}

		/// <summary>
		/// Called to set the <see cref="DaysOfYear"/> value.
		/// </summary>
		/// <param name="daysOfYear">The days of year.</param>
		/// <returns></returns>
		public CalendarSchedule OnDaysOfYear(HashSet<int> daysOfYear)
		{
			DaysOfYear = daysOfYear;

			return this;
		}

		/// <summary>
		/// Sets the value of <see cref="DailyStartTime"/>.
		/// </summary>
		/// <param name="timeOfDay">The time of day.</param>
		/// <returns></returns>
		public CalendarSchedule StartDailyAt(TimeOfDay timeOfDay)
		{
			DailyStartTime = timeOfDay;
			return this;
		}

		/// <summary>
		/// Sets the value of <see cref="RepeatInterval"/>.
		/// </summary>
		/// <param name="repeatInterval">The repeat interval.</param>
		/// <returns></returns>
		public CalendarSchedule WithRepeatInterval(TimeSpan? repeatInterval)
		{
			RepeatInterval = repeatInterval;
			return this;
		}

		/// <summary>
		/// Sets the value of <see cref="EndDateTime"/>.
		/// </summary>
		/// <param name="endDateTime">The end date time.</param>
		/// <returns></returns>
		public CalendarSchedule EndingAt(DateTime? endDateTime)
		{
			EndDateTime = endDateTime;
			return this;
		}

		/// <summary>
		/// Sets the value of <see cref="StartDateTime"/>.
		/// </summary>
		/// <param name="startDateTime">The start date time.</param>
		/// <returns></returns>
		public CalendarSchedule StartingAt(DateTime startDateTime)
		{
			StartDateTime = startDateTime;
			return this;
		}

		/// <summary>
		/// Gets the next occurrence of this schedule (if any) from now onwards.
		/// </summary>
		/// <returns></returns>
		public DateTime? GetNextOccurrence()
		{
			return GetNextOccurrence(DateTime.Now);
		}

		/// <summary>
		/// Gets the next occurrence after a specified date time.
		/// </summary>
		/// <param name="afterDateTime">The after date time - this datetime is not included in the possible return values.</param>
		/// <returns></returns>
		public DateTime? GetNextOccurrence(DateTime afterDateTime)
		{
			DateTime now = DateTime.Now;

			//Short circuit the calculation.  We store both the calculated value and what the afterDateTime was.  So calls with the same param will not recalculate a value.
			if (cachedNextOccurrence.HasValue && cachedNextOccurrence.Value >= now && cacheParam.Value == afterDateTime)
				return cachedNextOccurrence;

			TimeSpan dailyStartAt = dailyStartTime.AsTimeSpan;
			DateTime? foundDateTime = null;

			//The start of the search must be in the future.
			DateTime startDateTime = (afterDateTime < this.StartDateTime ? this.StartDateTime : afterDateTime);

			DateTime compareDateTime = startDateTime;

			DateTime twoYearsFromNow = now.AddYears(2);

			//Have to search until either an occurrence is found, or we're past the (if any) schedule end date.  Also limit to two years search, because you could specify all kinds of funny calendars.
			while (foundDateTime == null && (endDateTime.HasValue ? compareDateTime < endDateTime : true) && (compareDateTime < twoYearsFromNow))
			{
				//Have to make sure that the day being checked is allowed.
				if (IsDayAllowed(compareDateTime))
				{
					//If there's no repeat interval set (i.e. repeat every 3 mins), then the earliest start date for the day is returned.
					if (!repeatInterval.HasValue)
					{
						if (compareDateTime.Date.Add(dailyStartAt) > startDateTime)
						{
							foundDateTime = compareDateTime.Date.Add(dailyStartAt);
						}
					}
					else
					{
						//If there is a repeat interval, need to start from the start of day and count off the periods until we reach the first possible after start datetime
						TimeSpan compareTime = dailyStartAt;

						while (compareDateTime.Date.Add(compareTime) < startDateTime)
						{
							compareTime = compareTime.Add(repeatInterval.Value);
						}

						var calculatedCompareDateTime = compareDateTime.Date.Add(compareTime);

						if (!endDateTime.HasValue || calculatedCompareDateTime <= endDateTime.Value)
						{
							//compare time now contains the first time after the calculated startdatetime that is on the periodic schedule.
							foundDateTime = calculatedCompareDateTime;
						}
					}
					//Ensure that the possible found date time is AFTER the afterDateTime
					//Since the above iterations could generate a date/time on a different day, we have to check the days again.
					if (foundDateTime.HasValue && (foundDateTime <= afterDateTime || !IsDayAllowed(foundDateTime.Value)))
					{
						foundDateTime = null;
					}
				}
				compareDateTime = compareDateTime.AddDays(1);
			}

			//If the found datetime is in the future by at least one day, make sure to use the daily start time with the day.
			if (foundDateTime.HasValue && foundDateTime.Value.DayOfYear > now.DayOfYear)
			{
				foundDateTime = new DateTime(foundDateTime.Value.Year, foundDateTime.Value.Month, foundDateTime.Value.Day, dailyStartAt.Hours, dailyStartAt.Minutes, dailyStartAt.Seconds);
			}

			//Cache the founddatetime and the param used to query the method
			cachedNextOccurrence = foundDateTime;
			cacheParam = afterDateTime;

			return foundDateTime;
		}

		/// <summary>
		/// Determines whether this instance can occur at the specified run at date time.
		/// </summary>
		/// <param name="dateTime">The date time to check for valid occurrence.</param>
		/// <returns>
		///   <c>true</c> if this instance can occur at the specified date time; otherwise, <c>false</c>.
		/// </returns>
		public bool CanOccurAt(DateTime dateTime)
		{
			if (IsDayAllowed(dateTime) && dateTime >= StartDateTime && (EndDateTime.HasValue ? dateTime <= EndDateTime.Value : true))
			{
				if (!repeatInterval.HasValue)
				{
					//If there's no repeat interval, check against the daily start time
					return dateTime >= dateTime.Date.Add(DailyStartTime.AsTimeSpan);
				}
				else
				{
					//In the case where there's a refresh interval, it's possible that the schedule can run before the daily start (since it could overflow from previous day).
					return true;
				}
			}
			return false;
		}

		private bool IsDayAllowed(DateTime compareDateTime)
		{
			//Have to make sure that the day being checked is allowed.  This shouldn't be too slow as hashsets have good performance.
			return ((daysOfWeek == null || daysOfWeek.Count == 0 || daysOfWeek.Contains(compareDateTime.DayOfWeek))
							&& (daysOfMonth == null || daysOfMonth.Count == 0 || daysOfMonth.Contains(compareDateTime.Day))
							&& (daysOfYear == null || daysOfYear.Count == 0 || daysOfYear.Contains(compareDateTime.DayOfYear)));
		}
	}
}
