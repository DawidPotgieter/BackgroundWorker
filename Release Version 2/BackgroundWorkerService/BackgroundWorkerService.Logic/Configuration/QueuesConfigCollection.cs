using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BackgroundWorkerService.Logic.Configuration
{
	/// <summary>
	/// A collection of queue definitions as specified in the config file.
	/// </summary>
	[ConfigurationCollection(typeof(QueueConfigDefinition), AddItemName = "Queue", CollectionType = ConfigurationElementCollectionType.BasicMap)]
	class QueuesConfigCollection : ConfigurationElementCollection
	{
		///<summary>
		///When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
		///</summary>
		///
		///<returns>
		///A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
		///</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new QueueConfigDefinition();
		}

		///<summary>
		///Gets the element key for a specified configuration element when overridden in a derived class.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
		///</returns>
		///
		///<param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for. </param>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((QueueConfigDefinition)element).Id;
		}
	}
}
