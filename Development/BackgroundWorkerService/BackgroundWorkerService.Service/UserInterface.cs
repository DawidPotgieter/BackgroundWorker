using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BackgroundWorkerService.Logic.Interfaces;

namespace BackgroundWorkerService.Service
{
	public partial class UserInterface : Form
	{
		BackgroundWorkerService.Logic.Service backgroundWorkerService;

		public UserInterface(BackgroundWorkerService.Service.Service windowsService, bool exitButtonVisible)
		{
			InitializeComponent();
			this.backgroundWorkerService = windowsService.WorkerService;
			btnForceExit.Visible = exitButtonVisible;
		}

		void service_Notify(object sender, NotificationEventArgs e)
		{
			SetText(e.Message);
		}

		delegate void SetTextCallback(string text);
		private void SetText(string text)
		{
			try
			{
				if (this.lblMessage.InvokeRequired)
				{
					SetTextCallback d = new SetTextCallback(SetText);
					this.Invoke(d, new object[] { text });
				}
				else
				{
					uint runningJobsCount = backgroundWorkerService.RunningJobsCount;
					if (runningJobsCount > 0)
					{
						this.lblMessage.Text = string.Format("Executing Jobs : {0}\r\n{1}", backgroundWorkerService.RunningJobsCount, text);
					}
					else
					{
						this.lblMessage.Text = string.Format("Waiting...");
					}
					this.lblMessage.Refresh();
					Application.DoEvents();
				}
			}
			catch { }
		}

		private void btnForceExit_Click(object sender, EventArgs e)
		{
			btnForceExit.Enabled = false;
			backgroundWorkerService.Stop(true);
			Close();
		}

		private void UserInterface_Load(object sender, EventArgs e)
		{
			this.backgroundWorkerService.Notify += new EventHandler<NotificationEventArgs>(service_Notify);
			this.backgroundWorkerService.Start();
		}

		private void UserInterface_FormClosing(object sender, FormClosingEventArgs e)
		{
			backgroundWorkerService.Stop(true); //This is incase the form get's closed in some other way
			while (backgroundWorkerService.ServiceStatus != Logic.DataModel.Internal.Service.ServiceStatus.Stopped)
			{
				System.Threading.Thread.Sleep(1000);
			}
		}
	}
}
