using System.Net;

namespace BackgroundWorkerService.Jobs.DataModel
{
	public class WebRequestHeader
	{
		public HttpRequestHeader Header { get; set; }
		public string Value { get; set; }
	}
}