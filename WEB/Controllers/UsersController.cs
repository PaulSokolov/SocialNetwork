﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Index(string search)
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());
            await soc.Friends.Counters.FriendsCounters();


            #region Parallel operations
            var countries = soc.Users.GetCountriesWithUsersAsync();
            var friends = soc.Friends.GetFriendsAsync();
            var followers = soc.Friends.GetFollowersAsync();
            var followed = soc.Friends.GetFollowedAsync();
            var unread = soc.Messages.GetUnreadAsync();
            var avatar = soc.Users.GetAvatarAsync();
            var myPublicId = soc.Users.GetPublicIdAsync();
            var newFriends = soc.Friends.Counters.CountRequestsAync(); 
            #endregion

            var usersFound = new List<UserProfileDTO>();

            if (search == string.Empty)
                usersFound = await soc.Users.SearchAsync();
            else
            {
                usersFound = await soc.Users.SearchAsync(activityConcurence: search, aboutConcurence: search);
                usersFound.AddRange(await soc.Users.SearchAsync(search));
            }
            
            usersFound = usersFound.Distinct().ToList();
            await Task.WhenAll(friends, followers, followed);
            var users = usersFound.AsParallel().Select(user =>
            {
                var userModel = new UserSearchModel
                {
                    Name = user.Name,
                    Surname = user.LastName,
                    Address = user.Address,
                    Avatar = user.Avatar,
                    PublicId = user.PublicId
                };
                if (friends.Result.Contains(user))
                    userModel.IsFriend = true;
                else if (followers.Result.Contains(user))
                    userModel.IsFollower = true;
                else if (followed.Result.Contains(user))
                    userModel.IsFollowed = true;
                return userModel;
            }).ToList();
            
            await Task.WhenAll(countries, unread, avatar, myPublicId, newFriends);

            ViewBag.Unread = unread.Result;
            ViewBag.Avatar = avatar.Result;
            ViewBag.MyPublicId = myPublicId.Result;
            ViewBag.NewFriends = newFriends.Result;
            return View(new SearchPageModel { Users = users, Countries =  countries.Result });
        }

        [HttpPost]
        public async Task<ActionResult> Search(string search, int? ageFrom, int? ageTo, long? cityId, long? countryId, string activityConcurence, string aboutConcurence, int? sex, short? sort, int? lastIndex)
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());
            await soc.Friends.Counters.FriendsCounters();
            #region Parallel operations
            var friends = soc.Friends.GetFriendsAsync();
            var followers = soc.Friends.GetFollowersAsync();
            var followed = soc.Friends.GetFollowedAsync();
            var unread = soc.Messages.GetUnreadAsync();
            var newFriends = soc.Friends.Counters.CountRequestsAync();
            var myPublicId = soc.Users.GetPublicIdAsync(); 
            #endregion

            var users = await soc.Users.SearchAsync(search,ageFrom, ageTo, cityId, countryId, activityConcurence, aboutConcurence, sex, sort, lastIndex);
            
            

            await Task.WhenAll(friends, followers, followed);
            
            var parallelQuery = users.AsParallel();
            switch (sort)
            {
                case 0:
                    parallelQuery = parallelQuery.OrderByDescending(user => user.ActivatedDate);
                    break;
                case 1:
                    parallelQuery =
                        parallelQuery.OrderByDescending(user => user.ActivatedDate);
                    break;
            }



            var userModels = parallelQuery.Select(user =>
            {
                var userSM = new UserSearchModel
                {
                    Name = user.Name,
                    Surname = user.LastName,
                    Address = user.Address,
                    Avatar = user.Avatar,
                    PublicId = user.PublicId
                };

                if (friends.Result.Contains(user))
                    userSM.IsFriend = true;
                else if (followers.Result.Contains(user))
                    userSM.IsFollower = true;
                else if (followed.Result.Contains(user))
                    userSM.IsFollowed = true;
                return userSM;

            }).ToList();
            await Task.WhenAll(unread, newFriends, myPublicId);
            
            ViewBag.Unread = unread.Result;
            ViewBag.NewFriends = newFriends.Result;
            ViewBag.MyPublicId = myPublicId.Result;

            return PartialView("Partial/Users", userModels);
        }

        public async Task<ActionResult> AutocompleteSearch(string term)
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());

            var searchResult = await soc.Users.SearchAsync(term);
            var users = searchResult.Select(u => new { name = u.Name, avatar = u.Avatar, lastName = u.LastName, publicId = u.PublicId, address = u.Address.Length > 30 ? u.Address.Substring(0, 30):u.Address }).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }
    }
}