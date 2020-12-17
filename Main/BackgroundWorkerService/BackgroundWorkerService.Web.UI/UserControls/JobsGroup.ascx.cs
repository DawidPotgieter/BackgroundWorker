using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.BackgroundWorkerService.Service;

namespace WebUI.UserControls
{
	public partial class JobsGroup : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ShowJobs();
				GroupNameHeader.Text = GroupName;
			}
		}

		public string ApplicationName { get; set; }

		public string GroupName { get; set; }

		public uint PageSize { get; set; }

		public List<JobStatus> ShownStatuses { get; set; }

		public void ShowJobs()
		{
			using (AccessPointClient accessPoint = new AccessPointClient())
			{
				List<JobData> jobs = new List<JobData>();
				JobData[] page;
				do
				{
					if (!string.IsNullOrWhiteSpace(ApplicationName))
					{
						page = accessPoint.GetJobs(new GetJobsRequest
						{
							Skip = (uint)jobs.Count,
							Take = PageSize,
							Applications = new string[] { ApplicationName },
							JobStatuses = ShownStatuses.ToArray(),
						}).Jobs;
						jobs.AddRange(page);
					}
					else
					{
						page = accessPoint.GetJobs(new GetJobsRequest
						{
							Skip = (uint)jobs.Count,
							Take = PageSize,
							JobStatuses = ShownStatuses.ToArray(),
						}).Jobs;
						jobs.AddRange(page.Where(j => string.IsNullOrWhiteSpace(j.Application)));
					}
				}
				while (page != null && page.Length == PageSize);

				jobs = jobs.Where(j => string.IsNullOrWhiteSpace(GroupName) ? j.Group == null || j.Group == "" : j.Group == GroupName).ToList();

				JobsList.DataSource = jobs;
				JobsList.DataBind();
				JobsListPager.Visible = jobs.Count > JobsListPager.MaximumRows;
			}
		}

		protected void JobsList_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			if (e.Item.ItemType == ListViewItemType.DataItem)
			{
				JobData job = (JobData)e.Item.DataItem;
				JobHistory history = e.Item.FindControl("jobHistoryControl") as JobHistory;
				Image runButtonImage = e.Item.FindControl("btnRunJobImage") as Image;
				Image deleteButtonImage = e.Item.FindControl("btnDeleteJobImage") as Image;
				Image statusIcon = e.Item.FindControl("StatusIcon") as Image;

				history.JobId = job.Id;
				runButtonImage.Attributes["onclick"] = string.Format("queryRunJob({0},'" + ApplicationName + "');return false;", job.Id);
				deleteButtonImage.Attributes["onclick"] = string.Format("queryDeleteJob({0}, '" + ApplicationName + "');return false;", job.Id);

				switch ((JobStatus)job.Status)
				{
					case JobStatus.Done:
						statusIcon.ImageUrl = "~/content/images/StatusOK_16x.png";
						break;
					case JobStatus.Pending:
						statusIcon.ImageUrl = "~/content/images/Hourglass_16x.png";
						break;
					case JobStatus.Queued:
						statusIcon.ImageUrl = "~/content/images/BuildQueueCircle_16x.png";
						break;
					case JobStatus.Ready:
						statusIcon.ImageUrl = "~/content/images/StatusReady_16x.png";
						break;
					case JobStatus.Scheduled:
						statusIcon.ImageUrl = "~/content/images/Time_16x.png";
						break;
					case JobStatus.Executing:
						statusIcon.ImageUrl = "~/content/images/Execute_16x.png";
						break;
					case JobStatus.Fail:
					case JobStatus.FailRetry:
						statusIcon.ImageUrl = "~/content/images/StatusInvalid_16x.png";
						break;
					case JobStatus.FailAutoRetry:
						statusIcon.ImageUrl = "~/content/images/StatusSuppressed_16x.png";
						break;
					case JobStatus.ShutdownTimeout:
						statusIcon.ImageUrl = "~/content/images/StopTime_16x.png";
						break;
					case JobStatus.ExecutionTimeout:
						statusIcon.ImageUrl = "~/content/images/Timeout_16x.png";
						break;
					default:
						statusIcon.ImageUrl = "~/content/images/Job.png";
						break;
				}
			}
		}

		protected void JobsList_PagePropertiesChanged(object sender, EventArgs e)
		{
			ShowJobs();
		}
	}
}