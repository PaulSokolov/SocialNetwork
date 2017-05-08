using BusinessLayer.BusinessModels;
using Microsoft.AspNet.Identity;
using SocialNetwork.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using WEB.Hubs;

namespace WEB.Controllers
{
    [Authorize]
    public class FriendController : Controller
    {

        public ActionResult Index()
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            List<FriendModel> friends = new List<FriendModel>();

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;
            ViewBag.Friends = soc.Friends.Counters.Friends;
            ViewBag.Followers = soc.Friends.Counters.Followers;
            ViewBag.Followed = soc.Friends.Counters.Followed;

            foreach (var friend in soc.Friends.GetFriends())
            {
                friends.Add(new FriendModel
                {
                    Address = friend.Address,
                    Name = friend.Name,
                    Surname = friend.LastName,
                    PublicId = friend.PublicId,
                    Avatar = friend.Avatar,
                    IsFriend = true
                });
            } 
            return View(friends);
        }

        [HttpPost]
        public ActionResult Friends()
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            StringBuilder content = new StringBuilder();

            foreach (var friend in soc.Friends.GetFriends())
            {
                var friendModel = new FriendModel
                {
                    Address = friend.Address,
                    Name = friend.Name,
                    Surname = friend.LastName,
                    PublicId = friend.PublicId,
                    Avatar = friend.Avatar,
                    IsFriend = true
                };
                content.Append(RenderRazorViewToString("Partial/Friend", friendModel));
            }
            return Content(content.ToString());
        }

        [HttpPost]
        public ActionResult Followed()
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            StringBuilder content = new StringBuilder();
            foreach (var friend in soc.Friends.GetFollowed())
            {
                var friendModel = new FriendModel
                {
                    Address = friend.Address,
                    Name = friend.Name,
                    Surname = friend.LastName,
                    PublicId = friend.PublicId,
                    Avatar = friend.Avatar,
                    IsRequested = true
                };
                content.Append(RenderRazorViewToString("Partial/Friend", friendModel));
            }

            return Content(content.ToString());
        }

        [HttpPost]
        public ActionResult Followers()
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            StringBuilder content = new StringBuilder();
            foreach (var friend in soc.Friends.GetFollowers())
            {
                var friendModel = new FriendModel
                {
                    Address = friend.Address,
                    Name = friend.Name,
                    Surname = friend.LastName,
                    PublicId = friend.PublicId,
                    Avatar = friend.Avatar,
                    IsFollower = true
                };
                content.Append(RenderRazorViewToString("Partial/Friend", friendModel));
            }
            return Content(content.ToString());
        }

        [HttpPost]
        public ActionResult Add(long publicId)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = soc.Friends.Add(publicId);
            var user = soc.Users.Get(soc.Id);

            FriendNotificationModel model = new FriendNotificationModel { Avatar = soc.Users.Avatar, Name = user.Name, Surname = user.LastName, PublicId = soc.Users.PublicId, Status = "followed you" };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));

            SocialNetworkFunctionalityUser friendSoc = new SocialNetworkFunctionalityUser(friend.UserId);

            UpdateCounters(friend.UserId, new long[] { friendSoc.Friends.Counters.Followers, friendSoc.Friends.Counters.Friends, friendSoc.Friends.Counters.Followed });
            
            return Content("Unsubscribe");
        }

        [HttpPost]
        public ActionResult Confirm(long publicId)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = soc.Friends.Confirm(publicId);
            var user = soc.Users.Get(soc.Id);

            FriendNotificationModel model = new FriendNotificationModel { Avatar = soc.Users.Avatar, Name = user.Name, Surname = user.LastName, PublicId = soc.Users.PublicId, Status = "confirmed your request" };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));

            SocialNetworkFunctionalityUser friendSoc = new SocialNetworkFunctionalityUser(friend.UserId);

            UpdateCounters(friend.UserId, new long[] { friendSoc.Friends.Counters.Followers, friendSoc.Friends.Counters.Friends, friendSoc.Friends.Counters.Followed });

            return Content("Delete");
        }

        [HttpPost]
        public ActionResult Delete(long publicId)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = soc.Friends.Delete(publicId);            
            var user = soc.Users.Get(soc.Id);

            UpdateCounters(soc.Id, new long[] { soc.Friends.Counters.Requests, soc.Friends.Counters.Friends, soc.Friends.Counters.Followed });

            FriendNotificationModel model = new FriendNotificationModel { Avatar = soc.Users.Avatar, Name = user.Name, Surname = user.LastName, PublicId = soc.Users.PublicId, Status = "deleted you from friends" };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));

            SocialNetworkFunctionalityUser friendSoc = new SocialNetworkFunctionalityUser(friend.UserId);

            UpdateCounters(friend.FriendId, new long[] { friendSoc.Friends.Counters.Requests, friendSoc.Friends.Counters.Friends, friendSoc.Friends.Counters.Followed });
            
            return Content("Add To Friends");
        }
        [HttpPost]
        public ActionResult Unsubscribe(long publicId)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            soc.Friends.Unsubscribe(publicId);
            UpdateCounters(soc.Id, new long[] { soc.Friends.Counters.Followers, soc.Friends.Counters.Friends, soc.Friends.Counters.Followed });

            return Content("Add Friend");
        }

        #region SignalR Methods
        private void AddFriend(string publicId, string notification)
        {
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(publicId).addFriend(notification);
        }

        private void UpdateCounters(string publicId, long[] count)
        {
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(publicId).friendCounters(count);
        }
        #endregion
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