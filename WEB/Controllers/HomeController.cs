using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using Microsoft.AspNet.Identity;
using SocialNetwork.Models;
using WEB.Filters;

namespace WEB.Controllers
{
    public class HomeController : Controller
    {
        [AuthorizedIndex]
        public async Task<ActionResult> Index()
        {
            ViewBag.Countries = await new SocialNetworkManager().Database.GetAllCountriesAsync();
            ViewBag.Cities = new List<CityDTO>();
            return View();
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> Index(HttpPostedFileBase file)
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());

            ViewBag.Avatar = soc.Users.Avatar;

            if (file != null && (file.ContentLength > 0 || !file.ContentType.Contains("image")))
                try
                {
                    
                    var filePath = $"/Images/avatar{User.Identity.GetUserId()}.jpg";
                    string path = Path.Combine(Server.MapPath("~/Images"),
                                               Path.GetFileName("avatar" + User.Identity.GetUserId()+".jpg"));
                    var user = await soc.Users.GetAsync(User.Identity.GetUserId());
                    user.Avatar = filePath;
                    user = await soc.Users.UpdateAsync(user);

                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message;
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return RedirectToAction("Profile");
        }

        [Authorize, ActionName("Profile")]
        public async Task<ActionResult> ProfileInfo()
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());

            UserProfileDTO user = await soc.Users.GetAsync(soc.Id);

            await soc.Friends.Counters.FriendsCounters();
            #region Parallel operations
            var newFriendsTask = soc.Friends.Counters.CountRequestsAync();
            var friendsTask = soc.Friends.Counters.CountFriendsAync();
            var unread = soc.Messages.GetUnreadAsync();
            var avatar = soc.Users.GetAvatarAsync();
            var myPublicId = soc.Users.GetPublicIdAsync();
            var friends = soc.Friends.GetFriendsAsync(6); 
            #endregion

            await Task.WhenAll(friends, newFriendsTask, friendsTask, unread, avatar, myPublicId);
            ViewBag.NewFriends = newFriendsTask.Result;
            ViewBag.Friends = friendsTask.Result;
            ViewBag.UnRead = unread.Result;
            ViewBag.Avatar = avatar.Result;
            ViewBag.MyPublicId = myPublicId.Result;

