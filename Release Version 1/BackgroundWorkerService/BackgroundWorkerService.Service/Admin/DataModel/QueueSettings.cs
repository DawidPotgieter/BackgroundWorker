using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "QueueSettings", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class QueueSettings
	{
		[DataMember(Name = "Id", IsRequired = true)]
		public byte Id { get; set; }

		[DataMember(Name = "Type", IsRequired = true)]
		public string Type { get; set; }

		[DataMember(Name = "ThreadCount", IsRequired = true)]
		public uint ThreadCount { get; set; }
	}
}
