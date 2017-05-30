using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Xml;

namespace BackgroundWorkerService.Service.Install
{
	public partial class Configuration : Form
	{
		string installPath;
		public Configuration(string installPath)
		{
			InitializeComponent();
			this.installPath = System.IO.Path.GetDirectoryName(installPath) + @"\";
		}

		private void Configuration_Load(object sender, EventArgs e)
		{
			try
			{
				InstallerSettingsProvider settings = new InstallerSettingsProvider(installPath + "BackgroundWorkerService.Service.exe.config");
				SettingsGrid.SelectedObject = settings;
				this.BringToFront();
			}
			catch (Exception ex)
			{
				btnOk.Enabled = false;
				MessageBox.Show(ex.Message + "\n\n" + "You have to manually configure the service by editing '" + installPath + "\\BackgroundWorkerService.Service.exe.config'.");
			}
		}

		private void Configuration_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			((InstallerSettingsProvider)SettingsGrid.SelectedObject).Commit();
		}
	}
}
