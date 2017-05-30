using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BackgroundWorkerService.Logic.Helpers;
using BackgroundWorkerService.Logic.Interfaces;

namespace BackgroundWorkerService.Logic.Configuration
{
	/// <summary>
	/// Provides the settings for a job store of type <see cref="Implementation.JobStore.Linq2Sql.Linq2SqlJobStore"/>
	/// </summary>
	class Linq2SqlJobStoreConfigurationSection : System.Configuration.ConfigurationSection, ILinq2SqlJobStoreSettingsProvider
	{
		/// <summary>
		/// Gets the name of the connection string where the jobs are stored. The connectionstring name points to a named connection string in the &lt;connectionstrings&gt; section.
		/// </summary>
		/// <value>
		/// The name of the connection string.
		/// </value>
		[ConfigurationProperty("connectionStringName", IsRequired = true)]
		public string ConnectionStringName
		{
			get { return (string)base["connectionStringName"]; }
		}

		/// <summary>
		/// Gets the value of the connectionstring based on <see cref="ConnectionStringName"/>.
		/// </summary>
		public string ConnectionString
		{
			get
			{
				return Utils.GetConnectionString(ConnectionStringName);
			}
		}

		/// <summary>
		/// Gets the amount of time that transactions are allowed before being cancelled.  Defaults to 60 seconds.
		/// </summary>
		[ConfigurationProperty("transactionLockTimeout", IsRequired = false)]
		public TimeSpan TransactionLockTimeout
		{
			get 
			{
				TimeSpan? timeSpan = base["transactionLockTimeout"] as TimeSpan?;
				if (timeSpan.HasValue)
				{
					return timeSpan.Value;
				}
				else
				{
					return new TimeSpan(0, 1, 0);
				}
			}
		}
	}
}
