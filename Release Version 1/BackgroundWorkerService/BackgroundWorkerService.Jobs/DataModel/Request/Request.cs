using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Jobs.DataModel.Request
{
	[DataContract(Name = "Request", Namespace = "http://backgroundworkerservice/jobs/DataModel/01/01/12")]
	public class Request
	{
		[DataMember(Name = "Data", IsRequired = true)]
		public string Data { get; set; }

		[DataMember(Name = "MetaData", IsRequired = true)]
		public string MetaData { get; set; }
	}
}
