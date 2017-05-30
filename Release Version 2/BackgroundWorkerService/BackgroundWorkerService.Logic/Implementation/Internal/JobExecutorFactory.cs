using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces.Internal;
using BackgroundWorkerService.Logic.DataModel.Internal.Jobs;
using BackgroundWorkerService.Logic.Interfaces;

namespace BackgroundWorkerService.Logic.Implementation.Internal
{
	/// <summary>
	/// Manages the creation of <see cref="Interfaces.Internal.IJobExecutor"/> according to the type of job.
	/// </summary>
	internal class JobExecutorFactory : IJobExecutorFactory
	{
		/// <summary>
		/// Gets the job executor.
		/// </summary>
		/// <param name="jobContext">The job context.</param>
		/// <returns></returns>
		public IJobExecutor GetJobExecutor(JobContext jobContext)
		{
			if (jobContext.JobInstance is IJob)
			{
				return new DefaultJobExecutor();
			}
			throw new ArgumentException("jobContext.JobInstance");
		}
	}
}
