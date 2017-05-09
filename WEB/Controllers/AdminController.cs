using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using BusinessLayer.Infrastructure;
using BusinessLayer.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SocialNetwork.Models;
using SocialNetwork.Models.AdminViewModels;

namespace WEB.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private IUserService UserService => HttpContext.GetOwinContext().GetUserManager<IUserService>();

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
            var roles = new List<RoleModel>();
            foreach (var role in UserService.GetRoles())
            {
                roles.Add(new RoleModel { Name = role });
            }

            return PartialView("Partial/DeleteRole", roles);
        }

        [HttpGet]
        public ActionResult ManageRoles()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var usersWithRoles = new List<ManageUserRolesModel>();
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
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

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
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

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
            if (!ModelState.IsValid) return PartialView("Partial/CreateUser", model);

            var userDto = new UserProfileDTO
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
            ModelState.AddModelError(string.Empty, operationDetails.Message);

            return PartialView("Partial/CreateUser", model);
        }
        //TODO: design
        [HttpPost]
        public async Task<ActionResult> DeleteUser(long? publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            if (!ModelState.IsValid) return HttpNotFound();

            UserProfileDTO userDto = soc.Users.GetByPublicId((long)publicId);
            OperationDetails operationDetails = await UserService.Delete(userDto);

            if (operationDetails.Succedeed)
                return View("SuccessRegister");

            ModelState.AddModelError(string.Empty, operationDetails.Message);
            return Content($"<div class=\"row\">{userDto.Name} {userDto.LastName} deleted successfully</div>");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(RoleModel role)
        {
            if (!ModelState.IsValid) return PartialView("Partial/CreateRole", role);

            OperationDetails operationDetails = await UserService.CreateRole(role.Name);

            if (operationDetails.Succedeed)
                return View("SuccessRegister");

            ModelState.AddModelError(string.Empty, operationDetails.Message);

            return PartialView("Partial/CreateRole", role);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteRole(string role)
        {
            OperationDetails operationDetails = await UserService.DeleteRole(role);
            return operationDetails.Succedeed ? View("SuccessRegister") : View();
        }

        [HttpPost]
        public ActionResult Search(string search, int? ageFrom, int? ageTo, long? cityId, long? countryId, string activityConcurence, string aboutConcurence, int? sex, short? sort)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            ViewBag.Unread = soc.Messages.UnRead;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;
            ViewBag.MyPublicId = soc.Users.PublicId;

            var users = soc.Users.Search(search, ageFrom, ageTo, cityId, countryId, activityConcurence, aboutConcurence, sex, sort);

            var models = users.Select(user => new UserDeleteModel
                {
                    Name = user.Name,
                    Surname = user.LastName,
                    Address = user.Address,
                    Avatar = user.Avatar,
                    PublicId = user.PublicId
                })
                .ToList();

            return PartialView("Partial/Users", models);
        }
    }
}