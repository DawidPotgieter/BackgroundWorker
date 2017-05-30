using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using BackgroundWorkerService.Logic.Interfaces;

namespace BackgroundWorkerService.Logic.Implementation.Internal.Providers.Configuration
{
	class EventLogLoggingProvider : ILoggingProvider
	{
		#region ILoggingProvider Members
		private string LogSource = "Background Worker Service";

		/// <summary>
		/// Initializes a new instance of the <see cref="EventLogLoggingProvider"/> class.
		/// </summary>
		public EventLogLoggingProvider()
		{
		}

		public void Initialize(ILoggingSettingsProvider loggingSettingsProvider)
		{
			if (loggingSettingsProvider is ConfigSettingsProvider)
			{
				LogSource = ((ConfigSettingsProvider)loggingSettingsProvider).EventLogSource;
			}
		}

		/// <summary>
		/// Log a standard message to the event log specified in <see cref="ConfigSettingsProvider.EventLogSource"/>.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void LogMessage(string message)
		{
			try
			{
				EventLog.WriteEntry(LogSource, message);
			}
			catch {}
		}

		/// <summary>
		/// Log the complete exception tree to the event log specified in <see cref="ConfigSettingsProvider.EventLogSource"/>.
		/// </summary>
		/// <param name="ex">The exception to log.</param>
		public void LogException(Exception ex)
		{
			try
			{
				EventLog.WriteEntry(LogSource, Helpers.Utils.GetExceptionMessage(ex), EventLogEntryType.Error);
			}
			catch { }
		}

		/// <summary>
		/// Log the specified customer error message followed by the complete exception tree to the event log specified in <see cref="ConfigSettingsProvider.EventLogSource"/>.
		/// </summary>
		/// <param name="message">The custom exception message to log.</param>
		/// <param name="ex">The exception to log.</param>
		public void LogException(string message, Exception ex)
		{
			try
			{
				EventLog.WriteEntry(LogSource, message + "\n\n" + Helpers.Utils.GetExceptionMessage(ex), EventLogEntryType.Error);
			}
			catch { }
		}

		/// <summary>
		/// Log a warning the event log specified in <see cref="ConfigSettingsProvider.EventLogSource"/>.
		/// </summary>
		/// <param name="message">The message to log as a warning.</param>
		public void LogWarning(string message)
		{
			try
			{
				EventLog.WriteEntry(LogSource, message, EventLogEntryType.Warning);
			}
			catch { }
		}

		#endregion
	}
}
