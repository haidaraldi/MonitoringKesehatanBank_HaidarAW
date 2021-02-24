using System.Web;
using System.Web.Mvc;

namespace MonitoringKesehatanBank_HaidarAW
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
