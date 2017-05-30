using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.BackgroundWorkerService.Service;
using System.Web.UI.HtmlControls;

namespace WebUI.UserControls
{
	public partial class Jobs : System.Web.UI.UserControl
	{
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
				JobData[] jobs = accessPoint.GetJobs(new GetJobsRequest
				{
					Skip = 0,
					Take = int.MaxValue,
				}).Jobs;

				JobsList.DataSource = jobs;
				JobsList.DataBind();
				JobsListPager.Visible = jobs.Length > JobsListPager.MaximumRows;
			}
		}

		protected void JobsList_ItemCommand(object sender, ListViewCommandEventArgs e)
		{
			long jobId;
			if (long.TryParse((string)e.CommandArgument, out jobId))
			{
				switch (e.CommandName.ToUpper())
				{
					case "RUN":
						using (AccessPointClient accessPoint = new AccessPointClient())
						{
							JobData job = accessPoint.GetJobs(new GetJobsRequest
							{
								Skip = 0,
								Take = 1,
								JobIds = new long[] { jobId },
							}).Jobs.FirstOrDefault();

							if (job != null)
							{
								if (job.CalendarSchedule == null)
								{
									job.Status = JobStatus.Ready;
									accessPoint.UpdateJob(new UpdateJobRequest { JobData = job });
								}
								else
								{
									accessPoint.RunScheduledJob(new RunScheduledJobRequest { JobId = jobId });
								}
							}
						}
						break;
					case "DELETE":
						using (AccessPointClient accessPoint = new AccessPointClient())
						{
							accessPoint.DeleteJob(new DeleteJobRequest { JobId = jobId });
						}
						break;
				}
			}
		}

		protected void JobsList_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			if (e.Item.ItemType == ListViewItemType.DataItem)
			{
				JobData job = (JobData)e.Item.DataItem;
				JobHistory history = e.Item.FindControl("jobHistoryControl") as JobHistory;
				history.JobId = job.Id;
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