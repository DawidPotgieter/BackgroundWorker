using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;

namespace WebUI.Code
{
	internal class AjaxUtility
	{
		internal static string RenderUserControl(string virtualPath)
		{
			Page page = new Page();
			page.Controls.Add(page.LoadControl(virtualPath));
			using (StringWriter writer = new StringWriter())
			{
				HttpContext.Current.Server.Execute(page, writer, false);
				return writer.ToString();
			}
		}

		internal static string RenderUserControl(UserControl usercontrol)
		{
			Page page = new Page();
			page.Controls.Add(usercontrol);
			using (StringWriter writer = new StringWriter())
			{
				HttpContext.Current.Server.Execute(page, writer, false);
				return writer.ToString();
			}
		}

		internal static Control LoadControl(string virtualPath)
		{
			Page page = new Page();
			return page.LoadControl(virtualPath);
		}
	}

}