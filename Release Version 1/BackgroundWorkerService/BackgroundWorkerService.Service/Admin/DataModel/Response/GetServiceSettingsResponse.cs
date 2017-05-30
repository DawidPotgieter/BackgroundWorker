using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "GetServiceSettingsResponse", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class GetServiceSettingsResponse
	{
		[DataMember(Name = "Settings", IsRequired = true)]
		public Settings Settings { get; set; }
	}
}
