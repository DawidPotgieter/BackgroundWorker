using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.BackgroundWorkerService.Service;
using WebUI.Code;

namespace WebUI
{
	public partial class JobHistory : System.Web.UI.Page
	{
		protected long JobHistoryId { get; set; }

		private JobExecutionHistory jobHistory;

		private static JobExecutionHistory GetJobHistory(long jobHistoryId)
		{
			JobExecutionHistory jobHistory = null;
			using (AccessPointClient accessPoint = new AccessPointClient())
			{
				jobHistory = accessPoint.GetJobExecutionHistories(new GetJobExecutionHistoriesRequest
				{
					Skip = 0,
					Take = 1,
					Ids = new long[] { jobHistoryId },
				}).JobHistories.FirstOrDefault();
			}
			return jobHistory;
		}

		private static long GetJobHistoryIdFromQueryString()
		{
			long jobHistoryId;
			if (!long.TryParse(HttpContext.Current.Request.QueryString["Id"], out jobHistoryId))
			{
				throw new ArgumentException("Job history id could not be parsed from querystring.");
			}
			return jobHistoryId;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			JobHistoryId = GetJobHistoryIdFromQueryString();
			jobHistory = GetJobHistory(JobHistoryId);
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			lblId.Text = jobHistory.Id.ToString();
			lblApplication.Text = jobHistory.Application;
			lblData.Text = HttpUtility.HtmlEncode(jobHistory.Data);
			lblDescription.Text = jobHistory.Description;
			lblEnd.Text = jobHistory.EndTime.Value.ToString("dd-MM-yyyy HH:mm:ss");
			lblErrorMessage.Text = HttpUtility.HtmlEncode(jobHistory.ErrorMessage);
			lblGroup.Text = jobHistory.Group;
			lblInstance.Text = jobHistory.Instance;
			lblJobId.Text = jobHistory.JobId.ToString();
			lblJobType.Text = jobHistory.JobType;
			lblMetaData.Text = HttpUtility.HtmlEncode(jobHistory.MetaData);
			lblName.Text = jobHistory.Name;
			lblQueueId.Text = jobHistory.QueueId.ToString();
			lblStart.Text = jobHistory.StartTime.Value.ToString("dd-MM-yyyy HH:mm:ss");
			lblStatus.Text = jobHistory.Status.ToString();
			lblStatus.Style["color"] = Utility.GetStatusColor(jobHistory.Status);
			lblSuccess.Text = jobHistory.Success.ToString();
			lblUniqueId.Text = jobHistory.JobUniqueId.ToString();
		}
	}
}