            return View(new ProfileModel
            {
                PublicId = user.PublicId,
                About=user.About,
                AboutIsHidden=user.AboutIsHidden,
                Activity =user.Activity,
                ActivityIsHidden =user.ActivityIsHidden,
                Address =user.Address,
                Avatar =user.Avatar,
                BirthDate =user.BirthDate,
                BirthDateIsHidden=user.BirthDateIsHidden,
                City =user.City, Email=user.Email,
                EmailIsHidden =user.EmailIsHidden,
                LastName =user.LastName,
                Name =user.Name,
                Role =user.Role,
                Sex =user.Sex,
                Friends=friends.Result,
                Languages=user.Languages,
                IsEditHidden = false

            });
        }

        [Authorize, HttpGet, ActionName("User")]
        public async Task<ActionResult> UserInfo(long id)
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());
            UserProfileDTO user = await soc.Users.GetByPublicIdAsync(id);
            if (user.Id == soc.Id)
                return RedirectToAction("Profile");
            await soc.Friends.Counters.FriendsCounters();

            #region Parallel operations

            var newFriends = soc.Friends.Counters.CountRequestsAync();
            var friendsCountTask = soc.Friends.Counters.CountFriendsAync();
            var unread = soc.Messages.GetUnreadAsync();
            var avatar = soc.Users.GetAvatarAsync();
            var myPublicId = soc.Users.GetPublicIdAsync();
            var friendsTask = soc.Friends.GetFriendsAsync();
            var followers = soc.Friends.GetFollowersAsync();
            var followed = soc.Friends.GetFollowedAsync();

            #endregion

            

            var userSoc = new SocialNetworkManager(user.Id);
            var friends = userSoc.Friends.GetFriendsAsync(6);
            var profileModel = new ProfileModel
            {
                PublicId = user.PublicId,
                About = user.About,
                AboutIsHidden = user.AboutIsHidden,
                Activity = user.Activity,
                ActivityIsHidden = user.ActivityIsHidden,
                Address = user.Address,
                Avatar = user.Avatar,
                BirthDate = user.BirthDate,
                BirthDateIsHidden = user.BirthDateIsHidden,
                City = user.City,
                Email = user.Email,
                EmailIsHidden = user.EmailIsHidden,
                LastName = user.LastName,
                Name = user.Name,
                Role = user.Role,
                Sex = user.Sex,
                Languages = user.Languages,
                IsEditHidden = soc.Id != user.Id
            };

            await Task.WhenAll(friendsTask, followers, followed);

            if (friendsTask.Result.Select(u => u.Id).Contains(user.Id))
                profileModel.IsFriend = true;
            else if (followers.Result.Select(u => u.Id).Contains(user.Id))
                profileModel.IsFollower = true;
            else if (followed.Result.Select(u => u.Id).Contains(user.Id))
                profileModel.IsFollowed = true;

            await Task.WhenAll(friends, newFriends, friendsCountTask, unread, avatar, myPublicId);
            profileModel.Friends = friends.Result;
            ViewBag.NewFriends = newFriends.Result;
            ViewBag.Friends = friendsCountTask.Result;
            ViewBag.UnRead = unread.Result;
            ViewBag.Avatar = avatar.Result;
            ViewBag.MyPublicId = myPublicId.Result;
            return View("Profile", profileModel);
        }

        [Authorize]
        public async Task<ActionResult> Settings()
        {
           

            var soc = new SocialNetworkManager(User.Identity.GetUserId());
            UserProfileDTO user = await soc.Users.GetAsync(soc.Id);
            await soc.Friends.Counters.FriendsCounters();

            #region Parallel operations
            var getCountries = soc.Database.GetAllCountriesAsync();
            var getCities = soc.Database.GetCitiesAsync(user.City.CountryId);
            var unread = soc.Messages.GetUnreadAsync();
            var avatar = soc.Users.GetAvatarAsync();
            var newFriends = soc.Friends.Counters.CountRequestsAync();
            #endregion

            Mapper.Initialize(cnf =>
            {
                cnf.CreateMap<CityDTO, CityViewModel>();
                cnf.CreateMap<CountryDTO, CountryViewModel>();
            });
            await Task.WhenAll(getCountries, getCities);
            var model = new SettingsViewModel
            {
                About = user.About,
                AboutIsHidden = user.AboutIsHidden,
                Activity = user.Activity,
                ActivityIsHidden = user.ActivityIsHidden,
                Address = user.Address,
                //BirthDate = user.BirthDate,
                BirthDateIsHidden = user.BirthDateIsHidden,
                CityId = user.CityId,
                CountryId = user.City.CountryId,
                Email = user.Email,
                EmailIsHidden = user.EmailIsHidden,
                Surname = user.LastName,
                Name = user.Name,
                Sex = user.Sex,
                Countries = Mapper.Map<IEnumerable<CountryViewModel>>(getCountries.Result),
                Cities = Mapper.Map<IEnumerable<CityViewModel>>(getCities.Result)
            };

            await Task.WhenAll(unread, avatar, newFriends);
            ViewBag.UnRead = unread.Result;
            ViewBag.Avatar = avatar.Result;
            ViewBag.NewFriends = newFriends.Result;

            return View(model);
        }

        [Authorize, HttpPost]
        public async Task<ActionResult> Settings(SettingsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var soc = new SocialNetworkManager(User.Identity.GetUserId());

            UserProfileDTO user = await soc.Users.GetAsync(soc.Id);

            await soc.Friends.Counters.FriendsCounters();
            #region Parallel operations
            var newFriends = soc.Friends.Counters.CountRequestsAync();
            var friends = soc.Friends.Counters.CountFriendsAync();
            var unread = soc.Messages.GetUnreadAsync();
            var avatar = soc.Users.GetAvatarAsync();
            var countries = soc.Database.GetAllCountriesAsync();
            #endregion
            Mapper.Initialize(cnf =>
            {
                cnf.CreateMap<CityDTO, CityViewModel>();
                cnf.CreateMap<CountryDTO, CountryViewModel>();
            });


            user.About = model.About;
            user.AboutIsHidden = model.AboutIsHidden;
            user.Activity = model.Activity;
            user.ActivityIsHidden = model.ActivityIsHidden;
            user.Address = model.Address;
            //user.BirthDate = model.BirthDate;
            user.BirthDateIsHidden = model.BirthDateIsHidden;
            user.CityId = model.CityId;
            user.Email = model.Email;
            user.EmailIsHidden = model.EmailIsHidden;
            user.LastName = model.Surname;
            user.Name = model.Name;
            user.Sex = model.Sex;
            await soc.Users.UpdateAsync(user);
            await Task.WhenAll(newFriends, friends, unread, avatar,countries);
            ViewBag.NewFriends = newFriends.Result;
            ViewBag.Friends = friends.Result;
            ViewBag.UnRead = unread.Result;
            ViewBag.Avatar = avatar.Result;
            model.Countries = Mapper.Map<IEnumerable<CountryViewModel>>(countries.Result);
            model.Cities = Mapper.Map<IEnumerable<CityViewModel>>(await soc.Database.GetCitiesAsync((long)model.CountryId));
            return View(model);
        }
        [AjaxOnly]
        public async Task<ActionResult> Cities(long countryId)
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());

            Mapper.Initialize(cnf =>
            {
                cnf.CreateMap<CityDTO, CityViewModel>();
            });

            var cities = Mapper.Map<IEnumerable<CityViewModel>>(await soc.Database.GetCitiesAsync(countryId));

            return PartialView("Cities", cities);
        }
    }
}