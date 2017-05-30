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

namespace BackgroundWorkerService.Logic.Implementation
{
	/// <summary>
	/// The default concreted implementation of <see cref="IJobManager"/>
	/// </summary>
	public sealed class JobManager : IJobManager
	{
		private IJobStore jobStore;
		private ILoggingProvider logger;
		private ISettingsProvider settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="JobManager"/> class.
		/// </summary>
		public JobManager()
		{
			settings = ConfigurationSettings.SettingsProvider;
			jobStore = Utils.CreateInstanceWithRequiredInterface(settings.JobStoreType, typeof(IJobStore).Name) as IJobStore;
			Initialize(jobStore, settings, ConfigurationSettings.LoggingProvider);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JobManager"/> class.
		/// </summary>
		/// <param name="jobStore">The job store.</param>
		public JobManager(IJobStore jobStore)
			: this(jobStore, ConfigurationSettings.SettingsProvider, ConfigurationSettings.LoggingProvider)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JobManager"/> class.
		/// </summary>
		/// <param name="jobStore">The job store.</param>
		/// <param name="settingsProvider">The settings provider.</param>
		/// <param name="loggingProvider">The logging provider.</param>
		public JobManager(IJobStore jobStore, ISettingsProvider settingsProvider, ILoggingProvider loggingProvider)
		{
			Initialize(jobStore, settingsProvider, loggingProvider);
		}

		private void Initialize(IJobStore jobStore, ISettingsProvider settingsProvider, ILoggingProvider loggingProvider)
		{
			this.jobStore = jobStore;
			this.logger = loggingProvider;
			this.settings = settingsProvider;
			this.jobStore.JobActionRequired += new EventHandler<JobActionRequiredEventArgs>(jobStore_JobActionRequired);
			this.jobStore.JobDeleted += new EventHandler<JobActionRequiredEventArgs>(jobStore_JobDeleted);
		}

		/// <summary>
		/// Occurs when the job store raises a JobActionRequired event.
		/// </summary>
		public event EventHandler<JobActionRequiredEventArgs> JobActionRequired;
		/// <summary>
		/// Occurs when the job stoer raises a JobDeleted event.
		/// </summary>
		public event EventHandler<JobActionRequiredEventArgs> JobDeleted;

		void jobStore_JobActionRequired(object sender, JobActionRequiredEventArgs e)
		{
			var jobActionRequired = JobActionRequired;
			if (jobActionRequired != null)
			{
				jobActionRequired(this, e);
			}
		}

		void jobStore_JobDeleted(object sender, JobActionRequiredEventArgs e)
		{
			var jobDeleted = JobDeleted;
			if (jobDeleted != null)
			{
				jobDeleted(this, e);
			}
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
		public ILoggingProvider Logger
		{
			get { return logger; }
		}
	}
}
