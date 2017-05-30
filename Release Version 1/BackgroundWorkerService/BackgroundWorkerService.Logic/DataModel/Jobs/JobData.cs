using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.DataModel.Scheduling;

namespace BackgroundWorkerService.Logic.DataModel.Jobs
{
	/// <summary>
	/// Contains all the information that is known about a job before execution time.
	/// </summary>
	public class JobData
	{
		/// <summary>
		/// Gets the auto generated job id.
		/// </summary>
		/// <value>The auto generated job id.</value>
		public long Id { get; internal set; }
		/// <summary>
		/// Gets or sets the type of the job.
		/// </summary>
		/// <value>
		/// The type of the job.
		/// </value>
		public Type JobType { get; internal set; }
		/// <summary>
		/// Gets the job data.
		/// </summary>
		/// <value>The job data.</value>
		public string Data { get; set; }
		/// <summary>
		/// Gets or sets the job meta data.
		/// </summary>
		/// <value>The job meta data.</value>
		public string MetaData { get; set; }
		/// <summary>
		/// Gets or sets the error message (if any) associated with this job.
		/// </summary>
		/// <value>ErrorMessage will be set if the job did not return a success result.</value>
		public string LastErrorMessage { get; set; }
		/// <summary>
		/// Gets the name of the BackgroundWorkerInstance Service instance that has picked up this job.
		/// </summary>
		public string Instance { get; internal set; }
		/// <summary>
		/// Gets the DateTime when this job was created.
		/// </summary>
		public DateTime CreatedDate { get; internal set; }
		/// <summary>
		/// Gets the DateTime when this job started executing.
		/// </summary>
		/// <value>
		/// A null value means the job has not been started.  
		/// This value will be non-null if a job as started executing at least once.
		/// Value is reset when job starts to execute.
		/// </value>
		public DateTime? LastStartTime { get; internal set; }
		/// <summary>
		/// Gets the DateTime when the job execution finished.
		/// </summary>
		/// <value>
		/// A null value means the job has not once finished execution. 
		/// This value will be non-null if a job as finished executing at least once.
		/// A non-null value does not indicate success, simply that the job has finished executing at least once.
		/// Value is reset when job starts to execute.
		/// </value>
		public DateTime? LastEndTime { get; internal set; }
		/// <summary>
		/// Gets or sets the time that this job is allowed to run, before being forcefully terminated.
		/// </summary>
		/// <value>
		/// A null value means that the job will never forcefully be terminated (except perhaps on service shutdown).
		/// </value>
		public TimeSpan? AbsoluteTimeout { get; set; }
		/// <summary>
		/// Gets the current status of this job.
		/// </summary>
		/// <value>
		/// This value changes depending on where in the execution lifecycle the job is currently.
		/// </value>
		public JobStatus Status { get; internal set; }
		/// <summary>
		/// Gets or sets which queue will be used to execute this job.
		/// </summary>
		public int QueueId { get; set; }
		/// <summary>
		/// Gets or sets an optional schedule.  If not set, the job is assumbed to be a once off job and will be executed as soon as possible.
		/// </summary>
		/// <value>
		/// The schedule that this job will be executed on.
		/// </value>
		public ISchedule Schedule { get; set; }
		/// <summary>
		/// Gets or sets the optional application.
		/// </summary>
		/// <value>
		/// The application name - used as a way to distinguish jobs in the same jobstore.
		/// </value>
		public string Application { get; set; }

		/// <summary>
		/// Gets or sets the optional group.
		/// </summary>
		/// <value>
		/// The group name - used as a way to distinguish jobs in the same jobstore and application.
		/// </value>
		public string Group { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether history records are created or not.
		/// </summary>
		/// <value>
		///   If you do not wish a <see cref="JobExecutionHistory"/> record to be created, set this value to <c>true</c>.
		/// </value>
		public bool SuppressHistory { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the job is permanently deleted after a successfull (<see cref="JobStatus.Done"/>) execution.
		/// </summary>
		/// <value>
		///   Set to <c>true</c> if you do not want the job to be in the database if it completed successfully.
		/// </value>
		public bool DeleteWhenDone { get; set; }

		/// <summary>
		/// Gets or sets the optional descriptive name of the job.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the optional description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description { get; set; }
	}
}
