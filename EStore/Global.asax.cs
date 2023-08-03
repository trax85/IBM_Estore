using EStore.Utilities;
using EStore.Utilities.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using Unity.Mvc5;

namespace EStore
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["UserCount"] = 0;
            UnityConfig.RegisterComponents();

            // Create a Unity container
            var container = new UnityContainer();

            // Register dependencies
            container.RegisterType<IUserDataRepository, UserDataRepository>();
            container.RegisterType<IProductDataRepository, ProductDataRepository>();
            container.RegisterType<IProductDataRepositoryV2, ProductDataRepositoryV2>();
            container.RegisterType<IDashboardDataRepository, DashboardDataRepository>();
            container.RegisterType<ICartDataRepository, CartDataRepository>();

            // Set the Unity dependency resolver
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        protected void Session_start(object sender, EventArgs e)
        {
            Session[Models.Cart.CartCountSessionString] = 0;
            Session[Models.Cart.CartSessionString] = new List<Models.Cart>();

            Application.Lock();
            Application["UserCount"] = ((int)Application["UserCount"]) + 1;
            Application.UnLock();
        }

        protected void Session_end()
        {
            // Decrement the user count when a session (user) ends
            Application.Lock();
            Application["UserCount"] = ((int)Application["UserCount"]) - 1;
            Application.UnLock();
        }
    }
}
