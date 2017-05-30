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
	/// This interface is used if you'll have a webservice with multiple callback jobs.  You have to use the MethodName in request to distinguish what to call on client side.
	/// </summary>
	[ServiceContract(Name = "CompositeJobCallback", Namespace = "http://backgroundworkerservice/jobs/01/01/12")]
	public interface ICompositeJobCallback
	{
		[OperationContract(Name = "Execute")]
		CompositeBasicJobCallbackResponse Execute(CompositeBasicJobCallbackRequest request);
	}
}
