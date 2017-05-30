using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace BackgroundWorkerService.Service
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			bool uiMode = false;
			string[] commandLineArguments = Environment.GetCommandLineArgs();

			if (commandLineArguments.Length == 2)
			{
				if (commandLineArguments[1].ToLower().Contains("ui"))
				{
					uiMode = true;
				}
			}

			if (uiMode)
			{
				BackgroundWorkerService.Service.Service executor = new BackgroundWorkerService.Service.Service();
				System.Windows.Forms.Application.EnableVisualStyles();
				System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
				System.Windows.Forms.Application.Run(new UserInterface(executor, true));
			}
			else
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] 
				{ 
					new BackgroundWorkerService.Service.Service() 
				};
				ServiceBase.Run(ServicesToRun);
			}
		}
	}
}
