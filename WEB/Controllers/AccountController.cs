﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using BusinessLayer.Infrastructure;
using BusinessLayer.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SocialNetwork.Models;
using WEB.Filters;

namespace WEB.Controllers
{
    public class AccountController : Controller
    {
        private IUserService UserService => HttpContext.GetOwinContext().GetUserManager<IUserService>();

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            await SetInitialDataAsync();

            if (!ModelState.IsValid) return View(model);

            var userDto = new UserProfileDTO { Email = model.Email, Password = model.Password };
            ClaimsIdentity claim = await UserService.Authenticate(userDto);
            if (claim == null)
                ModelState.AddModelError("", @"Неверный логин или пароль.");
            else
            {
                AuthenticationManager.SignOut();
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);

                return RedirectToAction("Profile", "Home");
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Register()
        {
            var soc = new SocialNetworkManager("");
            ViewBag.Countries = await soc.Database.GetAllCountriesAsync();
            ViewBag.Cities = new List<CityDTO>();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            //await SetInitialDataAsync();

            if (!ModelState.IsValid) return View(model);

            var userDto = new UserProfileDTO
            {

                Email = model.Email,
                Password = model.Password,
                Name = model.Name,
                LastName=model.Surname,                   
                Role = "user",
                BirthDate = model.BirthDate,
                CityId = model.CityId,
                Address = model.Address,
                ActivatedDate = DateTime.Now,
                Avatar = "/Images/imj.jpg",
                Sex = model.Sex,
                

            };
            OperationDetails operationDetails = await UserService.Create(userDto);

            if (operationDetails.Succedeed)
            {
                ClaimsIdentity claim = await UserService.Authenticate(userDto);
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);

                return RedirectToAction("Profile", "Home");
            }

            ModelState.AddModelError(operationDetails.Property, operationDetails.Message);

            return View(model);
        }

        [Authorize, HttpPost, AjaxOnly]
        public async Task<ActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            if (oldPassword == newPassword)
            {
                return Content("New password should differ from the old one");
            }

            OperationDetails details = await UserService.ChangePassword(User.Identity.GetUserId(), oldPassword, newPassword);

            if (details.Succedeed)
                return Content("Password was changed successfully");

            return Content(details.Message);
        }

        private async Task SetInitialDataAsync()
        {
            var names = new List<string>
            {
                "Vasylyi",
                "Viktor",
                "Vitalyi",
                "Ojeg",
                "Alex",
                "Dmitryi",
                "Egor",
                "Mickle",
                "Ruslan",
                "Roman"
            };
            var lastNames = new List<string>
            {
                "Vasyliev",
                "Pobedov",
                "Klychko",
                "Pogosian",
                "Shybin",
                "Orel",
                "Marchenko",
                "Pruglo",
                "Kovalenko",
                "Kushnir"
            };
            var activities = new List<string>
            {
                "football",
                "valleyball",
                "movies",
                "dancing",
                "arts",
                "box",
                "driving",
                "walking"

            };
            var about = new List<string>
            {
                "money",
                "beuty",
                "pretty",
                "no alcohol",
                "no smoking",
                "sport",
                "food",
                "fruits"
            };
            for (int i = 0; i < 10; i++)
            {
                var rnd = new Random();
                var index = rnd.Next(1, 6);
                await UserService.SetInitialData(new UserProfileDTO
                {
                    Email = $"{names[i].ToLower()}.{lastNames[i].ToLower()}@mail.ru",
                    //UserName = "somemail@mail.ru",
                    Password = "tara",
                    Name = names[i],
                    LastName= lastNames[i],
                    Address = $"ул. Спортивная, д.{i}, кв.{i*10}",
                    Role = "user",
                    Avatar = "images/img.jpg",
                    CityId = i,
                    About= string.Join(", ", about[index], about[index + 1], about[index - 1]),
                    Activity=string.Join(", ", activities[index], activities[index + 1], activities[index - 1]),
                    BirthDate=DateTime.Now,
                    Sex=SexDTO.Male
                }, new List<string> { "user", "admin", "moderator" });
            }
            
        }
    }
}