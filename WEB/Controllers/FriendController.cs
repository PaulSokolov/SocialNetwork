using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLayer.BusinessModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using SocialNetwork.Models;
using WEB.Hubs;

namespace WEB.Controllers
{
    [System.Web.Mvc.Authorize]
    public class FriendController : Controller
    {

        public async Task<ActionResult> Index()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friendModels = new List<FriendModel>();

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            await soc.Friends.Counters.FriendsCounters();
                var newFriends = soc.Friends.Counters.CountRequestsAync();
                var friends = soc.Friends.Counters.CountFriendsAync();
                var followers = soc.Friends.Counters.CountFollowersAync();
                var followed = soc.Friends.Counters.CountFollowedAync();
          

            //foreach (var friend in soc.Friends.GetFriends())
            //{
            //    friendModels.Add(new FriendModel
            //    {
            //        Address = friend.Address,
            //        Name = friend.Name,
            //        Surname = friend.LastName,
            //        PublicId = friend.PublicId,
            //        Avatar = friend.Avatar,
            //        IsFriend = true
            //    });
            //}
            Parallel.ForEach(await soc.Friends.GetFriendsAsync(), (friend) =>
            {
                lock (friendModels)
                {
                    friendModels.Add(new FriendModel
                    {
                        Address = friend.Address,
                        Name = friend.Name,
                        Surname = friend.LastName,
                        PublicId = friend.PublicId,
                        Avatar = friend.Avatar,
                        IsFriend = true
                    });
                }
            });
            //Task.WaitAll();

           
            ViewBag.NewFriends = newFriends.Result;
            ViewBag.Friends = friends.Result;
            ViewBag.Followers = followers.Result;
            ViewBag.Followed = followed.Result;

            return View(friendModels);
        }

        [HttpPost]
        public ActionResult Friends()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var content = new StringBuilder();

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
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var content = new StringBuilder();
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
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var content = new StringBuilder();
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
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = soc.Friends.Add(publicId);
            var user = soc.Users.Get(soc.Id);

            var model = new FriendNotificationModel { Avatar = soc.Users.Avatar, Name = user.Name, Surname = user.LastName, PublicId = soc.Users.PublicId, Status = "followed you" };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));

            var friendSoc = new SocialNetworkFunctionalityUser(friend.UserId);

            UpdateCounters(friend.UserId, new[] { friendSoc.Friends.Counters.Followers, friendSoc.Friends.Counters.Friends, friendSoc.Friends.Counters.Followed });
            
            return Content("Unsubscribe");
        }

        [HttpPost]
        public ActionResult Confirm(long publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = soc.Friends.Confirm(publicId);
            var user = soc.Users.Get(soc.Id);

            var model = new FriendNotificationModel { Avatar = soc.Users.Avatar, Name = user.Name, Surname = user.LastName, PublicId = soc.Users.PublicId, Status = "confirmed your request" };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));

            var friendSoc = new SocialNetworkFunctionalityUser(friend.UserId);

            UpdateCounters(friend.UserId, new[] { friendSoc.Friends.Counters.Followers, friendSoc.Friends.Counters.Friends, friendSoc.Friends.Counters.Followed });

            return Content("Delete");
        }

        [HttpPost]
        public ActionResult Delete(long publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = soc.Friends.Delete(publicId);            
            var user = soc.Users.Get(soc.Id);

            UpdateCounters(soc.Id, new[] { soc.Friends.Counters.Requests, soc.Friends.Counters.Friends, soc.Friends.Counters.Followed });

            var model = new FriendNotificationModel { Avatar = soc.Users.Avatar, Name = user.Name, Surname = user.LastName, PublicId = soc.Users.PublicId, Status = "deleted you from friends" };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));

            var friendSoc = new SocialNetworkFunctionalityUser(friend.UserId);

            UpdateCounters(friend.FriendId, new[] { friendSoc.Friends.Counters.Requests, friendSoc.Friends.Counters.Friends, friendSoc.Friends.Counters.Followed });
            
            return Content("Add To Friends");
        }
        [HttpPost]
        public ActionResult Unsubscribe(long publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            soc.Friends.Unsubscribe(publicId);
            UpdateCounters(soc.Id, new[] { soc.Friends.Counters.Followers, soc.Friends.Counters.Friends, soc.Friends.Counters.Followed });

            return Content("Add Friend");
        }

        #region SignalR Methods
        private void AddFriend(string publicId, string notification)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(publicId).addFriend(notification);
        }

        private void UpdateCounters(string publicId, long[] count)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
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