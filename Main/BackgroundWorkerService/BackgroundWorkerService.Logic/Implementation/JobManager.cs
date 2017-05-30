using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Logic.Implementation.JobStore;
using BackgroundWorkerService.Logic.Implementation.Internal.Providers.Configuration;
using BackgroundWorkerService.Logic.Configuration;
using BackgroundWorkerService.Logic.Helpers;
using Common.Logging;

namespace BackgroundWorkerService.Logic.Implementation
{
	/// <summary>
	/// The default concreted implementation of <see cref="IJobManager"/>
	/// </summary>
	public sealed class JobManager : IJobManager
	{
		private IJobStore jobStore;
		private ILog logger;
		private ISettingsProvider settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="JobManager"/> class.
		/// </summary>
		public JobManager()
		{
			settings = ConfigurationSettings.SettingsProvider;
			jobStore = Utils.CreateInstanceWithRequiredInterface(settings.JobStoreType.AssemblyQualifiedName, typeof(IJobStore).Name) as IJobStore;
			Initialize(jobStore, settings, LogManager.GetCurrentClassLogger());
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JobManager"/> class.
		/// </summary>
		/// <param name="jobStore">The job store.</param>
		public JobManager(IJobStore jobStore)
			: this(jobStore, ConfigurationSettings.SettingsProvider, LogManager.GetCurrentClassLogger())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JobManager"/> class.
		/// </summary>
		/// <param name="jobStore">The job store.</param>
		/// <param name="settingsProvider">The settings provider.</param>
		/// <param name="loggingProvider">The logging provider.</param>
		public JobManager(IJobStore jobStore, ISettingsProvider settingsProvider, ILog loggingProvider)
		{
			Initialize(jobStore, settingsProvider, loggingProvider);
		}

		private void Initialize(IJobStore jobStore, ISettingsProvider settingsProvider, ILog loggingProvider)
		{
			this.jobStore = jobStore;
			this.logger = loggingProvider;
			this.settings = settingsProvider;
		}

		/// <summary>
		/// Gets the job store that is used to persist jobs.
		/// </summary>
		public IJobStore JobStore
		{
			get 
			{ 
				return jobStore; 
			}
		}

		/// <summary>
		/// Gets the logger for this job manager.
		/// </summary>
		public ILog Logger
		{
			get { return logger; }
		}
	}
}
