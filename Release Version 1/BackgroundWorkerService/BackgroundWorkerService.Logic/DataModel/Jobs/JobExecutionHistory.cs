using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.DataModel.Jobs
{
	/// <summary>
	/// Contains a job execution history record data.
	/// </summary>
	public class JobExecutionHistory
	{
		/// <summary>
		/// Gets or sets the id of the job execution history record.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		public long Id { get; set; }
		/// <summary>
		/// Gets the id of the job that was executed.
		/// </summary>
		/// <value>The auto generated job id.</value>
		public long JobId { get; set; }
		/// <summary>
		/// Gets the job data at the time of execution.
		/// </summary>
		/// <value>The job data.</value>
		public string Data { get; set; }
		/// <summary>
		/// Gets the job meta data at time of execution.
		/// </summary>
		/// <value>The job meta data.</value>
		public string MetaData { get; set; }
		/// <summary>
		/// Gets the error message (if any) associated with this execution of the job.
		/// </summary>
		public string ErrorMessage { get; set; }
		/// <summary>
		/// Gets the name of the BackgroundWorkerInstance Service instance that executed this job.
		/// </summary>
		public string Instance { get; set; }
		/// <summary>
		/// Gets the DateTime when this job was created.
		/// </summary>
		public DateTime CreatedDate { get; set; }
		/// <summary>
		/// Gets the DateTime when this job started executing.
		/// </summary>
		/// <remarks>
		/// Even though it's a nullable DateTime, the value will be populated.
		/// </remarks>
		public DateTime? StartTime { get; set; }
		/// <summary>
		/// Gets the DateTime when the job execution finished.
		/// </summary>
		/// <remarks>
		/// Even though it's a nullable DateTime, the value will be populated.
		/// </remarks>
		public DateTime? EndTime { get; set; }
		/// <summary>
		/// Gets the absolute timeout value as at execution time.
		/// </summary>
		public TimeSpan? AbsoluteTimeout { get; set; }
		/// <summary>
		/// Gets the status that the job was in after this execution.
		/// </summary>
		public JobStatus Status { get; set; }
		/// <summary>
		/// Gets which queue that was used to execute this job.
		/// </summary>
		public int QueueId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="JobExecutionHistory"/> executed without errors.
		/// </summary>
		/// <value>
		///   <c>true</c> if no errors occurred and the job returned a success value; otherwise, <c>false</c>.
		/// </value>
		public bool Success { get; set; }

		/// <summary>
		/// Gets or sets the type of the job that was executed.
		/// </summary>
		/// <value>
		/// The type of the job.
		/// </value>
		public Type JobType { get; internal set; }

		/// <summary>
		/// Gets or sets the application name as at execution time.
		/// </summary>
		/// <value>
		/// The application.
		/// </value>
		public string Application { get; set; }

		/// <summary>
		/// Gets or sets the group name as at execution time.
		/// </summary>
		/// <value>
		/// The group.
		/// </value>
		public string Group { get; set; }

		/// <summary>
		/// Gets or sets the name as at execution time.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description as at execution time.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description { get; set; }
	}
}
