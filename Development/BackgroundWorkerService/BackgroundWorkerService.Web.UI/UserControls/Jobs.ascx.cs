using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.BackgroundWorkerService.Service;
using System.Web.UI.HtmlControls;
using System.Web.Services;

namespace WebUI.UserControls
{
	public partial class Jobs : System.Web.UI.UserControl
	{
		private const uint PageSize = 100;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ShowJobs();
			}
		}

		protected void JobsListPager_PreRender(object sender, EventArgs e)
		{
		}

		private void ShowJobs()
		{
			using (AccessPointClient accessPoint = new AccessPointClient())
			{
				List<JobData> jobs = new List<JobData>();
				JobData[] page;
				do
				{
					page = accessPoint.GetJobs(new GetJobsRequest
					{
						Skip = (uint)jobs.Count,
						Take = PageSize,
					}).Jobs;
					jobs.AddRange(page);
				}
				while (page != null && page.Length == PageSize);
				JobsList.DataSource = jobs;
				JobsList.DataBind();
				JobsListPager.Visible = jobs.Count > JobsListPager.MaximumRows;
			}
			lblLastRefresh.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
		}

		protected void JobsList_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			if (e.Item.ItemType == ListViewItemType.DataItem)
			{
				JobData job = (JobData)e.Item.DataItem;
				JobHistory history = e.Item.FindControl("jobHistoryControl") as JobHistory;
				Image runButtonImage = e.Item.FindControl("btnRunJobImage") as Image;
				Image deleteButtonImage = e.Item.FindControl("btnDeleteJobImage") as Image;

				history.JobId = job.Id;
				runButtonImage.Attributes["onclick"] = string.Format("queryRunJob({0});return false;", job.Id);
				deleteButtonImage.Attributes["onclick"] = string.Format("queryDeleteJob({0});return false;", job.Id);
			}
		}

		protected void JobsList_PagePropertiesChanged(object sender, EventArgs e)
		{
			ShowJobs();
		}

		protected void btnRefresh_Click(object sender, ImageClickEventArgs e)
		{
			ShowJobs();
		}
	}
}