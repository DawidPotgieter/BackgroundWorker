using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using BackgroundWorkerService.Logic.Helpers;

namespace BackgroundWorkerService.Jobs.DataModel
{
	public class BasicHttpCallbackJobMetaData
	{
		public ContractType ContractType { get; set; }
		public BasicHttpSecurityMode SecurityMode { get; set; }
		public HttpClientCredentialType TransportCredentialType { get; set; }
		public BasicHttpMessageCredentialType? MessageCredentialType { get; set; }
		public string CallbackUrl { get; set; }
		public string Domain { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string MetaData { get; set; }
		public double TimeoutMilliseconds { get; set; }
		public int MessageSize { get; set; }
		public bool IgnoreCertificateErrors { get; set; }

		public BasicHttpCallbackJobMetaData()
		{
			//Give some defaults in case they're not specified.
			MessageSize = 1024 * 1024 * 4;
			TimeoutMilliseconds = new TimeSpan(1, 0, 0).TotalMilliseconds;
			IgnoreCertificateErrors = false;
		}

		public string Serialize()
		{
			return Utils.SerializeObject<BasicHttpCallbackJobMetaData>(this);
		}
	}
}
