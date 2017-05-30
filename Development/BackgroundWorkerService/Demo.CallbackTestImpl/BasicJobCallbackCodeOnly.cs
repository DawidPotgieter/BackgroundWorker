using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BackgroundWorkerService.Jobs.DataModel;
using BackgroundWorkerService.Jobs.DataModel.Response;
using BackgroundWorkerService.Jobs.DataModel.Request;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Jobs.Contracts;

namespace Demo.CallbackTestImpl
{
	/// <summary>
	/// This is an example of a code only (without .svc file) implementation of the interface. Note, you have to specify 
	/// the route in web.config under serviceHostingEnvironment/serviceActivations.
	/// </summary>
	public class BasicJobCallbackCodeOnly : IBasicJobCallback
	{
		/// <summary>
		/// Executes the specified request that was initiated from the remote worker service
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		public BasicJobCallbackResponse Execute(BasicJobCallbackRequest request)
		{
			if (new Random().Next(0, 100) > 50)
			{
				return new BasicJobCallbackResponse
				{
					Result = new JobExecutionResult
					{
						ResultStatus = (string.IsNullOrEmpty(request.MetaData) ? JobResultStatus.FailAutoRetry : JobResultStatus.Success), //Force one retry, since it will be set to random number after first retry.
						MetaData = request.MetaData + " " + new Random().Next().ToString(),
					},
				};
			}
			else
			{
				return new BasicJobCallbackResponse
				{
					Exception = new SerializableException(new Exception("Test Exception")),
				};
			}
		}
	}
}