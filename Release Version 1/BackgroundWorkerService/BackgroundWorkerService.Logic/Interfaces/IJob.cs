using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.DataModel.Jobs;

namespace BackgroundWorkerService.Logic.Interfaces
{
	/// <summary>
	/// Interface that has to be implemented by each background job.
	/// </summary>
	public interface IJob
	{
		/// <summary>
		/// This method will be called when the background worker service executes the job.
		/// </summary>
		/// <param name="jobData">The data provided by the consumer that created the job.</param>
		/// <param name="metaData">Optional data provided by the consumer that created the job.  This is usually used when re-executing a job or wrapping the job inside another job.</param>
		/// <returns>
		/// This method should preferably never throw any exceptions.
		/// Instead, return <see cref="JobResultStatus.Fail"/> as the value for <see cref="JobExecutionResult.ResultStatus"/> in the return value.
		/// If an exception is thrown or the return value is null, the service will not fail, but it will
		/// be more difficult for the consumer to pinpoint the exact failure (persisted to the Job datastore).
		/// </returns>
		JobExecutionResult Execute(string jobData, string metaData);
	}
}
