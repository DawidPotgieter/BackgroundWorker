using System;
using System.Collections.Generic;
using System.Text;

namespace BackgroundWorkerService.Logic.Interfaces
{
	/// <summary>
	/// This interface is used to provide logging functionality for the background worker service module.
	/// See <see cref="Configuration.ConfigurationSettings.LoggingProvider"/> that uses this interface for defaults. 
	/// </summary>
	public interface ILoggingProvider
	{
		/// <summary>
		/// Initializes the specified logging settings provider.  This is called by the service after the constructor.
		/// </summary>
		/// <param name="loggingSettingsProvider">The logging settings provider.</param>
		void Initialize(ILoggingSettingsProvider loggingSettingsProvider);
		/// <summary>
		/// Log a standard message to a data store.
		/// </summary>
		/// <param name="message">The message to log.</param>
		void LogMessage(string message);
		/// <summary>
		/// Log an exception.  The implementation should preferably log the complete exception tree.
		/// </summary>
		/// <param name="ex">The exception to log.</param>
		void LogException(Exception ex);
		/// <summary>
		/// Log an exception with a custom exception message to a data store.  The implementation should preferably log the complete exception tree.
		/// </summary>
		/// <param name="message">The custom exception message to log.</param>
		/// <param name="ex">The exception to log.</param>
		void LogException(string message, Exception ex);
		/// <summary>
		/// Log a warning to a data store.
		/// </summary>
		/// <param name="message">The message to log as a warning.</param>
		void LogWarning(string message);
	}
}
