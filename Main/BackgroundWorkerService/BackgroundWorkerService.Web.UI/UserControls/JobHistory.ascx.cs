using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.BackgroundWorkerService.Service;

namespace WebUI.UserControls
{
	public partial class JobHistory : System.Web.UI.UserControl
	{
		private const uint HistoryCount = 10;
		internal long JobId { get; set; }

		protected void Page_PreRender(object sender, EventArgs e)
		{
			this.Visible = false;
			ShowExecutionHistories();
		}

		private void ShowExecutionHistories()
		{
			try
			{
				using (AccessPointClient accessPoint = new AccessPointClient())
				{
					JobExecutionHistory[] jobHistories = accessPoint.GetJobExecutionHistories(new GetJobExecutionHistoriesRequest
					{
						Skip = 0,
						Take = HistoryCount,
						JobIds = new long[] { JobId },
					}).JobHistories;

					this.Visible = true;
					JobHistoryList.DataSource = jobHistories;
					JobHistoryList.DataBind();
				}
			}
			catch { }
		}
	}
}