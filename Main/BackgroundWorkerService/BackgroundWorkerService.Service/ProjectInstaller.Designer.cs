namespace BackgroundWorkerService.Service
{
	partial class ProjectInstaller
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.BackgroundWorkerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.BackgroundWorkerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			this.BackgroundWorkderServiceController = new System.ServiceProcess.ServiceController();
			// 
			// BackgroundWorkerServiceProcessInstaller
			// 
			this.BackgroundWorkerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
			this.BackgroundWorkerServiceProcessInstaller.Password = null;
			this.BackgroundWorkerServiceProcessInstaller.Username = null;
			this.BackgroundWorkerServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.BackgroundWorkerServiceProcessInstaller_AfterInstall);
			// 
			// BackgroundWorkerServiceInstaller
			// 
			this.BackgroundWorkerServiceInstaller.Description = "A background worker service that will execute jobs in a multithreaded environment" +
    " with two different queues.";
			this.BackgroundWorkerServiceInstaller.DisplayName = "Background Worker Service";
			this.BackgroundWorkerServiceInstaller.ServiceName = "Background Worker Service";
			this.BackgroundWorkerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			this.BackgroundWorkerServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.BackgroundWorkerServiceInstaller_AfterInstall);
			// 
			// BackgroundWorkderServiceController
			// 
			this.BackgroundWorkderServiceController.ServiceName = "Background Worker Service";
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.BackgroundWorkerServiceProcessInstaller,
            this.BackgroundWorkerServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller BackgroundWorkerServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller BackgroundWorkerServiceInstaller;
		private System.ServiceProcess.ServiceController BackgroundWorkderServiceController;
	}
}