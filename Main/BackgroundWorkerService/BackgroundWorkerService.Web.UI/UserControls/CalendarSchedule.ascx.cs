using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUI.UserControls
{
	public partial class CalendarSchedule : System.Web.UI.UserControl
	{
		public BackgroundWorkerService.Service.CalendarSchedule SelectedCalendarSchedule { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				StartDateTime.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (SelectedCalendarSchedule != null)
			{
				StartDailyAt.Text = SelectedCalendarSchedule.StartDailyAt.Hour.ToString("00") + ":" + SelectedCalendarSchedule.StartDailyAt.Minute.ToString("00") + ":" + SelectedCalendarSchedule.StartDailyAt.Second.ToString("00");
				RepeatInterval.Text = SelectedCalendarSchedule.RepeatInterval.ToString();
				StartDateTime.Text = SelectedCalendarSchedule.StartDateTime.ToString("dd-MM-yyyy HH:mm:ss");
				if (SelectedCalendarSchedule.DaysOfWeek != null)
				{
					foreach (var dayOfWeek in SelectedCalendarSchedule.DaysOfWeek)
					{
						var checkBox = this.FindControl(dayOfWeek.ToString()) as CheckBox;
						if (checkBox != null)
						{
							checkBox.Checked = true;
						}
					}
				}
			}
		}
	}
}