using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BackgroundWorkerService.Logic.Configuration
{
	/// <summary>
	/// Provides the information required to determine which job store provider to use.
	/// </summary>
	class JobStoreConfigDefinition : ConfigurationElement
	{
		/// <summary>
		/// Gets the assembly qualified type of full name of the class for the <see cref="Interfaces.IJobStore"/> implementation.
		/// </summary>
		[ConfigurationProperty("type", IsRequired = true)]
		internal string Type
		{
			get { return (string)base["type"]; }
		}

		/// <summary>
		/// Gets the assembly qualified type or full name of the class that provides the JobStore with it's settings.
		/// </summary>
		[ConfigurationProperty("settingsProviderType", IsRequired = false)]
		internal string SettingsProviderType
		{
			get { return (string)base["settingsProviderType"]; }
		}
	}
}
