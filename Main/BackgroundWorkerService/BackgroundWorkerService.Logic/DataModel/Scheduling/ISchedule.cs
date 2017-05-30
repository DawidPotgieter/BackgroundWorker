using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.DataModel.Scheduling
{
	/// <summary>
	/// An interface that must be implemented by each schedule provider.
	/// </summary>
	public interface ISchedule
	{
		/// <summary>
		/// Gets the next occurrence of this schedule (if any) from now onwards.
		/// </summary>
		DateTime? GetNextOccurrence();
		/// <summary>
		/// Gets the next occurrence after a specified date time.
		/// </summary>
		/// <param name="afterDateTime">The after date time - this datetime is not included in the possible return values.</param>
		DateTime? GetNextOccurrence(DateTime afterDateTime);
		/// <summary>
		/// Gets or sets the start date time. No occurrences will happen before this datatime.
		/// </summary>
		/// <value>
		/// The start date time.
		/// </value>
		DateTime StartDateTime { get; }
		/// <summary>
		/// Determines whether this instance can occur at the specified run at date time.
		/// </summary>
		/// <param name="dateTime">The date time to check for valid occurrence.</param>
		/// <returns>
		///   <c>true</c> if this instance can occur at the specified date time; otherwise, <c>false</c>.
		/// </returns>
		bool CanOccurAt(DateTime dateTime);
	}
}
