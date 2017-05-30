using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Logic.DataModel.Jobs
{
	/// <summary>
	/// The result that should be returned by a call to <see cref="Interfaces.IJob.Execute"/>.
	/// </summary>
	[DataContract(Name = "JobExecutionResult")]
	public class JobExecutionResult
	{
		/// <summary>
		/// Result of the <see cref="Interfaces.IJob.Execute"/> method call.
		/// </summary>
		[DataMember(Name = "ResultStatus")]
		public JobResultStatus ResultStatus { get; set; }
		/// <summary>
		/// Populate this with a complete error message if <see cref="ResultStatus"/> is not <see cref="JobResultStatus.Success"/> instead of throwing exceptions.  This value is persisted to the job datastore.
		/// </summary>
		[DataMember(Name = "ErrorMessage")]
		public string ErrorMessage { get; set; }
		/// <summary>
		/// (Optional) Populate this with job metadata.  This value is persisted to the job datastore and is usable when the job is re-executed.
		/// </summary>
		[DataMember(Name = "MetaData")]
		public string MetaData { get; set; }
	}
}
