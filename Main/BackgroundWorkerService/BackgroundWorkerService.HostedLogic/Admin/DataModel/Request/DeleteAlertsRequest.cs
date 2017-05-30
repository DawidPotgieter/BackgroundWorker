using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Request
{
	[DataContract(Name = "DeleteAlertsRequest", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class DeleteAlertsRequest
	{
		[DataMember(Name = "Ids", IsRequired = true)]
		public List<long> Ids { get; set; }
	}
}
