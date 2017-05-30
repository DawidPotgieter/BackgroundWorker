using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.ServiceModel;
using BackgroundWorkerService.Logic.Interfaces;
using Common.Logging;

namespace BackgroundWorkerService.AzureWorker
{
	public class WorkerRole : RoleEntryPoint
	{
		private ServiceHost accessPointHost = null;
		private BackgroundWorkerService.Logic.Service workerService = null;
		private CancellationTokenSource cancelSource = new CancellationTokenSource();

		public WorkerRole()
			: base()
		{
		}

		public override void Run()
		{
			cancelSource.Token.WaitHandle.WaitOne();
			cancelSource.Dispose();
			cancelSource = null;
		}

		public override bool OnStart()
		{
			workerService = new BackgroundWorkerService.Logic.Service();

			SetupAdminAccessPoint();
			try
			{
				workerService.Start();
			}
			catch (Exception ex)
			{
				Logger.Error(new Exception("BackgroundWorkerService.Service Failed to Start service.", ex));
			}

			return base.OnStart();
		}

		public override void OnStop()
		{
			Stop(true);
			base.OnStop();
		}

		private void Stop(bool waitforJobs)
		{
			try
			{
				if (workerService.ServiceStatus != Logic.DataModel.Internal.Service.ServiceStatus.Stopped && workerService.ServiceStatus != Logic.DataModel.Internal.Service.ServiceStatus.Stopping)
				{
					workerService.Stop(waitforJobs);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(new Exception("BackgroundWorkerService.Service Failed to Stop service.", ex));
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

			cancelSource.Cancel();
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
			BackgroundWorkerService.Service.Admin.IAccessPoint accessPoint = new BackgroundWorkerService.Service.Admin.AccessPoint(workerService, SettingsProvider);
			try
			{
				IPEndPoint ip = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["AccessPoint"].IPEndpoint;
				Uri baseAddress = new Uri(String.Format("http://{0}/BackgroundWorkerService.Service", ip));
				accessPointHost = new ServiceHost(accessPoint, baseAddress);
				accessPointHost.Open();
			}
			catch (Exception ex)
			{
				Logger.Error(new Exception("BackgroundWorkerService.Service Failed to open external admin interface.", ex));
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

		private ILog logger;
		/// <summary>
		/// Gets the logging provider.
		/// </summary>
		/// <value>The logging provider.</value>
		internal ILog Logger
		{
			get
			{
				if (logger == null)
				{
					logger = LogManager.GetCurrentClassLogger();
				}
				return logger;
			}
		}
	}
}
