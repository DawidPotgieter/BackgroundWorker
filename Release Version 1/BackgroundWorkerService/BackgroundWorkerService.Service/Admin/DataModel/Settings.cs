using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using BackgroundWorkerService.Logic.Configuration;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "Settings", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class Settings
	{
		internal Settings(BackgroundWorkerService.Logic.Interfaces.ISettingsProvider settingsProvider)
		{
			PollFrequency = settingsProvider.PollFrequency;
			JobStoreType = settingsProvider.JobStoreType;
			JobStoreSettingsProviderType = settingsProvider.JobStoreSettingsProviderType;
			Queues = new List<QueueSettings>();
			if (settingsProvider.Queues != null)
			{
				foreach (var queue in settingsProvider.Queues)
				{
					Queues.Add(new QueueSettings { Id = queue.Id, Type = queue.Type, ThreadCount = queue.ThreadCount });
				}
			}
			ShutdownTimeout = settingsProvider.ShutdownTimeout;
			InstanceName = settingsProvider.InstanceName;
		}

		#region ISettingsProvider Members

		[DataMember(Name = "PollFrequency", IsRequired = true)]
		public TimeSpan PollFrequency { get; set; }

		[DataMember(Name = "JobStoreType", IsRequired = true)]
		public string JobStoreType { get; set; }

		[DataMember(Name = "JobStoreSettingsProviderType", IsRequired = true)]
		public string JobStoreSettingsProviderType { get; set; }

		[DataMember(Name = "Queues", IsRequired = true)]
		public List<QueueSettings> Queues { get; set; }

		[DataMember(Name = "ShutdownTimeout", IsRequired = false)]
		public TimeSpan? ShutdownTimeout { get; set; }

		[DataMember(Name = "InstanceName", IsRequired = true)]
		public string InstanceName { get; set; }

		#endregion
	}
}
