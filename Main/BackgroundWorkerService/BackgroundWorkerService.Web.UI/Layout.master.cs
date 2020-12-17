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
				var page = accessPoint.GetJobs(new GetJobsRequest
				{
					Skip = 0,
					Take = int.MaxValue,
				});

				jobs.AddRange(page.Jobs.Select(x => x.Application));
			}

			foreach (string applicationName in jobs.Distinct().OrderBy(an => an))
			{
				if (string.IsNullOrWhiteSpace(applicationName))
					continue;
				var tab = new HtmlGenericControl("li");
				tab.Controls.Add(new HtmlAnchor() { HRef = "#" + applicationName, InnerText = applicationName, ID = "tab" + applicationName });

				mainMenuDiv.FindControl("mainMenuUl").Controls.Add(tab);

				var div = new HtmlGenericControl("div");
				div.Attributes.Add("id", applicationName);

				mainMenuDiv.Controls.Add(div);
				var control = (WebUI.UserControls.Jobs)Page.LoadControl("~/UserControls/Jobs.ascx");
				control.ID = "Jobs_" + applicationName;
				control.ApplicationName = applicationName;
				div.Controls.Add(control);
			}
		}
	}
}