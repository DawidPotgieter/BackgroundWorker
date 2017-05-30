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
	public partial class Alerts : System.Web.UI.Page
	{
		List<Alert> alerts = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			alerts = System.Web.HttpRuntime.Cache[Constants.AlertsListCacheKey] as List<Alert>;
			if (alerts == null) alerts = new List<Alert>();

			AlertsView.DataSource = alerts;
			AlertsView.DataBind();
		}

		protected void AlertsView_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
			if (e.Item.ItemType == ListViewItemType.DataItem)
			{
				Alert alert = (Alert)e.Item.DataItem;
				Image deleteButtonImage = e.Item.FindControl("btnDeleteAlertImage") as Image;

				deleteButtonImage.Attributes["onclick"] = string.Format("queryDeleteAlert({0});return false;", alert.Id);
			}
		}

		protected void btnRefresh_Click(object sender, ImageClickEventArgs e)
		{
		}
	}
}