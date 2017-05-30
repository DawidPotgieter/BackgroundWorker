using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "QueueStatus", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class QueueStatus
	{
		[DataMember(Name = "Id", IsRequired = true)]
		public byte Id { get; set; }

		[DataMember(Name = "RunningJobs", IsRequired = true)]
		public uint RunningJobs { get; set; }
	}
}
