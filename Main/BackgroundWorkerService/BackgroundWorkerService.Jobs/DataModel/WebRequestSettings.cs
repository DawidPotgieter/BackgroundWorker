using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BackgroundWorkerService.Logic.Helpers;

namespace BackgroundWorkerService.Jobs.DataModel
{
	public class WebRequestSettings
	{
		public string Url { get; set; }
		public HttpStatusCode ExpectedResponseCode { get; set; }
		public bool UseDefaultCredentials { get; set; }
		public CredentialType? CredentialType { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Domain { get; set; }
		public int TimeoutMilliseconds { get; set; }
		public List<WebRequestHeader> Headers { get; set; }

		public string Serialize()
		{
			return Utils.SerializeObject<WebRequestSettings>(this);
		}
	}
}
