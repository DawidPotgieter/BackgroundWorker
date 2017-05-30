using System;
using System.Collections.Generic;
using System.Text;
using BackgroundWorkerService.Logic.Configuration;
using BackgroundWorkerService.Logic.DataModel;

namespace BackgroundWorkerService.Logic.Interfaces
{
	/// <summary>
	/// This interface is used to provide the background worker service Logic with the settings it requires to function.
	/// See <see cref="Configuration.ConfigurationSettings.SettingsProvider"/> that uses this interface for defaults.
	/// </summary>
	public interface ISettingsProvider
	{
		/// <summary>
		/// A <see cref="TimeSpan"/> value indicating how often the background worker service will poll for new jobs from the data store.
		/// </summary>
		TimeSpan PollFrequency { get; }

		/// <summary>
		/// Gets the execution queue definitions.
		/// </summary>
		List<QueueDefinition> Queues { get; }

		/// <summary>
		/// Gets the assembly qualified or full name of the the job store type.
		/// </summary>
		/// <value>
		/// The type of the job store.
		/// </value>
		string JobStoreType { get; }

		/// <summary>
		/// Gets the type of the job store settings provider.
		/// </summary>
		/// <value>
		/// The type of the job store settings provider.
		/// </value>
		string JobStoreSettingsProviderType { get; }

		/// <summary>
		/// A nullable <see cref="TimeSpan"/> value indicating how long the background worker service will wait for
		/// jobs to complete before forcing a shutdown.  It's a good idea to set this to null,
		/// as jobs should complete before the service shuts down.  However, if jobs become unstable and never times out,
		/// it becomes quite difficult stopping the service.  Usage dependent.
		/// </summary>
		TimeSpan? ShutdownTimeout { get; }

		/// <summary>
		/// This setting provides the name of the background worker service that has picked up a specific job.
		/// A good idea in a custom settings provider is to use the computer name here, or
		/// FirstTimerJobService etc.
		/// </summary>
		string InstanceName { get; }
	}
}
