using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI
{
	public static class Helpers
	{
		public static Guid? TryParseNullableGuid(string guidValue)
		{
			Guid guid;
			if (Guid.TryParse(guidValue, out guid))
			{
				return guid;
			}
			return null;
		}
	}
}