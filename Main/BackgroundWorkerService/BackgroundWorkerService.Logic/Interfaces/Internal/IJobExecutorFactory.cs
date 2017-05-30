using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.DataModel.Internal.Jobs;

namespace BackgroundWorkerService.Logic.Interfaces.Internal
{
	internal interface IJobExecutorFactory
	{
		IJobExecutor GetJobExecutor(JobContext jobContext);
	}
}
