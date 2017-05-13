using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLayer.BusinessModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using SocialNetwork.Models;
using WEB.Filters;
using WEB.Hubs;

namespace WEB.Controllers
{
    [System.Web.Mvc.Authorize]
    public class FriendController : Controller
    {

        public async Task<ActionResult> Index()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());



            await soc.Friends.Counters.FriendsCounters();

            #region Parallel operations
            var unread = soc.Messages.GetUnreadAsync();
            var newFriends = soc.Friends.Counters.CountRequestsAync();
            var friends = soc.Friends.Counters.CountFriendsAync();
            var followers = soc.Friends.Counters.CountFollowersAync();
            var followed = soc.Friends.Counters.CountFollowedAync();
            var avatar = soc.Users.GetAvatarAsync();
            #endregion

            var friendModels = (await soc.Friends.GetFriendsAsync()).AsParallel().Select(friend => new FriendModel
            {
                Address = friend.Address,
                Name = friend.Name,
                Surname = friend.LastName,
                PublicId = friend.PublicId,
                Avatar = friend.Avatar,
                IsFriend = true
            }).ToList();


            await Task.WhenAll(friends, followed, followers, newFriends, unread, avatar);

            ViewBag.NewFriends = newFriends.Result;
            ViewBag.Friends = friends.Result;
            ViewBag.Followers = followers.Result;
            ViewBag.Followed = followed.Result;
            ViewBag.UnRead = unread.Result;
            ViewBag.Avatar = avatar.Result;

            return View(friendModels);
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Friends()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friendModels = (await soc.Friends.GetFriendsAsync()).AsParallel().Select(friend => new FriendModel
            {
                Address = friend.Address,
                Name = friend.Name,
                Surname = friend.LastName,
                PublicId = friend.PublicId,
                Avatar = friend.Avatar,
                IsFriend = true

            }).ToList();
            return PartialView("Partial/Friends", friendModels);
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Followed()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friendModels = (await soc.Friends.GetFollowedAsync()).AsParallel().Select(friend => new FriendModel
            {
                Address = friend.Address,
                Name = friend.Name,
                Surname = friend.LastName,
                PublicId = friend.PublicId,
                Avatar = friend.Avatar,
                IsRequested = true
            });

