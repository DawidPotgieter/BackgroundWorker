using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUI.BackgroundWorkerService.Service;
using System.Web.Services;
using WebUI.datamodel;
using System.Globalization;

namespace WebUI
{
	public partial class Job : System.Web.UI.Page
	{
		private const string dateTimeFormat = "dd/MM/yyyy HH:mm:ss";
		protected long JobId { get; set; }

		private JobData job;

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

		protected string ScheduleGroupToSelect { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			JobId = GetJobIdFromQueryString();
			job = GetJob(JobId);
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (job != null)
			{
				lblId.Text = job.Id.ToString();
				txtUniqueId.Text = job.UniqueId.ToString();
				txtName.Text = job.Name;
				txtDescription.Text = job.Description;
				txtAbsoluteTimeout.Text = job.AbsoluteTimeout.ToString();
				txtApplication.Text = job.Application;
				txtData.Text = job.Data;
				txtGroup.Text = job.Group;
				txtMetaData.Text = job.MetaData;
				cmbQueueId.SelectedValue = job.QueueId.ToString();
				lblCreatedDate.Text = job.CreatedDate.ToString(dateTimeFormat);
				lblInstance.Text = job.Instance;
				lblJobType.Text = job.JobType;
				lblLastEndTime.Text = job.LastEndTime.HasValue ? job.LastEndTime.Value.ToString(dateTimeFormat) : "";
				lblLastErrorMessage.Text = job.LastErrorMessage;
				lblLastStartTime.Text = job.LastStartTime.HasValue ? job.LastStartTime.Value.ToString(dateTimeFormat) : "";
				lblNextStart.Text = job.NextStartTime.HasValue ? job.NextStartTime.Value.ToString(dateTimeFormat) : "";
				lblStatus.Text = job.Status.ToString();
				cmbSuppressHistory.SelectedValue = job.SuppressHistory.ToString();
				cmbDeleteWhenDone.SelectedValue = job.DeleteWhenDone.ToString();
				if (job.CalendarSchedule != null)
				{
					ScheduleGroupToSelect = "runSchedule";
					runOnce.Checked = false;
					runSchedule.Checked = true;
					scheduleContainer.Style["display"] = "";
					CalendarScheduleControl.SelectedCalendarSchedule = job.CalendarSchedule;
				}
				else
				{
					ScheduleGroupToSelect = "runOnce";
					runOnce.Checked = true;
					runSchedule.Checked = false;
					scheduleContainer.Style["display"] = "none";
				}
			}
		}

		[WebMethod]
		public static string UpdateJob(long jobId, string uniqueId, string name, string description, string data, string metaData, string statusId, string absoluteTimeout, string queueId, string application, string group, string suppressHistory, string deleteWhenDone, SimpleSchedule schedule)
		{
			try
			{
				JobData job = GetJob(jobId);
				if (job != null)
				{
					if (!string.IsNullOrEmpty(statusId))
					{
						job.Status = (JobStatus)(int.Parse(statusId));
					}

					job.UniqueId = Guid.Parse(uniqueId);
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
					job.SuppressHistory = bool.Parse(suppressHistory);
					job.DeleteWhenDone = bool.Parse(deleteWhenDone);

					if (schedule != null)
					{
						if (!schedule.StartDailyAt.HasValue) schedule.StartDailyAt = new TimeSpan();
						job.CalendarSchedule = new CalendarSchedule
						{
							ScheduleType = typeof(global::BackgroundWorkerService.Logic.DataModel.Scheduling.CalendarSchedule).AssemblyQualifiedName,
							DaysOfWeek = schedule.DaysOfWeek.ToArray(),
							StartDailyAt = new TimeOfDay { Hour = schedule.StartDailyAt.Value.Hours, Minute = schedule.StartDailyAt.Value.Minutes, Second = schedule.StartDailyAt.Value.Seconds },
							RepeatInterval = schedule.RepeatInterval,
							EndDateTime = null,
							StartDateTime = !string.IsNullOrEmpty(schedule.StartDateTime) ? DateTime.ParseExact(schedule.StartDateTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.CurrentCulture) : job.CalendarSchedule.StartDateTime,
						};
					}
					else
					{
						job.CalendarSchedule = null;
					}

					using (AccessPointClient accessPoint = new AccessPointClient())
					{
						accessPoint.UpdateJob(new UpdateJobRequest { JobData = job });
					}
				}
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
			return string.Empty;
		}
	}
}