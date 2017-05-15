using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BusinessLayer.BusinessModels;
using Microsoft.AspNet.Identity;

namespace WEB.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public async Task<ActionResult> NotFound()
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());
            ViewBag.Avatar = await soc.Users.GetAvatarAsync();
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult Forbidden()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}