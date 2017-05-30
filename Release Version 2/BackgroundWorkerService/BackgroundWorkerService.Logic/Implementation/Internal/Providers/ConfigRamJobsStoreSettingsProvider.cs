using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;

namespace BackgroundWorkerService.Logic.Implementation.Internal.Providers
{
	class ConfigRamJobsStoreSettingsProvider : IRamJobStoreSettingsProvider
	{
		private Logic.Configuration.RamJobStoreConfigurationSection configSection;

		public ConfigRamJobsStoreSettingsProvider()
		{
			configSection = Helpers.Utils.GetConfigurationSection<Logic.Configuration.RamJobStoreConfigurationSection>();
		}

		public long MaxHistoryRecords
		{
			get
			{
				return configSection.MaxHistoryRecords;
			}
		}
	}
}
