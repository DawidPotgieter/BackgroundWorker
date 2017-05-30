using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;

namespace BackgroundWorkerService.Logic.Implementation.Internal.Providers
{
	class ConfigLinq2SqlJobStoreSettingsProvider : ILinq2SqlJobStoreSettingsProvider
	{
		private Logic.Configuration.Linq2SqlJobStoreConfigurationSection configSection;

		public ConfigLinq2SqlJobStoreSettingsProvider()
		{
			configSection = Helpers.Utils.GetConfigurationSection<Logic.Configuration.Linq2SqlJobStoreConfigurationSection>();
		}

		public string ConnectionString
		{
			get 
			{
				return configSection.ConnectionString;
			}
		}

		public TimeSpan TransactionLockTimeout
		{
			get
			{
				return configSection.TransactionLockTimeout;
			}
		}
	}
}
