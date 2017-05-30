using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BackgroundWorkerService.Jobs.Contracts;
using BackgroundWorkerService.Jobs.DataModel.Response;
using BackgroundWorkerService.Jobs.DataModel.Request;
using BackgroundWorkerService.Logic.DataModel.Jobs;

namespace Demo.CallbackTestImpl
{
	public class CompositeJobCallback : ICompositeJobCallback 
	{
		private JobExecutionResult SomeMethod(string data, string metaData)
		{
			return new JobExecutionResult
			{
				ResultStatus = JobResultStatus.Success,
				MetaData = metaData,
			};
		}

		public CompositeBasicJobCallbackResponse Execute(CompositeBasicJobCallbackRequest request)
		{
			CompositeBasicJobCallbackResponse response = new CompositeBasicJobCallbackResponse { Result = new JobExecutionResult { ResultStatus = JobResultStatus.Fail, MetaData = request.MetaData } };

			try
			{
				switch (request.MethodName)
				{
					case "SomeMethod":
						response.Result = SomeMethod(request.Data, request.MetaData);
						break;
				}
			}
			catch (Exception ex)
			{
				response.Result.ErrorMessage = ex.Message;
				response.Exception = new BackgroundWorkerService.Jobs.DataModel.SerializableException(ex);
			}

			return response;
		}
	}
}
