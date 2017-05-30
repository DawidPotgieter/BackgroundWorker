using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Jobs.DataModel.Request
{
	[DataContract(Name = "CompositeBasicJobCallbackRequest", Namespace = "http://backgroundworkerservice/jobs/DataModel/01/01/12")]
	public class CompositeBasicJobCallbackRequest : Request
	{
		[DataMember(Name = "MethodName", IsRequired = true)]
		public string MethodName { get; set; }
	}
}
