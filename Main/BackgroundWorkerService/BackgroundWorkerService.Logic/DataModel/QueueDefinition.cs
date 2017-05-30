using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BackgroundWorkerService.Logic.DataModel
{
	/// <summary>
	/// The data object used to store queue definitions.
	/// </summary>
	public class QueueDefinition
	{
		/// <summary>
		/// Gets or sets the queue id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		[Description("The queue id.  Must be 0 - 255")]
		public byte Id { get; set; }
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public string Type { get; set; }
		/// <summary>
		/// Gets or sets the thread count.
		/// </summary>
		/// <value>
		/// The thread count.
		/// </value>
		public uint ThreadCount { get; set; }
	}
}
