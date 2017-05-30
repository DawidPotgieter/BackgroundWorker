using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.Implementation;
using BackgroundWorkerService.Logic.Implementation.JobStore.Linq2Sql;
using BackgroundWorkerService.Logic.Implementation.JobStore;
using CassiniDev;

namespace BackgroundWorkerService.Service
{
	public partial class Service : ServiceBase
	{
		private ServiceHost accessPointHost = null;
		private CassiniDevServer webServer = null;

		internal Service()
		{
			InitializeComponent();
			this.CanHandlePowerEvent = false;
			this.CanPauseAndContinue = true;
			this.CanShutdown = true;
			this.CanStop = true;

			workerService = new BackgroundWorkerService.Logic.Service();
			SetupAdminAccessPoint();
			SetupWebUI();
		}

		private BackgroundWorkerService.Logic.Service workerService;
		public BackgroundWorkerService.Logic.Service WorkerService
		{
			get
			{
				return workerService;
			}
		}

		/// <summary>
		/// Starts the built in webserver if it is enabled via config.
		/// </summary>
		/// <remarks>
		/// To run the built in webserver, you will need to assign write permissions to the user running the service for the folder :
		/// C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files
		/// 
		/// You migh also need to add the exceptions via netsh as specified for the admin access point.
		/// </remarks>
		private void SetupWebUI()
		{
			try
			{
				var config = BackgroundWorkerService.Logic.Helpers.Utils.GetConfigurationSection<Configuration.WebServerConfiguration>();
				if (config.Enabled)
				{
					webServer = new CassiniDevServer();
					webServer.StartServer(config.ApplicationPath, config.Port, config.VirtualPath, config.HostName);
				}
			}
			catch (Exception ex)
			{
				LoggingProvider.LogException(new Exception("BackgroundWorkerService.Service Failed to open hosted webserver.", ex));
			}
		}

		/// <summary>
		/// Setups the admin access point.
		/// </summary>
		/// <remarks>
		/// If you get a communication exception with the message : 'HTTP could not register URL http://+:7776/BackgroundWorkerService.Service/. Your process does not have access rights to this namespace.'
		/// then open a visual studio command prompt as administrator and run :
		/// netsh http add urlacl url=http://+:7776/BackgroundWorkerService.Service/ user=Everyone
		/// substituting the port numbers and the user account.  You need to tell windows who is allowed to host on specific ports.  You'll have to do this on the deployment server too.
		/// </remarks>
		private void SetupAdminAccessPoint()
		{
			Admin.IAccessPoint accessPoint = new Admin.AccessPoint(this);
			try
			{
				accessPointHost = new ServiceHost(accessPoint);
				accessPointHost.Open();
			}
			catch (Exception ex)
			{
				LoggingProvider.LogException(new Exception("BackgroundWorkerService.Service Failed to open external admin interface.", ex));
			}
		}

		private ISettingsProvider settingsProvider;
		/// <summary>
		/// Gets the settings provider.
		/// </summary>
		/// <value>The settings provider.</value>
		internal ISettingsProvider SettingsProvider
		{
			get
			{
				if (settingsProvider == null)
				{
					settingsProvider = BackgroundWorkerService.Logic.Configuration.ConfigurationSettings.SettingsProvider;
				}
				return settingsProvider;
			}
		}

		private ILoggingProvider loggingProvider;
		/// <summary>
		/// Gets the logging provider.
		/// </summary>
		/// <value>The logging provider.</value>
		internal ILoggingProvider LoggingProvider
		{
			get
			{
				if (loggingProvider == null)
				{
					loggingProvider = BackgroundWorkerService.Logic.Configuration.ConfigurationSettings.LoggingProvider;
				}
				return loggingProvider;
			}
		}

		UserInterface userInterface = null;
		/// <summary>
		/// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
		/// </summary>
		/// <param name="args">Data passed by the start command.</param>
		protected override void OnStart(string[] args)
		{
			try
			{
				WorkerService.Start();
				userInterface = new UserInterface(this, false);
				userInterface.Visible = false;
				System.Threading.Thread visualThread = new System.Threading.Thread(LoadForm);
				visualThread.Start();
			}
			catch (Exception ex)
			{
				LoggingProvider.LogException(new Exception("BackgroundWorkerService.Service Failed to Start service.", ex));
			}
		}

		private void Stop(bool waitforJobs)
		{
			try
			{
				if (WorkerService.ServiceStatus != Logic.DataModel.Internal.Service.ServiceStatus.Stopped && WorkerService.ServiceStatus != Logic.DataModel.Internal.Service.ServiceStatus.Stopping)
				{
					if (waitforJobs && SettingsProvider != null && SettingsProvider.ShutdownTimeout.HasValue)
					{
						RequestAdditionalTime((int)SettingsProvider.ShutdownTimeout.Value.TotalMilliseconds);
					}

					WorkerService.Stop(waitforJobs);
				}
			}
			catch (Exception ex)
			{
				LoggingProvider.LogException(new Exception("BackgroundWorkerService.Service Failed to Stop service.", ex));
			}

			if (userInterface != null)
			{
				userInterface.Close();
			}

			if (accessPointHost != null)
			{
				try
				{
					accessPointHost.Close();
				}
				catch { }
				accessPointHost = null;
			}
		}

		/// <summary>
		/// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
		/// </summary>
		protected override void OnStop()
		{
			Stop(true);
		}

		protected override void OnShutdown()
		{
			Stop(false);
		}

		protected override void OnPause()
		{
			WorkerService.Pause();
		}

		protected override void OnContinue()
		{
			WorkerService.Resume();
		}

		/// <summary>
		/// Loads the form.
		/// </summary>
		public void LoadForm()
		{
			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
			System.Windows.Forms.Application.Run();
			userInterface.Hide();
		}
	}
}
