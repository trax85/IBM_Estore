using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["UserCount"] = 0;
        }

        protected void Session_start(object sender, EventArgs e)
        {
            Session[Models.Cart.CartCountSessionString] = 0;
            Session[Models.Cart.CartSessionString] = new List<Models.Cart>();

            Application.Lock();
            Application["UserCount"] = (int)Application["UserCount"] + 1;
            Application.UnLock();
        }

        protected void Session_end()
        {
            // Decrement the user count when a session (user) ends
            Application.Lock();
            Application["UserCount"] = (int)Application["UserCount"] - 1;
            Application.UnLock();
        }
    }
}
