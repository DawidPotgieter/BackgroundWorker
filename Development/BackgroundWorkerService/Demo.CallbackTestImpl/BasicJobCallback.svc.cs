﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BackgroundWorkerService.Jobs.DataModel;
using BackgroundWorkerService.Jobs.DataModel.Response;
using BackgroundWorkerService.Jobs.DataModel.Request;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Jobs.Contracts;

namespace Demo.CallbackTestImpl
{
	public class BasicJobCallback : IBasicJobCallback
	{
		public BasicJobCallbackResponse Execute(BasicJobCallbackRequest request)
		{
			//System.Threading.Thread.Sleep(new Random().Next(3) * 1000);
			//System.Threading.Thread.Sleep(1000);
			return new BasicJobCallbackResponse
			{
				Result = new JobExecutionResult
				{
					ResultStatus = JobResultStatus.Success,
					MetaData = request.MetaData + " " + new Random().Next().ToString(),
				},
			};
		}
	}
}
