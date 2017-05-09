using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using Microsoft.AspNet.Identity;
using SocialNetwork.Models;

namespace WEB.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {

        [HttpGet]
        public ActionResult Index(string search)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            ViewBag.Unread = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            ViewBag.MyPublicId = soc.Users.PublicId;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;

            var friends = soc.Friends.GetFriends();
            var followers = soc.Friends.GetFollowers();
            var followed = soc.Friends.GetFollowed();
            var usersFound = new List<UserProfileDTO>();

            if (search == string.Empty)
                usersFound = soc.Users.Search();
            else
            {
                usersFound = soc.Users.Search(activityConcurence: search, aboutConcurence: search);
                usersFound.AddRange(soc.Users.Search(search));
            }

            var users = new List<UserSearchModel>();
            usersFound = usersFound.Distinct().ToList();

            foreach (var user in usersFound)
            {
                var userModel = new UserSearchModel
                {
                    Name = user.Name,
                    Surname = user.LastName,
                    Address = user.Address,
                    Avatar = user.Avatar,
                    PublicId = user.PublicId
                };
                if (friends.Contains(user))
                    userModel.IsFriend = true;
                else if (followers.Contains(user))
                    userModel.IsFollower = true;
                else if (followed.Contains(user))
                    userModel.IsFollowed = true;

                users.Add(userModel);
            }

            return View(new SearchPageModel { Users = users, Countries = soc.Users.GetCountriesWithUsers() });
        }

        [HttpPost]
        public ActionResult Search(string search, int? ageFrom, int? ageTo, long? cityId, long? countryId, string activityConcurence, string aboutConcurence, int? sex, short? sort)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            ViewBag.Unread = soc.Messages.UnRead;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;
            ViewBag.MyPublicId = soc.Users.PublicId;

            var friends = soc.Friends.GetFriends();
            var followers = soc.Friends.GetFollowers();
            var followed = soc.Friends.GetFollowed();
            var users = soc.Users.Search(search,ageFrom, ageTo, cityId, countryId, activityConcurence, aboutConcurence, sex, sort);
            
            var content = new StringBuilder();

            foreach (var user in users)
            {
                var userSM = new UserSearchModel
                {
                    Name = user.Name,
                    Surname = user.LastName,
                    Address = user.Address,
                    Avatar = user.Avatar,
                    PublicId = user.PublicId
                };

                if (friends.Contains(user))
                    userSM.IsFriend = true;
                else if (followers.Contains(user))
                    userSM.IsFollower = true;
                else if (followed.Contains(user))
                    userSM.IsFollowed = true;

                content.Append(RenderRazorViewToString("Partial/User", userSM));
            }

            return Content(content.ToString());
        }

        public ActionResult AutocompleteSearch(string term)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var searchResult = soc.Users.Search(term);
            var users = searchResult.Select(u => new { name = u.Name, avatar = u.Avatar, lastName = u.LastName, publicId = u.PublicId, address = u.Address.Length > 30 ? u.Address.Substring(0, 30):u.Address }).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        #region Helper
        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        } 
        #endregion
    }
}