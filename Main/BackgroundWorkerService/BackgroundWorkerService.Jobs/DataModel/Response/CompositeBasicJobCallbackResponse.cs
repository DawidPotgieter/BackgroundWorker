using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Jobs.DataModel.Response
{
	[DataContract(Name = "CompositeBasicJobCallbackResponse", Namespace = "http://backgroundworkerservice/jobs/DataModel/01/01/12")]
	public class CompositeBasicJobCallbackResponse : Response
	{
	}
}
