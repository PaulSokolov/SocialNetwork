using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WEB.Filters
{
    public class AjaxOnlyAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
                base.OnActionExecuting(filterContext);
            else
            {
                filterContext.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary(new {controller = "Error", action = "Forbidden"}));
            }
        }
    }
    public class AuthorizedIndexAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
                base.OnActionExecuting(filterContext);
            else
            {
                filterContext.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Home", action = "Profile" }));
            }
        }
    }
}