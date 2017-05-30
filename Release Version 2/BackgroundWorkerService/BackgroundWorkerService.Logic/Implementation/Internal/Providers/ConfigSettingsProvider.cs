using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel;
using BackgroundWorkerService.Logic.Configuration;
using BackgroundWorkerService.Logic.Helpers;
using BackgroundWorkerService.Logic.Implementation.JobStore;

namespace BackgroundWorkerService.Logic.Implementation.Internal.Providers.Configuration
{
	/// <summary>
	/// A <see cref="System.Configuration.ConfigurationSection"/> implementation of the <see cref="Interfaces.ISettingsProvider"/>
	/// interface.  This class is used to read settings from the app.config file for the background worker service.
	/// </summary>
	class ConfigSettingsProvider : System.Configuration.ConfigurationSection, ISettingsProvider
	{
		#region ISettingsProvider Members

		/// <summary>
		/// A <see cref="TimeSpan"/> value indicating how often the background worker service will poll for new jobs from the data store.
		/// </summary>
		/// <value></value>
		[ConfigurationProperty("PollFrequency", IsRequired = true)]
		public TimeSpan PollFrequency
		{
			get { return (TimeSpan)base["PollFrequency"]; }
		}

		/// <summary>
		/// A nullable <see cref="TimeSpan"/> value indicating how long the background worker service will wait for
		/// jobs to complete before forcing a shutdown.  It's a good idea to set this to null,
		/// as jobs should complete before the service shuts down.  However, if jobs become unstable and never times out,
		/// it becomes quite difficult stopping the service.  Usage dependent.
		/// Optional.  Defaults to 'null' if not set.
		/// </summary>
		/// <value></value>
		[ConfigurationProperty("ShutdownTimeout", IsRequired = false, DefaultValue = null)]
		public TimeSpan? ShutdownTimeout 
		{
			get { return (TimeSpan?)base["ShutdownTimeout"]; }
		}

		/// <summary>
		/// Gets the concrete implementation type of the job store to use.
		/// </summary>
		/// <value>
		/// The type of the job store. Defaults to RamJobStore if not specified.
		/// </value>
		public Type JobStoreType
		{
			get
			{
				Type jobStoreType = typeof(RamJobStore);
				if (JobStoreConfigDefinition != null && !string.IsNullOrEmpty(JobStoreConfigDefinition.Type))
				{
					jobStoreType = Type.GetType(JobStoreConfigDefinition.Type);
				}
				return jobStoreType;
			}
		}

		/// <summary>
		/// Gets the type of the job store settings provider.
		/// </summary>
		/// <value>
		/// The type of the job store settings provider.
		/// </value>
		public Type JobStoreSettingsProviderType
		{
			get
			{
				Type jobStoreProviderType = null;
				if (JobStoreConfigDefinition != null)
				{
					jobStoreProviderType = Type.GetType(JobStoreConfigDefinition.SettingsProviderType);
				}
				return jobStoreProviderType;
			}
		}

		/// <summary>
		/// This setting provides the name of the background worker service that has picked up a specific job.
		/// A good idea in a custom settings provider is to use the computer name here, or
		/// FirstTimerJobService etc.
		/// </summary>
		/// <value>
		/// Defaults to the current machine name if not set, or set to empty string.
		/// </value>
		[ConfigurationProperty("InstanceName", IsRequired = false)]
		public string InstanceName
		{
			get 
			{
				if (string.IsNullOrEmpty((string)base["InstanceName"]))
					return System.Environment.MachineName;
				return (string)base["InstanceName"]; 
			}
		}

		/// <summary>
		/// Gets the queue definitions that was specified in the config file.
		/// </summary>
		public List<QueueDefinition> Queues
		{
			get
			{
				List<QueueDefinition> queueDefinitions = new List<QueueDefinition>();
				QueuesConfigCollection queueDefinitionsCollection = QueueDefinitions;
				foreach (QueueConfigDefinition queueDefinition in queueDefinitionsCollection)
				{
					queueDefinitions.Add(new QueueDefinition { Id = queueDefinition.Id, Type = queueDefinition.Type, ThreadCount = queueDefinition.ThreadCount });
				}
				return queueDefinitions;
			}
		}

		#endregion

		[ConfigurationProperty("JobStore", IsRequired = false)]
		private JobStoreConfigDefinition JobStoreConfigDefinition
		{
			get
			{
				return base["JobStore"] as JobStoreConfigDefinition;
			}
		}

		[ConfigurationProperty("Queues", IsDefaultCollection = false, IsKey = false, IsRequired = false)]
		private QueuesConfigCollection QueueDefinitions
		{
			get
			{
				QueuesConfigCollection ret = base["Queues"] as QueuesConfigCollection;
				if (ret == null)
				{
					ret = new QueuesConfigCollection();
				}
				return ret;
			}
		}

	}
}
