using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace BackgroundWorkerService.Service
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
		}

		private void BackgroundWorkerServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
		{
			//System.Diagnostics.Debugger.Break();
			string installDir = Context.Parameters["TARGETDIR"];

			BackgroundWorkerService.Service.Install.Configuration frm = new BackgroundWorkerService.Service.Install.Configuration(installDir);
			frm.BringToFront();
			frm.ShowDialog();

			//TODO : Add the dependent actions for hosting wcf admin endpoint and also the webserver permissions.

			//BackgroundWorkderServiceController.Start();
			//BackgroundWorkderServiceController.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, new TimeSpan(0, 1, 0));
		}

		private void BackgroundWorkerServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
		{

		}
	}
}
