using BusinessLayer.BusinessModels;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BusinessLayer.DTO;
using System.IO;
using AutoMapper;

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
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            ViewBag.Avatar = soc.Users.Avatar;

            if (file != null && file.ContentLength > 0 || !file.ContentType.Contains("image"))
                try
                {
                    
                    string filePath = $"/Images/avatar{User.Identity.GetUserId()}.jpg";
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
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return RedirectToAction("Profile");
        }

        [Authorize, ActionName("Profile")]
        public ActionResult ProfileInfo()
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            UserProfileDTO user = soc.Users.Get(soc.Id);

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;
            ViewBag.Friends = soc.Friends.Counters.Friends;
            ViewBag.MyPublicId = soc.Users.PublicId;

            var friends = user.Friends.Where(f => f.Confirmed == true && f.Deleted == false).Select(f => f.Friended).ToList();

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
                Name =user.LastName,
                Role =user.Role,
                Sex =user.Sex,
                Friends=friends,
                Languages=user.Languages,
                IsEditHidden = false

            });
        }

        [Authorize, HttpGet, ActionName("User")]
        public ActionResult UserInfo(long id)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            UserProfileDTO user = soc.Users.GetByPublicId(id);

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;
            ViewBag.Friends = soc.Friends.Counters.Friends;
            ViewBag.MyPublicId = soc.Users.PublicId;

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
                Name = user.LastName,
                Role = user.Role,
                Sex = user.Sex,
                Languages = user.Languages,
                IsEditHidden = !(soc.Id == user.Id)
            };
            if (soc.Friends.GetFriends().Select(u => u.Id).Contains(user.Id))
                profileModel.IsFriend = true;
            else if (soc.Friends.GetFollowers().Select(u => u.Id).Contains(user.Id))
                profileModel.IsFollower = true;
            else if (soc.Friends.GetFollowed().Select(u => u.Id).Contains(user.Id))
                profileModel.IsFollowed = true;

            var friends = user.Friends.Where(f => f.Confirmed == true && f.Deleted == false).Select(f => f.Friended).ToList();
            profileModel.Friends = friends;
            return View("Profile", profileModel);
        }

        [Authorize]
        public ActionResult Settings()
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            UserProfileDTO user = soc.Users.Get(soc.Id);

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;

            Mapper.Initialize(cnf =>
            {
                cnf.CreateMap<CityDTO, CityViewModel>();
                cnf.CreateMap<CountryDTO, CountryViewModel>();
            });

            var model = new SettingsViewModel
            {
                About = user.About,
                AboutIsHidden = user.AboutIsHidden,
                Activity = user.Activity,
                ActivityIsHidden = user.ActivityIsHidden,
                Address = user.Address,
                BirthDate = user.BirthDate,
                BirthDateIsHidden = user.BirthDateIsHidden,
                CityId = user.CityId,
                CountryId = user.City.CountryId,
                Email = user.Email,
                EmailIsHidden = user.EmailIsHidden,
                Surname = user.LastName,
                Name = user.LastName,
                Sex = user.Sex,
                Countries = Mapper.Map<IEnumerable<CountryViewModel>>(soc.Database.GetAllCountries()),
                Cities = Mapper.Map<IEnumerable<CityViewModel>>(soc.Database.GetCities(user.City.CountryId))
            };
            return View(model);
        }

        [Authorize, HttpPost]
        public ActionResult Settings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

                UserProfileDTO user = soc.Users.Get(soc.Id);

                ViewBag.UnRead = soc.Messages.UnRead;
                ViewBag.Avatar = soc.Users.Avatar;
                ViewBag.NewFriends = soc.Friends.Counters.Requests;
                ViewBag.Friends = soc.Friends.Counters.Friends;

                user.About = model.About;
                user.AboutIsHidden = model.AboutIsHidden;
                user.Activity = model.Activity;
                user.ActivityIsHidden = model.ActivityIsHidden;
                user.Address = model.Address;
                user.BirthDate = model.BirthDate;
                user.BirthDateIsHidden = model.BirthDateIsHidden;
                user.CityId = model.CityId;
                user.Email = model.Email;
                user.EmailIsHidden = model.EmailIsHidden;
                user.LastName = model.Surname;
                user.Name = model.Surname;
                user.Sex = model.Sex;
                soc.Users.Update(user);
            }
            return View(model);
        }

        public ActionResult Cities(long countryId)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            Mapper.Initialize(cnf =>
            {
                cnf.CreateMap<CityDTO, CityViewModel>();
            });

            var cities = Mapper.Map<IEnumerable<CityViewModel>>( soc.Database.GetCities(countryId));

            return PartialView("Cities", cities);
        }
    }
}