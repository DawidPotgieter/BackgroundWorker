using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using WebUI.BackgroundWorkerService.Service;
using WebUI.Code;
using WebUI.UserControls;

namespace WebUI
{
	public partial class _Default : System.Web.UI.Page
	{
		private const uint PageSize = 100;

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		[WebMethod]
		public static string RunJob(long jobId)
		{
			string returnMessage = string.Format("Could not set job {0} to status 'Ready'.", jobId);
			try
			{
				using (AccessPointClient client = new AccessPointClient())
				{
					SetJobStatusesResponse response = client.SetJobStatuses(new SetJobStatusesRequest
					{
						JobIds = new long[] { jobId },
						NewStatus = JobStatus.Ready,
					});
					if (response.Success)
					{
						returnMessage = string.Format("Successfully set job {0} to be run.", jobId);
					}
				}
			}
			catch (Exception ex)
			{
				returnMessage += "<br /><br />" + ex.Message;
			}
			return returnMessage;
		}

		[WebMethod]
		public static string DeleteJob(long jobId)
		{
			string returnMessage = string.Format("Could not delete job {0}.", jobId);
			try
			{
				using (AccessPointClient client = new AccessPointClient())
				{
					DeleteJobResponse response = client.DeleteJob(new DeleteJobRequest
					{
						JobId = jobId,
					});
					if (response.Success)
					{
						returnMessage = string.Format("Successfully deleted job {0}.", jobId);
					}
				}
			}
			catch (Exception ex)
			{
				returnMessage += "<br /><br />" + ex.Message;
			}
			return returnMessage;
		}

		[WebMethod]
		public static string GetAlertsView()
		{
			List<Alert> alerts = new List<Alert>();
			using (AccessPointClient accessPoint = new AccessPointClient())
			{
				Alert[] page;
				do
				{
					page = accessPoint.GetAlerts(new GetAlertsRequest
					{
						Skip = (uint)alerts.Count,
						Take = PageSize,
					}).Alerts;
					alerts.AddRange(page);
				}
				while (page != null && page.Length == PageSize);

				long[] jobIds = alerts.Select(a => a.JobId).ToArray();
				List<JobData> jobs = new List<JobData>();
				JobData[] jobPage;
				do
				{
					jobPage = accessPoint.GetJobs(new GetJobsRequest
					{
						Skip = (uint)jobs.Count,
						Take = PageSize,
					}).Jobs;
					jobs.AddRange(jobPage);
				}
				while (jobPage != null && jobPage.Length == PageSize);

				if (jobs.Count > 0)
				{
					foreach (var alert in alerts)
					{
						var job = jobs.FirstOrDefault(j => j.Id == alert.JobId);
						if (job != null)
						{
							alert.JobName = job.Name;
						}
					}
				}
			}

			System.Web.HttpRuntime.Cache.Remove(Constants.AlertsListCacheKey);
			System.Web.HttpRuntime.Cache.Add(Constants.AlertsListCacheKey, alerts, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);

			AlertNotifier alertNotifierControl = AjaxUtility.LoadControl("/UserControls/AlertNotifier.ascx") as AlertNotifier;
			alertNotifierControl.AlertCount = alerts.Count;
			return AjaxUtility.RenderUserControl(alertNotifierControl);
		}

		[WebMethod]
		public static string DeleteAlert(long alertId)
		{
			string returnMessage = string.Format("Could not delete alert {0}.", alertId);
			try
			{
				using (AccessPointClient client = new AccessPointClient())
				{
					DeleteAlertsResponse response = client.DeleteAlerts(new DeleteAlertsRequest
					{
						Ids = new long[] { alertId },
					});
					if (response.Success)
					{
						returnMessage = string.Format("Successfully deleted alert {0}.", alertId);
						GetAlertsView();
					}
				}
			}
			catch (Exception ex)
			{
				returnMessage += "<br /><br />" + ex.Message;
			}
			return returnMessage;
		}

		[WebMethod]
		public static string DeleteAllAlerts()
		{
			string returnMessage = string.Format("Could not delete all alerts");
			try
			{
				using (AccessPointClient client = new AccessPointClient())
				{
					DeleteAlertsResponse response = client.DeleteAlerts(new DeleteAlertsRequest
					{
						Ids = null,
					});
					if (response.Success)
					{
						returnMessage = string.Format("Successfully deleted all alerts.");
						GetAlertsView();
					}
				}
			}
			catch (Exception ex)
			{
				returnMessage += "<br /><br />" + ex.Message;
			}
			return returnMessage;
		}
	}
}
