using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace BackgroundWorkerService.Service.Configuration
{
	class WebServerConfiguration : ConfigurationSection
	{
		[ConfigurationProperty("Enabled", IsRequired = false, DefaultValue = false)]
		internal bool Enabled
		{
			get { return (bool)base["Enabled"]; }
		}

		[ConfigurationProperty("Port", IsRequired = false, DefaultValue = 2048)]
		internal int Port
		{
			get { return (int)base["Port"]; }
		}

		[ConfigurationProperty("VirtualPath", IsRequired = false, DefaultValue = "/")]
		internal string VirtualPath
		{
			get 
			{
				//This is done because cassinidev kills the whole process if the virtual path is not specified.
				string virtualPath = (string)base["VirtualPath"];
				if (string.IsNullOrEmpty(virtualPath))
					virtualPath = "/";
				return virtualPath;
			}
		}

		[ConfigurationProperty("HostName", IsRequired = false)]
		internal string HostName
		{
			get { return (string)base["HostName"]; }
		}

		[ConfigurationProperty("ApplicationPath", IsRequired = false, DefaultValue = null)]
		internal string ApplicationPath
		{
			get 
			{ 
				string appPath = base["ApplicationPath"] as string;
				if (string.IsNullOrEmpty(appPath))
				{
					appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if DEBUG
					appPath = Path.Combine(appPath, @"..\..\..\..\BackgroundWorkerService.Web.UI");
#else
					appPath = Path.Combine(appPath, "WebUI");
#endif
				}
				return appPath;
			}
		}
	}
}
