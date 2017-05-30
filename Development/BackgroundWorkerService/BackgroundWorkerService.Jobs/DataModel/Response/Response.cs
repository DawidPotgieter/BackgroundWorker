using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Jobs.DataModel.Response
{
	[DataContract(Name = "Response", Namespace = "http://backgroundworkerservice/jobs/DataModel/01/01/12")]
	public class Response
	{
		[DataMember(Name = "Result", IsRequired = false)]
		public JobExecutionResult Result { get; set; }

		[DataMember(Name = "SerializableException", IsRequired = false)]
		public SerializableException Exception { get; set; }
	}
}
