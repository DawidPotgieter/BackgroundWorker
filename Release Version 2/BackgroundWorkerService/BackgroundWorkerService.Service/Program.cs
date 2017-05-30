using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Reflection;

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

			try
			{
				if (uiMode)
				{
					System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
					BackgroundWorkerService.Service.Service executor = new BackgroundWorkerService.Service.Service();
					System.Windows.Forms.Application.EnableVisualStyles();
					System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
					System.Windows.Forms.Application.Run(new UserInterface(executor, true));
				}
				else
				{
					System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
					ServiceBase[] ServicesToRun;
					ServicesToRun = new ServiceBase[] 
				{ 
					new BackgroundWorkerService.Service.Service() 
				};
					ServiceBase.Run(ServicesToRun);
				}
			}
			catch (Exception ex)
			{
				LogUnhandledException(ex);
			}
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			LogUnhandledException((Exception)e.ExceptionObject);
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			LogUnhandledException(e.Exception);
		}

		private static void LogUnhandledException(Exception ex)
		{
			try
			{
				//This is really a last ditch effort to provide some information when the service doesn't want to start - usually when the config is buggered.  Can't use the default
				//Logging here which is usually specified in config :(
				File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ServiceStartupErrors.txt"), "Fatal error starting up : " + ex.ToString());
			}
			catch { }
		}
	}
}
