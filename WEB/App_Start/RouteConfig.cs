using System.Web.Mvc;
using System.Web.Routing;

namespace WEB
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { controller = "Account", action = "Login" }
            );
            routes.MapRoute(
                name: "Register",
                url: "Register",
                defaults: new { controller = "Account", action = "Register" }
            );
            routes.MapRoute(
                name: "Admin",
                url: "Admin",
                defaults: new {controller = "Admin", action = "Index"}
            );
            routes.MapRoute(
                name: "Moderator",
                url: "Moderator",
                defaults: new { controller = "Moderator", action = "Index" }
            );
            routes.MapRoute(
                name: "Friend",
                url: "Friend",
                defaults: new { controller = "Friend", action = "Index" }
            );
            routes.MapRoute(
                name: "Messages",
                url: "Messages",
                defaults: new { controller = "Messages", action = "Index" }
            );
            routes.MapRoute(
                name: "Settings",
                url: "Settings",
                defaults: new { controller = "Home", action = "Settings" }
            );
            routes.MapRoute(
                name: "User",
                url: "User/{id}",
                defaults: new {controller = "Home", action = "User", id = UrlParameter.Optional}
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Profile", id = UrlParameter.Optional }
            );
        }
    }
}
