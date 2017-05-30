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
using System.IO;
using System.Reflection;

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
			this.BringToFront();
			this.TopMost = true;

			try
			{
				txtInstructions.LoadFile(GetResourceFileStream(Assembly.GetExecutingAssembly(), "Install.Important Next Steps.rtf"), RichTextBoxStreamType.RichText);
			}
			catch (Exception ex)
			{
				txtInstructions.Text = "Oops.  Could not load Important Next Steps.rtf\n\n" + ex.ToString();
			}
		}

		private void Configuration_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Reads the specified resource (file) from the assembly as a <see cref="Stream"/>.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		private Stream GetResourceFileStream(Assembly assembly, string filename)
		{
			string path = assembly.ManifestModule.Name;
			path = path.Substring(0, path.LastIndexOf("."));
			return assembly.GetManifestResourceStream(path + "." + filename);
		}
	}
}
