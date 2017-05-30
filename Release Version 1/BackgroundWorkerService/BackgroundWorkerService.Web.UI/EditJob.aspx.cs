using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.BackgroundWorkerService.Service;
using System.Web.Services;

namespace WebUI
{
	public partial class Job : System.Web.UI.Page
	{
		private const string dateTimeFormat = "dd/MM/yyyy HH:mm";
		protected long JobId { get; set; }

		private static JobData GetJob(long jobId)
		{
			JobData job = null;
			using (AccessPointClient accessPoint = new AccessPointClient())
			{
				job = accessPoint.GetJobs(new GetJobsRequest
				{
					Skip = 0,
					Take = 1,
					JobIds = new long[] { jobId },
				}).Jobs.FirstOrDefault();
			}
			return job;
		}

		private static long GetJobIdFromQueryString()
		{
			long jobId;
			if (!long.TryParse(HttpContext.Current.Request.QueryString["Id"], out jobId))
			{
				throw new ArgumentException("Job id could not be parsed from querystring.");
			}
			return jobId;
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			JobId = GetJobIdFromQueryString();

			JobData job = GetJob(JobId);
			if (job != null)
			{
				lblId.Text = job.Id.ToString();
				txtName.Text = job.Name;
				txtDescription.Text = job.Description;
				txtAbsoluteTimeout.Text = job.AbsoluteTimeout.ToString();
				txtApplication.Text = job.Application;
				txtData.Text = job.Data;
				txtGroup.Text = job.Group;
				txtMetaData.Text = job.MetaData;
				txtQueueId.Text = job.QueueId.ToString();
				lblCreatedDate.Text = job.CreatedDate.ToString(dateTimeFormat);
				lblInstance.Text = job.Instance;
				lblJobType.Text = job.JobType;
				lblLastEndTime.Text = job.LastEndTime.HasValue ? job.LastEndTime.Value.ToString(dateTimeFormat) : "";
				lblLastErrorMessage.Text = job.LastErrorMessage;
				lblLastStartTime.Text = job.LastStartTime.HasValue ? job.LastStartTime.Value.ToString(dateTimeFormat) : "";
				lblStatus.Text = job.Status.ToString();
				chkSuppressHistory.Checked = job.SuppressHistory;
				chkDeleteWhenDone.Checked = job.DeleteWhenDone;
			}
		}

		[WebMethod]
		public static string UpdateJob(string name, string description, string data, string metaData, string absoluteTimeout, string queueId, string application, string group, bool suppressHistory, bool deleteWhenDone)
		{
			try
			{
				long jobId = GetJobIdFromQueryString();
				JobData job = GetJob(jobId);
				if (job != null)
				{
					job.Name = name;
					job.Description = description;
					job.Data = data;
					job.MetaData = metaData;
					if (!string.IsNullOrEmpty(absoluteTimeout))
					{
						job.AbsoluteTimeout = TimeSpan.Parse(absoluteTimeout);
					}
					if (!string.IsNullOrEmpty(queueId))
					{
						job.QueueId = byte.Parse(queueId);
					}
					job.Application = application;
					job.Group = group;
					job.SuppressHistory = suppressHistory;
					job.DeleteWhenDone = deleteWhenDone;

					using (AccessPointClient accessPoint = new AccessPointClient())
					{
						accessPoint.UpdateJob(new UpdateJobRequest { JobData = job });
					}
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			return string.Empty;
		}
	}
}