using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Service
{
	/// <summary>
	/// Event arguments used to notify the service consumers of events
	/// </summary>
	public class NotificationEventArgs : EventArgs
	{
		internal NotificationEventArgs(string message)
		{
			Message = message;
		}

		/// <summary>
		/// Gets the notification message.
		/// </summary>
		public string Message { get; private set; }
	}
}
