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

		public string ApplicationName { get; set; }


		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				btnRefresh.OnClientClick = $"refreshJobsList{ApplicationName}();return false;";
				lblLastRefresh.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
			}

			var selectedStatuses = GetSelectedStatuses().ToArray();

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
						}).Jobs;
						jobs.AddRange(page);
					}
					else
					{
						page = accessPoint.GetJobs(new GetJobsRequest
						{
							Skip = (uint)jobs.Count,
							Take = PageSize,
						}).Jobs;
						jobs.AddRange(page.Where(j => string.IsNullOrWhiteSpace(j.Application)));
					}
				}
				while (page != null && page.Length == PageSize);

				foreach (string groupName in jobs.Select(j => j.Group).Distinct())
				{
					var control = (JobsGroup)Page.LoadControl("~/UserControls/JobsGroup.ascx");
					control.ID = "JobsGroup_" + ApplicationName + "_" + groupName;
					control.ApplicationName = ApplicationName;
					control.GroupName = groupName;
					control.PageSize = PageSize;
					control.ShownStatuses = selectedStatuses.ToList();
					JobGridsContainer.Controls.Add(control);
				}
			}
		}

		protected void btnRefresh_Click(object sender, ImageClickEventArgs e)
		{
			foreach (JobsGroup jobsGroup in JobGridsContainer.Controls.OfType<JobsGroup>())
			{
				jobsGroup.ShownStatuses = GetSelectedStatuses();
				jobsGroup.ShowJobs();
			}
			lblLastRefresh.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
		}

		private List<JobStatus> GetSelectedStatuses()
		{
			List<CheckBox> statusChecked = new List<CheckBox>
			{
				{ Done},
				{ Ready },
				{ Scheduled },
				{ Executing },
				{ Deleted },
				{ FailRetry },
				{ FailAutoRetry },
				{ ExecutionTimeout },
				{ Pending },
				{ Queued },
				{ ShutdownTimeout },
				{ Fail },
			};
			return statusChecked.Where(sc => sc.Checked).Select(sc => (JobStatus)Enum.Parse(typeof(JobStatus), sc.ID)).ToList();
		}

		protected void StatusCheckChanged(object sender, EventArgs e)
		{
			foreach (JobsGroup jobsGroup in JobGridsContainer.Controls.OfType<JobsGroup>())
			{
				jobsGroup.ShownStatuses = GetSelectedStatuses();
				jobsGroup.ShowJobs();
			}
		}
	}
}