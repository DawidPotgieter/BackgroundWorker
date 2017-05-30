using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using System.Configuration;

namespace BackgroundWorkerService.Logic.Configuration
{
	class RamJobStoreConfigurationSection : System.Configuration.ConfigurationSection, IRamJobStoreSettingsProvider
	{
		[ConfigurationProperty("maxHistoryRecords", IsRequired = false, DefaultValue = (long)10000)]
		public long MaxHistoryRecords
		{
			get { return (long)base["maxHistoryRecords"]; }
		}
	}
}
