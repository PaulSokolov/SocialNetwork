using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLayer.BusinessModels;
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

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Friends()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friendModels = new List<FriendModel>();

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
            return PartialView("Partial/Friends", friendModels);
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Followed()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friendModels = new List<FriendModel>();
            Parallel.ForEach(await soc.Friends.GetFollowedAsync(), (friend) =>
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
                        IsRequested = true
                    });
                }
            });

            return PartialView("Partial/Friends", friendModels);
        }

        [HttpPost,AjaxOnly]
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
        //TODO: After making userscategory async. add this methods to action
        [HttpPost,AjaxOnly]
        public async Task<ActionResult> Add(long publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = await soc.Friends.AddAsync(publicId);
            var user = soc.Users.Get(soc.Id);

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
            UpdateCounters(friend.UserId,
                new[]
                {
                    await friendSoc.Friends.Counters.CountFollowersAync(),
                    await friendSoc.Friends.Counters.CountFriendsAync(),
                    await friendSoc.Friends.Counters.CountFollowersAync()
                });
            
            return Content("Unsubscribe");
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Confirm(long publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var friend = await soc.Friends.ConfirmAsync(publicId);
            var user = soc.Users.Get(soc.Id);

            var model = new FriendNotificationModel
            {
                Avatar = soc.Users.Avatar,
                Name = user.Name,
                Surname = user.LastName,
                PublicId = soc.Users.PublicId,
                Status = "confirmed your request"
            };

            AddFriend(friend.FriendId, RenderRazorViewToString("../Shared/FriendNotification", model));

            var friendSoc = new SocialNetworkFunctionalityUser(friend.UserId);
            await friendSoc.Friends.Counters.FriendsCounters();
            UpdateCounters(friend.UserId,
                new[]
                {
                    await friendSoc.Friends.Counters.CountFollowersAync(),
                    await friendSoc.Friends.Counters.CountFriendsAync(),
                    await friendSoc.Friends.Counters.CountFollowedAync()
                });
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

            var friendSoc = new SocialNetworkFunctionalityUser(friend.UserId);
            paralelCountersInitializingTasks.Add(friendSoc.Friends.Counters.FriendsCounters());
            var user = soc.Users.Get(soc.Id);

            await Task.WhenAll(paralelCountersInitializingTasks);

            parallelFriendCounntingTasks.AddRange(new List<Task<long>>
            {
                soc.Friends.Counters.CountRequestsAync(),
                soc.Friends.Counters.CountFriendsAync(),
                soc.Friends.Counters.CountFollowedAync(),
                friendSoc.Friends.Counters.CountRequestsAync(),
                friendSoc.Friends.Counters.CountFriendsAync(),
                friendSoc.Friends.Counters.CountFollowedAync()
            });
            await Task.WhenAll(parallelFriendCounntingTasks);
            UpdateCounters(soc.Id,
                new[]
                {
                    parallelFriendCounntingTasks[0].Result,
                    parallelFriendCounntingTasks[1].Result,
                    parallelFriendCounntingTasks[2].Result
                });
            UpdateCounters(friend.FriendId,
                new[]
                {
                    parallelFriendCounntingTasks[3].Result,
                    parallelFriendCounntingTasks[4].Result,
                    parallelFriendCounntingTasks[5].Result
                });
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
        [HttpPost,AjaxOnly]
        public async Task<ActionResult> Unsubscribe(long publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());
            await soc.Friends.UnsubscribeAsync(publicId);

            var friendsTask = soc.Friends.Counters.CountFriendsAync();
            var followersTask = soc.Friends.Counters.CountFollowersAync();
            var followedTask = soc.Friends.Counters.CountFollowedAync();
            await Task.WhenAll(friendsTask, followedTask, followersTask);

            UpdateCounters(soc.Id,
                new[]
                {
                    followersTask.Result ,
                    friendsTask.Result ,
                    followedTask.Result 
                });

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