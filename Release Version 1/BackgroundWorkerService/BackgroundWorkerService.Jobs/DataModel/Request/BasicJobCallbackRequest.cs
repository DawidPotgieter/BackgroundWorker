using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Jobs.DataModel.Request
{
	[DataContract(Name = "BasicJobCallbackRequest", Namespace = "http://backgroundworkerservice/jobs/DataModel/01/01/12")]
	public class BasicJobCallbackRequest : Request
	{
	}
}
