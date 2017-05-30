using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;
using BackgroundWorkerService.Logic.Configuration;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel;

namespace BackgroundWorkerService.Service.Install
{
	public class InstallerSettingsProvider : Logic.Interfaces.ISettingsProvider
	{
		string configFilePath;
		XmlNode currentSettings;
		XmlDocument xmlDoc;

		internal InstallerSettingsProvider(string configFilePath)
		{
			this.configFilePath = configFilePath;
			xmlDoc = new XmlDocument();
			xmlDoc.Load(configFilePath);
			currentSettings = xmlDoc.DocumentElement.SelectSingleNode("BackgroundWorkerService.Logic.Implementation.Internal.Providers.Configuration.ConfigSettingsProvider");
			if (InstanceName == string.Empty)
			{
				InstanceName = System.Environment.MachineName;
			}
		}

		internal void Commit()
		{
			xmlDoc.Save(configFilePath);
		}

		#region ISettingsProvider Members

		[Description("A TimeSpan value indicating how often the Service will poll for new jobs from the data store.")]
		public TimeSpan PollFrequency
		{
			get
			{
				return TimeSpan.Parse(currentSettings.Attributes["PollFrequency"].InnerText);
			}
			set
			{
				currentSettings.Attributes["PollFrequency"].InnerText = value.ToString();
			}
		}

		[Description("A list of queues that the system will use with id's from 0 - 255 & their sizes and active threads. READ ONLY.")]
		[Browsable(false)]
		public List<QueueDefinition> Queues
		{
			get
			{
				List<QueueDefinition> queues = new List<QueueDefinition>();
				var queueNodes = currentSettings.SelectNodes(@"Queues/Queue");
				foreach (XmlNode queueNode in queueNodes)
				{
					QueueDefinition queue = new QueueDefinition();
					byte queueId = 0;
					string queueType = queueNode.Attributes["type"].InnerText;
					uint queueThreads = 1;
					if (queueNode.Attributes["Id"].InnerText != string.Empty)
						byte.TryParse(queueNode.Attributes["Id"].InnerText, out queueId);
					if (queueNode.Attributes["ThreadCount"].InnerText != string.Empty)
						uint.TryParse(queueNode.Attributes["ThreadCount"].InnerText, out queueThreads);

					queue.Id = queueId;
					queue.Type = queueType;
					queue.ThreadCount = queueThreads;

					queues.Add(queue);
				}
				return queues;
			}
			set
			{
			}
		}

		[Description("A nullable TimeSpan value indicating how long the Service will wait for jobs to complete before forcing a shutdown.")]
		public TimeSpan? ShutdownTimeout
		{
			get
			{
				if (currentSettings.Attributes["ShutdownTimeout"].InnerText != string.Empty)
					return TimeSpan.Parse(currentSettings.Attributes["ShutdownTimeout"].InnerText);
				return null;
			}
			set
			{
				if (value.HasValue)
					currentSettings.Attributes["ShutdownTimeout"].InnerText = value.ToString();
				else
					currentSettings.Attributes["ShutdownTimeout"].InnerText = string.Empty;
			}
		}

		[Description("This value is used if BackgroundWorkerService.Logic.Configuration.EventLogLoggingProvider is set as the logging provider. If a custom logging provider is used, this value is ignored.")]
		public string EventLogSource
		{
			get
			{
				return currentSettings.Attributes["EventLogSource"].InnerText;
			}
			set
			{
				currentSettings.Attributes["EventLogSource"].InnerText = value;
			}
		}

		[Description("The connectionstring used by the Linq2Sql Job Store.  If not using this job store, this value can be ignored.")]
		public string Linq2SqlConnectionString
		{
			get
			{
				if (JobStoreType.Contains(typeof(Logic.Implementation.JobStore.Linq2Sql.Linq2SqlJobStore).FullName))
				{
					var linq2SqlJobStoreConfig = xmlDoc.DocumentElement.SelectSingleNode("BackgroundWorkerService.Logic.Configuration.Linq2SqlJobStoreConfigurationSection");
					if (linq2SqlJobStoreConfig != null)
					{
						string connectionStringName = linq2SqlJobStoreConfig.Attributes["connectionStringName"].InnerText;
						var connectionStringsConfig = xmlDoc.DocumentElement.SelectSingleNode(string.Format("connectionStrings/add[@name='{0}']", connectionStringName));
						if (connectionStringsConfig != null) return connectionStringsConfig.Attributes["connectionString"].InnerText;
					}
				}
				return string.Empty;
			}
			set
			{
				if (JobStoreType.Contains(typeof(Logic.Implementation.JobStore.Linq2Sql.Linq2SqlJobStore).FullName))
				{
					var linq2SqlJobStoreConfig = xmlDoc.DocumentElement.SelectSingleNode("BackgroundWorkerService.Logic.Configuration.Linq2SqlJobStoreConfigurationSection");
					if (linq2SqlJobStoreConfig != null)
					{
						string connectionStringName = linq2SqlJobStoreConfig.Attributes["connectionStringName"].InnerText;
						var connectionStringsConfig = xmlDoc.DocumentElement.SelectSingleNode(string.Format("connectionStrings/add[@name='{0}']", connectionStringName));
						if (connectionStringsConfig != null) connectionStringsConfig.Attributes["connectionString"].InnerText = value;
					}
				}
			}
		}

		[Description("This setting provides the name of the service that has picked up a specific job.")]
		public string InstanceName
		{
			get
			{
				return currentSettings.Attributes["InstanceName"].InnerText;
			}
			set
			{
				currentSettings.Attributes["InstanceName"].InnerText = value;
			}
		}

		[Description("The object type of the jobstore implementation. Leave blank to use RamJobStore.")]
		public string JobStoreType
		{
			get 
			{
				var jobStoreNode = currentSettings.SelectSingleNode(@"JobStore");
				if (jobStoreNode != null)
				{
					return jobStoreNode.Attributes["type"].InnerText;
				}
				return null;
			}
		}

		[Description("The object type that provides the job store with it's settings.")]
		public string JobStoreSettingsProviderType
		{
			get
			{
				var jobStoreNode = currentSettings.SelectSingleNode(@"JobStore");
				if (jobStoreNode != null)
				{
					return jobStoreNode.Attributes["settingsProviderType"].InnerText;
				}
				return null;
			}
		}

		#endregion
	}
}
