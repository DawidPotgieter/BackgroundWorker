using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BackgroundWorkerService.Jobs.DataModel.Response;
using BackgroundWorkerService.Jobs.DataModel.Request;

namespace BackgroundWorkerService.Jobs.Contracts
{
	/// <summary>
	/// This interface is used if you'll have a webservice with a single callback job.
	/// </summary>
	[ServiceContract(Name = "BasicJobCallback", Namespace = "http://backgroundworkerservice/jobs/01/01/12")]
	public interface IBasicJobCallback
	{
		[OperationContract(Name = "Execute")]
		BasicJobCallbackResponse Execute(BasicJobCallbackRequest request);
	}
}
