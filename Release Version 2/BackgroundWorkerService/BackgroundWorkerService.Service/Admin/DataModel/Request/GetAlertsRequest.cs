using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Request
{
	[DataContract(Name = "GetAlertsRequest", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class GetAlertsRequest
	{
		[DataMember(Name = "Skip", IsRequired = true)]
		public uint Skip { get; set; }

		[DataMember(Name = "Take", IsRequired = true)]
		public uint Take { get; set; }
	}
}
