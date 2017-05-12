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

namespace WEB.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, Authorize]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            ViewBag.Avatar = soc.Users.Avatar;

            if (file != null && (file.ContentLength > 0 || !file.ContentType.Contains("image")))
                try
                {
                    
                    var filePath = $"/Images/avatar{User.Identity.GetUserId()}.jpg";
                    string path = Path.Combine(Server.MapPath("~/Images"),
                                               Path.GetFileName("avatar" + User.Identity.GetUserId()+".jpg"));
                    var user = soc.Users.Get(User.Identity.GetUserId());
                    user.Avatar = filePath;
                    user = soc.Users.Update(user);

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
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            UserProfileDTO user = soc.Users.Get(soc.Id);

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;

            await soc.Friends.Counters.FriendsCounters();
            var newFriendsTask = soc.Friends.Counters.CountRequestsAync();
            var friendsTask = soc.Friends.Counters.CountFriendsAync();
            
            ViewBag.MyPublicId = soc.Users.PublicId;

            var friends = soc.Friends.GetFriendsAsync();

            await Task.WhenAll(friends, newFriendsTask, friendsTask);
            ViewBag.NewFriends = newFriendsTask.Result;
            ViewBag.Friends = friendsTask.Result;

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
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            UserProfileDTO user = soc.Users.GetByPublicId(id);

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            await soc.Friends.Counters.FriendsCounters();
            var newFriendsTask = soc.Friends.Counters.CountRequestsAync();
            var friendsTask = soc.Friends.Counters.CountFriendsAync();
            
            ViewBag.MyPublicId = soc.Users.PublicId;
            var userSoc = new SocialNetworkFunctionalityUser(user.Id);
            var friends = userSoc.Friends.GetFriendsAsync();
            
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
            if (soc.Friends.GetFriends().Select(u => u.Id).Contains(user.Id))
                profileModel.IsFriend = true;
            else if (soc.Friends.GetFollowers().Select(u => u.Id).Contains(user.Id))
                profileModel.IsFollower = true;
            else if (soc.Friends.GetFollowed().Select(u => u.Id).Contains(user.Id))
                profileModel.IsFollowed = true;
            await Task.WhenAll(friends, newFriendsTask, friendsTask);
            profileModel.Friends = friends.Result;
            ViewBag.NewFriends = newFriendsTask.Result;
            ViewBag.Friends = friendsTask.Result;
            return View("Profile", profileModel);
        }

        [Authorize]
        public async Task<ActionResult> Settings()
        {
           

            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());
            UserProfileDTO user = soc.Users.Get(soc.Id);

            var getCountriesTask = soc.Database.GetAllCountriesAsync();
            var getCitiesTask = soc.Database.GetCitiesAsync(user.City.CountryId);
            

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            await soc.Friends.Counters.FriendsCounters();
            ViewBag.NewFriends = await soc.Friends.Counters.CountRequestsAync();

            Mapper.Initialize(cnf =>
            {
                cnf.CreateMap<CityDTO, CityViewModel>();
                cnf.CreateMap<CountryDTO, CountryViewModel>();
            });
            await Task.WhenAll(getCountriesTask, getCitiesTask);
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
                Name = user.LastName,
                Sex = user.Sex,
                Countries = Mapper.Map<IEnumerable<CountryViewModel>>(getCountriesTask.Result),
                Cities = Mapper.Map<IEnumerable<CityViewModel>>(getCitiesTask.Result)
            };
            return View(model);
        }

        [Authorize, HttpPost]
        public async Task<ActionResult> Settings(SettingsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            UserProfileDTO user = soc.Users.Get(soc.Id);

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            await soc.Friends.Counters.FriendsCounters();
            ViewBag.NewFriends = await soc.Friends.Counters.CountRequestsAync();
            ViewBag.Friends = await soc.Friends.Counters.CountFriendsAync();

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
            soc.Users.Update(user);
            return View(model);
        }

        public async Task<ActionResult> Cities(long countryId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            Mapper.Initialize(cnf =>
            {
                cnf.CreateMap<CityDTO, CityViewModel>();
            });

            var cities = Mapper.Map<IEnumerable<CityViewModel>>(await soc.Database.GetCitiesAsync(countryId));

            return PartialView("Cities", cities);
        }
    }
}