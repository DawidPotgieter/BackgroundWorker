using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;

namespace BackgroundWorkerService.Logic.TestJobs
{
	/// <summary>
	/// An implementation of a background job that simulates a short running job.
	/// </summary>
	public class ShortRunningJob : IJob
	{
		#region IBackgroundWorker Members

		/// <summary>
		/// This timerjob will run simulate a short running job that optionally returns
		/// failures and recovery data.
		/// </summary>
		/// <param name="jobData">Ignored.</param>
		/// <param name="metaData">Ignored.</param>
		/// <returns>
		/// The method either takes between 2-5 seconds to return <see cref="JobResultStatus.Success"/> or
		/// randomly returns <see cref="JobResultStatus.Fail"/> or <see cref="JobResultStatus.FailRetry"/>.
		/// </returns>
		public JobExecutionResult Execute(string jobData, string metaData)
		{
			JobExecutionResult result = new JobExecutionResult();

			result.ResultStatus = JobResultStatus.Success;
			result.MetaData = "";

			Random rnd = new Random();
			int random = rnd.Next(0, 100);
			if (random <= 10)
			{
				result.ResultStatus = JobResultStatus.Fail;
				result.ErrorMessage = "Random Error Margin : " + random;
				random = rnd.Next(0, 100);
				if (random <= 50)
				{
					result.ResultStatus = JobResultStatus.FailRetry;
					result.MetaData = "Recovery Data Margin : " + random;
				}
			}
			else if (random <= 15)
			{
				throw new Exception("Exception with Error Margin : " + random);
			}
			else
			{
				System.Threading.Thread.Sleep(rnd.Next(2000, 5000));
			}

			return result;
		}

		#endregion
	}
}
