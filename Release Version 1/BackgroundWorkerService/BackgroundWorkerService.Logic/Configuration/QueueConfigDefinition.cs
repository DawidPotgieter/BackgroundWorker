using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BackgroundWorkerService.Logic.Configuration
{
	/// <summary>
	/// Provides the information to define a queue in the config file.
	/// </summary>
	class QueueConfigDefinition : ConfigurationElement
	{
		/// <summary>
		/// Gets the id of the queue.  Number from 0-255.  Must be unique in the config file.
		/// </summary>
		[ConfigurationProperty("Id", IsRequired = false)]
		internal byte Id
		{
			get { return (byte)base["Id"]; }
		}

		/// <summary>
		/// Gets the assembly qualified type or fullname of the implementation of <see cref="Interfaces.Internal.IExecutionQueue"/>.
		/// </summary>
		[ConfigurationProperty("type", IsRequired = true)]
		internal string Type
		{
			get { return (string)base["type"]; }
		}

		/// <summary>
		/// Gets the thread count to be used for the thread queue.  See concrete queue implementations for more details.
		/// </summary>
		/// <seealso cref="Implementation.Internal.ThreadExecutionQueue"/>
		/// <seealso cref="Implementation.Internal.TimedThreadExecutionQueue"/>
		/// <seealso cref="Implementation.Internal.ThreadPoolExecutionQueue"/>
		[ConfigurationProperty("ThreadCount", IsRequired = true)]
		internal uint ThreadCount
		{
			get { return (uint)base["ThreadCount"]; }
		}
	}
}
