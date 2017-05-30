using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUI.UserControls
{
	public partial class AlertNotifier : System.Web.UI.UserControl
	{
		public long AlertCount { get; set; }

		protected void Page_PreRender(object sender, EventArgs e)
		{
			alertsNotifier.Visible = false;
			if (AlertCount > 0)
			{
				alertsNotifier.Visible = true;
				alertsNotifier.InnerText = AlertCount.ToString();
			}
		}
	}
}