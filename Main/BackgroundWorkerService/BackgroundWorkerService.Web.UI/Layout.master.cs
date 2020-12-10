using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WebUI.BackgroundWorkerService.Service;

namespace WebUI
{
	public partial class Layout : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			List<string> jobs = new List<string>();
			using (AccessPointClient accessPoint = new AccessPointClient())
			{
				List<string> page;
				page = accessPoint.GetJobs(new GetJobsRequest
				{
					Skip = (uint)jobs.Count,
					Take = 10,
				}).Jobs.Select(x => x.Application).ToList();
				jobs.AddRange(page);
			}

			foreach (string applicationName in jobs.Distinct())
			{
				if (applicationName != null)
				{
					var tab = new HtmlGenericControl("li");
					tab.Controls.Add(new HtmlAnchor() { HRef = "#" + applicationName, InnerText = applicationName });

					mainMenuDiv.FindControl("mainMenuUl").Controls.Add(tab);

					var div = new HtmlGenericControl("div");
					div.Attributes.Add("id", applicationName);

					mainMenuDiv.Controls.Add(div);
					var control = (WebUI.UserControls.Jobs)Page.LoadControl("~/UserControls/Jobs.ascx");
					control.ApplicationName = applicationName;
					div.Controls.Add(control);
				}
			}
		}
	}
}