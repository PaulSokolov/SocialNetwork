using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using BusinessLayer.Infrastructure;
using BusinessLayer.Interfaces;
using Microsoft.AspNet.Identity.Owin;
using SocialNetwork.Models;
using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SocialNetwork.Models.AdminViewModels;

namespace WEB.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return PartialView("Partial/CreateUser");
        }

        [HttpGet]
        public ActionResult DeleteUser()
        {
            return PartialView("Partial/DeleteUser");
        }

        [HttpGet]
        public ActionResult CreateRole()
        {
            return PartialView("Partial/CreateRole");
        }

        [HttpGet]
        public ActionResult DeleteRole()
        {
            List<RoleModel> roles = new List<RoleModel>();
            foreach (var role in UserService.GetRoles())
            {
                roles.Add(new RoleModel { Name = role });
            }

            return PartialView("Partial/DeleteRole", roles);
        }

        [HttpGet]
        public ActionResult ManageRoles()
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            List<ManageUserRolesModel> usersWithRoles = new List<ManageUserRolesModel>();
            foreach (var user in soc.Users.Search())
            {
                var userRole = UserService.GetRoles(user.Id);
                var availableRoles = UserService.GetRoles().Except(userRole).ToList();
                usersWithRoles.Add(new ManageUserRolesModel
                {
                    Name = user.Name,
                    Surname = user.LastName,
                    PublicId = user.PublicId,
                    Avatar = user.Avatar,
                    Roles = userRole,
                    AvailableRoles = availableRoles
                });

            }
            return PartialView("Partial/ManageRoles", usersWithRoles);
        }

        [HttpPost]
        public async Task<ActionResult> AddToRole(long publicId, string role)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var user = soc.Users.GetByPublicId(publicId);
            await UserService.AddToRoleAsync(user.Id, role);
            var userRole = UserService.GetRoles(user.Id);
            var availableRoles = UserService.GetRoles().Except(userRole).ToList();
            var model = new ManageUserRolesModel
            {
                Name = user.Name,
                Surname = user.LastName,
                PublicId = user.PublicId,
                Avatar = user.Avatar,
                Roles = userRole,
                AvailableRoles = availableRoles
            };

            return PartialView("Partial/ManageRoleRow", model);
        }

        [HttpPost]
        public async Task<ActionResult> RemoveFromRole(long publicId, string role)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var user = soc.Users.GetByPublicId(publicId);
            await UserService.RemoveFromRoleAsync(user.Id, role);
            var userRole = UserService.GetRoles(user.Id);
            var availableRoles = UserService.GetRoles().Except(userRole).ToList();
            var model = new ManageUserRolesModel
            {
                Name = user.Name,
                Surname = user.LastName,
                PublicId = user.PublicId,
                Avatar = user.Avatar,
                Roles = userRole,
                AvailableRoles = availableRoles
            };

            return PartialView("Partial/ManageRoleRow", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(RegisterModel model)
        {

            if (ModelState.IsValid)
            {
                UserProfileDTO userDto = new UserProfileDTO
                {

                    Email = model.Email,
                    Password = model.Password,
                    Name = model.Name,
                    LastName = model.Surname,
                    Role = "user",
                    BirthDate = model.BirthDate,
                    ActivatedDate = DateTime.Now

                };
                OperationDetails operationDetails = await UserService.Create(userDto);
                if (operationDetails.Succedeed)
                    return View("SuccessRegister");
                else
                    ModelState.AddModelError(string.Empty, operationDetails.Message);
            }

            return PartialView("Partial/CreateUser", model);
        }
        //TODO: design
        [HttpPost]
        public async Task<ActionResult> DeleteUser(long? publicId)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                UserProfileDTO userDto = soc.Users.GetByPublicId((long)publicId);
                OperationDetails operationDetails = await UserService.Delete(userDto);
                if (operationDetails.Succedeed)
                    return View("SuccessRegister");
                else
                    ModelState.AddModelError(string.Empty, operationDetails.Message);
                return Content($"<div class=\"row\">{userDto.Name} {userDto.LastName} deleted successfully</div>");
            }

            return HttpNotFound();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(RoleModel role)
        {
            if (ModelState.IsValid)
            {
                OperationDetails operationDetails = await UserService.CreateRole(role.Name);
                if (operationDetails.Succedeed)
                    return View("SuccessRegister");
                else
                    ModelState.AddModelError(string.Empty, operationDetails.Message);
            }

            return PartialView("Partial/CreateRole", role);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteRole(string role)
        {
            OperationDetails operationDetails = await UserService.DeleteRole(role);
            if (operationDetails.Succedeed)
                return View("SuccessRegister");
            else
                return View();
        }

        [HttpPost]
        public ActionResult Search(string search, int? ageFrom, int? ageTo, long? cityId, long? countryId, string activityConcurence, string aboutConcurence, int? sex, short? sort)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            ViewBag.Unread = soc.Messages.UnRead;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;
            ViewBag.MyPublicId = soc.Users.PublicId;

            var users = soc.Users.Search(search, ageFrom, ageTo, cityId, countryId, activityConcurence, aboutConcurence, sex, sort);
            List<UserDeleteModel> models = new List<UserDeleteModel>();
            foreach (var user in users)
            {
                var model = new UserDeleteModel
                {
                    Name = user.Name,
                    Surname = user.LastName,
                    Address = user.Address,
                    Avatar = user.Avatar,
                    PublicId = user.PublicId
                };
                models.Add(model);
            }

            return PartialView("Partial/Users", models);
        }
    }
}