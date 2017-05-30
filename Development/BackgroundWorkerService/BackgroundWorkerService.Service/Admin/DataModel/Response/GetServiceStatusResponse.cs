using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "GetServiceStatusResponse", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class GetServiceStatusResponse
	{
		[DataMember(Name = "ServiceStatus", IsRequired = true)]
		public ServiceStatus ServiceStatus { get; set; }
	}
}
