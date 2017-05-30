using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUI.BackgroundWorkerService.Service;

namespace WebUI.Code
{
	public class Utility
	{
		public static string GetStatusColor(JobStatus jobStatus)
		{
			switch (jobStatus)
			{
				case JobStatus.Done:
					return "Green";
				case JobStatus.Executing:
					return "Orange";
				case JobStatus.Deleted:
					return "Grey";
				case JobStatus.ExecutionTimeout:
				case JobStatus.Fail:
				case JobStatus.FailAutoRetry:
				case JobStatus.FailRetry:
				case JobStatus.ShutdownTimeout:
					return "Red";
				case JobStatus.Pending:
					return "Yellow";
				case JobStatus.Queued:
				case JobStatus.Queuing:
					return "Blue";
				default:
					return "White";
			}
		}

	}
}