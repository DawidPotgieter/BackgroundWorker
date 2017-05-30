using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace BackgroundWorkerService.Logic.Configuration 
{
	/// <summary>
	/// This class is used to get the assembly/classname that provides the logging provider for the service.
	/// </summary>
	class LoggingProviderConfigurationSection : System.Configuration.ConfigurationSection
	{
		/// <summary>
		/// Gets the type of the logging provider.
		/// </summary>
		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return (string)base["type"]; }
		}

		/// <summary>
		/// Gets the name of the config section that provides logging provider settings.
		/// </summary>
		/// <value>
		/// The name of the config section.
		/// </value>
		[ConfigurationProperty("providerSectionName", IsRequired = true)]
		public string ProviderConfigSectionName
		{
			get { return (string)base["providerSectionName"]; }
		}

	}
}