            return PartialView("Partial/Friends", friendModels);
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Followers()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friendModels = new List<FriendModel>();
            Parallel.ForEach(await soc.Friends.GetFollowersAsync(), (friend) =>
            {
                friendModels.Add(new FriendModel
                {
                    Address = friend.Address,
                    Name = friend.Name,
                    Surname = friend.LastName,
                    PublicId = friend.PublicId,
                    Avatar = friend.Avatar,
                    IsFollower = true
                });
            });
            return PartialView("Partial/Friends", friendModels);
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Add(long publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = await soc.Friends.AddAsync(publicId);
            var user = await soc.Users.GetAsync(soc.Id);
            await soc.Friends.Counters.FriendsCounters();


            var model = new FriendNotificationModel
            {
                Avatar = soc.Users.Avatar,
                Name = user.Name,
                Surname = user.LastName,
                PublicId = soc.Users.PublicId,
                Status = "followed you"
            };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));

            var friendSoc = new SocialNetworkFunctionalityUser(friend.UserId);
            await friendSoc.Friends.Counters.FriendsCounters();

            #region Parallel operations
            var ffollowers = friendSoc.Friends.Counters.CountFollowersAync();
            var ffriends = friendSoc.Friends.Counters.CountFriendsAync();
            var ffollowed = friendSoc.Friends.Counters.CountFollowedAync();
            var fnewfriends = friendSoc.Friends.Counters.CountRequestsAync();
            var followers = soc.Friends.Counters.CountFollowersAync();
            var friends = soc.Friends.Counters.CountFriendsAync();
            var followed = soc.Friends.Counters.CountFollowedAync();
            var newfriends = soc.Friends.Counters.CountRequestsAync();
            #endregion
            await Task.WhenAll(followers, friends, followed, newfriends, ffollowers, ffriends, ffollowed, fnewfriends);
            UpdateCounters(friendSoc.Id,Json(
                new
                {
                    followers = ffollowers.Result,
                    friends = ffriends.Result,
                    followed = ffollowed.Result,
                    newfriends = fnewfriends.Result
                }).Data);
            UpdateCounters(soc.Id, Json(
                new
                {
                    followers = followers.Result,
                    friends = friends.Result,
                    followed = followed.Result,
                    newfriends = newfriends.Result
                }).Data);
            return Content("Unsubscribe");
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Confirm(long publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = await soc.Friends.ConfirmAsync(publicId);
            var user = await soc.Users.GetAsync(soc.Id);

            var model = new FriendNotificationModel
            {
                Avatar = soc.Users.Avatar,
                Name = user.Name,
                Surname = user.LastName,
                PublicId = soc.Users.PublicId,
                Status = "confirmed your request"
            };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));

            var friendSoc = new SocialNetworkFunctionalityUser(friend.FriendId);
            await friendSoc.Friends.Counters.FriendsCounters();
            await soc.Friends.Counters.FriendsCounters();
            #region Parallel operations

            var friendNewFriends = friendSoc.Friends.Counters.CountRequestsAync();
            var friendFollowers = friendSoc.Friends.Counters.CountFollowersAync();
            var friendFriends = friendSoc.Friends.Counters.CountFriendsAync();
            var friendFollowed = friendSoc.Friends.Counters.CountFollowedAync();

            var newfriends = soc.Friends.Counters.CountRequestsAync();
            var followers = soc.Friends.Counters.CountFollowersAync();
            var friends = soc.Friends.Counters.CountFriendsAync();
            var followed = soc.Friends.Counters.CountFollowedAync();
            #endregion

            await Task.WhenAll(followers, friends, followed, newfriends, friendFollowers, friendFriends, friendFollowed,
                friendNewFriends);
            UpdateCounters(friendSoc.Id,Json(
                new
                {
                    newfriends = friendNewFriends.Result,
                    followers = friendFollowers.Result,
                    friends = friendFriends.Result,
                    followed = friendFollowed.Result
                }).Data);
            UpdateCounters(soc.Id,Json(
                new
                {
                    newfriends = newfriends.Result,
                    followers = followers.Result,
                    friends = friends.Result,
                    followed = followed.Result
                }).Data);
            return Content("Delete");
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Delete(long publicId)
        {
            List<Task> paralelCountersInitializingTasks = new List<Task>();
            List<Task<long>> parallelFriendCounntingTasks = new List<Task<long>>();

            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());
            var friend = await soc.Friends.DeleteAsync(publicId);

            paralelCountersInitializingTasks.Add(soc.Friends.Counters.FriendsCounters());

            var friendSoc = new SocialNetworkFunctionalityUser(friend.FriendId);
            paralelCountersInitializingTasks.Add(friendSoc.Friends.Counters.FriendsCounters());
            var user = await soc.Users.GetAsync(soc.Id);

            await Task.WhenAll(paralelCountersInitializingTasks);

            parallelFriendCounntingTasks.AddRange(new List<Task<long>>
            {
                soc.Friends.Counters.CountFollowersAync(),
                soc.Friends.Counters.CountFriendsAync(),
                soc.Friends.Counters.CountFollowedAync(),
                soc.Friends.Counters.CountRequestsAync(),
                friendSoc.Friends.Counters.CountFollowersAync(),
                friendSoc.Friends.Counters.CountFriendsAync(),
                friendSoc.Friends.Counters.CountFollowedAync(),
                friendSoc.Friends.Counters.CountRequestsAync()
            });
            await Task.WhenAll(parallelFriendCounntingTasks);
            UpdateCounters(soc.Id,Json(
                new
                {
                    followers = parallelFriendCounntingTasks[0].Result,
                    friends = parallelFriendCounntingTasks[1].Result,
                    followed = parallelFriendCounntingTasks[2].Result,
                    newfriends = parallelFriendCounntingTasks[3].Result
                }).Data);
            UpdateCounters(friendSoc.Id,Json(
                new
                {
                    followers = parallelFriendCounntingTasks[4].Result,
                    friends = parallelFriendCounntingTasks[5].Result,
                    followed = parallelFriendCounntingTasks[6].Result,
                    newfriends = parallelFriendCounntingTasks[7].Result
                }).Data);
            var model = new FriendNotificationModel
            {
                Avatar = soc.Users.Avatar,
                Name = user.Name,
                Surname = user.LastName,
                PublicId = soc.Users.PublicId,
                Status = "deleted you from friends"
            };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));


            return Content("Add To Friends");
        }
        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Unsubscribe(long publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());
            await soc.Friends.UnsubscribeAsync(publicId);
            await soc.Friends.Counters.FriendsCounters();

            var friendsTask = soc.Friends.Counters.CountFriendsAync();
            var followersTask = soc.Friends.Counters.CountFollowersAync();
            var followedTask = soc.Friends.Counters.CountFollowedAync();
            await Task.WhenAll(friendsTask, followedTask, followersTask);

            UpdateCounters(soc.Id,Json(
                new
                {
                    followers = followersTask.Result,
                    friends = friendsTask.Result,
                    followed = followedTask.Result
                }).Data);

            return Content("Add Friend");
        }

        #region SignalR Methods
        private void AddFriend(string publicId, string notification)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(publicId).addFriend(notification);
        }

        private void UpdateCounters(string publicId, object count)
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