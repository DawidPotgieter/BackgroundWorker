using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "GetAlertsResponse", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class GetAlertsResponse
	{
		[DataMember(Name = "Alerts", IsRequired = true)]
		public List<Alert> Alerts { get; set; }
	}
}
