using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Jobs.DataModel
{
	public class MailSettings
	{
		public string SmtpServer { get; set; }
		public int Port { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool EnableSsl { get; set; }
	}
}
