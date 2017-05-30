using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;
using BackgroundWorkerService.Logic.Interfaces;

namespace BackgroundWorkerService.Logic.Configuration
{
	/// <summary>
	/// Factory class that will handle the construction of settings providers.
	/// Settings Providers just need to implement the required interfaces.
	/// </summary>
	public class ConfigurationSettings
	{
		private static ISettingsProvider settingsProvider;
		/// <summary>
		/// Gets the settings provider specified by <see cref="ConfigSetupConfigurationSection"/>.
		/// </summary>
		/// <value>The instantiated settings provider object.  This object is cached in the appdomain when succesfully created.</value>
		public static ISettingsProvider SettingsProvider
		{
			get
			{
				if (settingsProvider == null)
				{
					string typeName = string.Empty;
					string providerConfigSectionName = string.Empty;
					ConfigSetupConfigurationSection configSetup = Helpers.Utils.GetConfigurationSection<ConfigSetupConfigurationSection>();
					try
					{
						typeName = configSetup.Type;
						providerConfigSectionName = configSetup.ProviderConfigSectionName;
						Type type = Type.GetType(typeName);

						if (type.GetInterface("BackgroundWorkerService.Logic.Interfaces.ISettingsProvider") == null)
						{
							throw new ConfigurationErrorsException(typeName + " does not support interface 'BackgroundWorkerService.Logic.Interfaces.ISettingsProvider'.");
						}
						settingsProvider = (ISettingsProvider)Activator.CreateInstance(type);
						if (typeof(System.Configuration.ConfigurationSection).IsAssignableFrom(settingsProvider.GetType()))
						{
							settingsProvider = (ISettingsProvider)ConfigurationManager.GetSection(providerConfigSectionName);
						}
					}
					catch (Exception ex)
					{
						throw new ConfigurationErrorsException("Failed to load : " + typeName + ".\n" + Helpers.Utils.GetExceptionMessage(ex));
					}
				}
				return settingsProvider;
			}
		}
	}
}
