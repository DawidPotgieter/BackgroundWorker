using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BackgroundWorkerService.Logic.Helpers;

namespace BackgroundWorkerService.Jobs.DataModel
{
	public class WebPostSettings : WebRequestSettings
	{
		public string Content { get; set; }

		public new string Serialize()
		{
			return Utils.SerializeObject<WebPostSettings>(this);
		}
	}
}
