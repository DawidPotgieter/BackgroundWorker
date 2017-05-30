using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.DataModel.Alerts
{
	public class Alert
	{
		public long Id { get; set; }
		public long JobId { get; set; }
		public long? JobHistoryId { get; set; }
		public string Message { get; set; }
	}
}
