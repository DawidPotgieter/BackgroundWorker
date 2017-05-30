using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "Alert", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class Alert
	{
		internal Alert(Logic.DataModel.Alerts.Alert alert)
		{
			this.Id = alert.Id;
			this.JobId = alert.JobId;
			this.JobHistoryId = alert.JobHistoryId;
			this.Message = alert.Message;
		}

		internal Logic.DataModel.Alerts.Alert AsInternalJobData()
		{
			return new Logic.DataModel.Alerts.Alert
			{
				Id = this.Id,
				JobId = this.JobId,
				JobHistoryId = this.JobHistoryId,
				Message = this.Message,
			};
		}

		[DataMember(Name = "Id", IsRequired = true)]
		public long Id { get; set; }

		[DataMember(Name = "JobId", IsRequired = true)]
		public long JobId { get; set; }

		[DataMember(Name = "JobHistoryId", IsRequired = true)]
		public long? JobHistoryId { get; set; }

		[DataMember(Name = "Message", IsRequired = true)]
		public string Message { get; set; }
	}
}
