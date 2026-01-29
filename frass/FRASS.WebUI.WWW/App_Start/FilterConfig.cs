using System.Web;
using System.Web.Mvc;

namespace FRASS.WebUI.WWW
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}