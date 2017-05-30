using System;
using System.Collections.Generic;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;

namespace BackgroundWorkerService.Logic.TestJobs
{
	/// <summary>
	/// An implementation of a background worker job to simulate an infinite running job.
	/// This job will never finish and simply force the thread to sleep indefinitely.
	/// </summary>
	public class InfiniteRunningJob : IJob
	{
		#region IBackgroundWorker Members

		/// <summary>
		/// This method simply puts the thread into infinite sleeping condition to simulate a 
		/// job that never finishes.
		/// </summary>
		/// <param name="jobData">Ignored</param>
		/// <param name="metaData">Ignored</param>
		/// <returns>
		/// This method never returns.
		/// </returns>
		public JobExecutionResult Execute(string jobData, string metaData)
		{
			while (true) System.Threading.Thread.Sleep(1000);
		}
		#endregion
	}
}
