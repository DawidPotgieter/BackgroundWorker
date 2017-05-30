using System;
using System.Collections.Generic;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;

namespace BackgroundWorkerService.Logic.DataModel.Internal.Jobs
{
	/// <summary>
	/// Contains the job information, including an already instantiated concrete (user) implementation of <see cref="IJob"/>.
	/// </summary>
	internal class JobContext
	{
		internal JobContext(IJobManager jobManager, JobData jobData, IJob jobInstance)
		{
			JobManager = jobManager;
			JobData = jobData;
			JobInstance = jobInstance;
		}

		/// <summary>
		/// Gets or sets the job data of this context.
		/// </summary>
		/// <value>
		/// The job data.
		/// </value>
		internal JobData JobData { get; set; }

		/// <summary>
		/// Gets or sets the job instance that has already been instantiated. This is an instance of the user defined job.
		/// </summary>
		/// <value>
		/// The job instance.
		/// </value>
		internal IJob JobInstance { get; set; }

		/// <summary>
		/// Gets or sets the job manager.
		/// </summary>
		/// <value>
		/// The job manager.
		/// </value>
 		internal IJobManager JobManager { get; set; }
	}
}
