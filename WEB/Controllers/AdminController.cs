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
using WEB.Filters;

namespace WEB.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private IUserService UserService => HttpContext.GetOwinContext().GetUserManager<IUserService>();

        public async Task<ActionResult> Index()
        {
            ViewBag.Avatar = await new SocialNetworkFunctionalityUser(User.Identity.GetUserId()).Users.GetAvatarAsync();
            return View();
        }

        [HttpGet, AjaxOnly]
        public ActionResult CreateUser()
        {
            return PartialView("Partial/CreateUser");
        }

        [HttpGet, AjaxOnly]
        public ActionResult DeleteUser()
        {
            return PartialView("Partial/DeleteUser");
        }

        [HttpGet, AjaxOnly]
        public ActionResult CreateRole()
        {
            return PartialView("Partial/CreateRole");
        }

        [HttpGet, AjaxOnly]
        public async Task<ActionResult> DeleteRole()
        {
            var roles = new List<RoleModel>();
            foreach (var role in await UserService.GetRoles())
            {
                roles.Add(new RoleModel { Name = role });
            }

            return PartialView("Partial/DeleteRole", roles);
        }

        [HttpGet, AjaxOnly]
        public async Task<ActionResult> ManageRoles()
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var usersWithRoles = new List<ManageUserRolesModel>();
            foreach (var user in await soc.Users.SearchAsync())
            {
                var userRole = await UserService.GetRoles(user.Id);
                var availableRoles = (await UserService.GetRoles()).Except(userRole).ToList();
                
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
            //Parallel.ForEach(await soc.Users.SearchAsync(), async user =>
            //{
            //    var userRole = await UserService.GetRoles(user.Id);
            //    var availableRoles = (await UserService.GetRoles()).Except(userRole).ToList();
            //    lock (usersWithRoles)
            //    {
            //        usersWithRoles.Add(new ManageUserRolesModel
            //        {
            //            Name = user.Name,
            //            Surname = user.LastName,
            //            PublicId = user.PublicId,
            //            Avatar = user.Avatar,
            //            Roles = userRole,
            //            AvailableRoles = availableRoles
            //        });
            //    }
            //});
            return PartialView("Partial/ManageRoles", usersWithRoles);
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> AddToRole(long publicId, string role)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var user = await soc.Users.GetByPublicIdAsync(publicId);
            await UserService.AddToRoleAsync(user.Id, role);
            var userRole = await UserService.GetRoles(user.Id);
            var availableRoles = (await UserService.GetRoles()).Except(userRole).ToList();
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

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> RemoveFromRole(long publicId, string role)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var user = await soc.Users.GetByPublicIdAsync(publicId);
            await UserService.RemoveFromRoleAsync(user.Id, role);
            var userRole = await UserService.GetRoles(user.Id);
            var availableRoles = (await UserService.GetRoles()).Except(userRole).ToList();
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

        [HttpPost, ValidateAntiForgeryToken, AjaxOnly]
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
        [HttpPost, AjaxOnly]
        public async Task<ActionResult> DeleteUser(long? publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            if (!ModelState.IsValid) return HttpNotFound();

            UserProfileDTO userDto = await soc.Users.GetByPublicIdAsync((long)publicId);
            OperationDetails operationDetails = await UserService.Delete(userDto);

            if (operationDetails.Succedeed)
                return View("SuccessRegister");

            ModelState.AddModelError(string.Empty, operationDetails.Message);
            return Content($"<div class=\"row\">{userDto.Name} {userDto.LastName} deleted successfully</div>");
        }

        [HttpPost, ValidateAntiForgeryToken, AjaxOnly]
        public async Task<ActionResult> CreateRole(RoleModel role)
        {
            if (!ModelState.IsValid) return PartialView("Partial/CreateRole", role);

            OperationDetails operationDetails = await UserService.CreateRole(role.Name);

            if (operationDetails.Succedeed)
                return View("SuccessRegister");

            ModelState.AddModelError(string.Empty, operationDetails.Message);

            return PartialView("Partial/CreateRole", role);
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> DeleteRole(string role)
        {
            OperationDetails operationDetails = await UserService.DeleteRole(role);
            return operationDetails.Succedeed ? View("SuccessRegister") : View();
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Search(string search, int? ageFrom, int? ageTo, long? cityId, long? countryId, string activityConcurence, string aboutConcurence, int? sex, short? sort)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());
            //await soc.Friends.Counters.FriendsCounters();
            //#region Parallel operations
            //var unread = soc.Messages.GetUnreadAsync();
            //var newFriends = soc.Friends.Counters.CountRequestsAync();
            //var myPublicId = soc.Users.GetPublicIdAsync();
            //#endregion


            var users = await soc.Users.SearchAsync(search, activityConcurence: activityConcurence, aboutConcurence: aboutConcurence);
            var models = users.AsParallel().Select(user => new UserDeleteModel
            {
                Name = user.Name,
                Surname = user.LastName,
                Address = user.Address,
                Avatar = user.Avatar,
                PublicId = user.PublicId
            });

            //await Task.WhenAll(unread, newFriends, myPublicId);

            //ViewBag.Unread = unread.Result;
            //ViewBag.NewFriends = newFriends.Result;
            //ViewBag.MyPublicId = myPublicId.Result;

            return PartialView("Partial/Users", models);
        }
    }
}