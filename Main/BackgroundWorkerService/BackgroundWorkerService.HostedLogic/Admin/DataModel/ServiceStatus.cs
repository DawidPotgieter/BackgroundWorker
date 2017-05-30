using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel
{
	[DataContract(Name = "ServiceStatus", Namespace = "http://backgroundworkerservice/DataModel/01/04/12")]
	public class ServiceStatus
	{
		internal ServiceStatus(ServiceHost host)
		{
			var query = from q in ((AccessPoint)host.SingletonInstance).workerService.JobQueues
									select new QueueStatus
									{
										Id = q.Id,
										RunningJobs = q.ActiveThreads,
									};
			Queues = query.ToList();
			Status = ((AccessPoint)host.SingletonInstance).Status;
		}

		[DataMember(Name = "Queues", IsRequired = true)]
		public List<QueueStatus> Queues { get; set; }

		[DataMember(Name = "Status", IsRequired = true)]
		public string Status { get; set; }
	}
}
