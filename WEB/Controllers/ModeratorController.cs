using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLayer.BusinessModels;
using Microsoft.AspNet.Identity;
using SocialNetwork.Models.AdminViewModels;
using SocialNetwork.Models.ModeratorModels;

namespace WEB.Controllers
{
    [Authorize(Roles = "admin, moderator")]
    public class ModeratorController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserMessages(long? publicId)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            ViewBag.PublicId = publicId;

            var models = new List<MessageModeratorModel>();
            if (publicId == null) return PartialView("Partial/UserMessages", models);

            var user = soc.Users.GetByPublicId((long)publicId);
            var messages = soc.Messages.GetAllMessagesByUserId(user.Id);

            foreach (var message in messages)
            {
                models.Add(new MessageModeratorModel
                {
                    Id = message.Id,
                    UserAvatar = message.FromUser.Avatar,
                    UserName = message.FromUser.Name,
                    UserSurname = message.FromUser.LastName,
                    UserPublicId = message.FromUser.PublicId,
                    RecipientAvatar = message.ToUser.Avatar,
                    RecipientName = message.ToUser.Name,
                    RecipientSurname = message.ToUser.LastName,
                    RecipientPublicId = message.ToUser.PublicId,
                    Body = message.Body,
                    PostedDate = message.PostedDate,
                    LastModifiedDate = message.ModifiedDate

                });
            }

            return PartialView("Partial/UserMessages", models);
        }

        [HttpPost]
        public ActionResult UpdateMessage(long? id, string body)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var updatedMessage = soc.Messages.Moderate((long)id, body);
            var model = new MessageModeratorModel
            {
                Id = updatedMessage.Id,
                UserAvatar = updatedMessage.FromUser.Avatar,
                UserName = updatedMessage.FromUser.Name,
                UserSurname = updatedMessage.FromUser.LastName,
                UserPublicId = updatedMessage.FromUser.PublicId,
                RecipientAvatar = updatedMessage.ToUser.Avatar,
                RecipientName = updatedMessage.ToUser.Name,
                RecipientSurname = updatedMessage.ToUser.LastName,
                RecipientPublicId = updatedMessage.ToUser.PublicId,
                Body = updatedMessage.Body,
                PostedDate = updatedMessage.PostedDate,
                LastModifiedDate = updatedMessage.ModifiedDate

            };

            return PartialView("Partial/MessageRow", model);
        }

        [HttpGet]
        public ActionResult DeleteMessage(long? id)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var deleted = id != null && soc.Messages.Delete((long)id);
            if (deleted)
                return Content($"<tr><td></td><td></td><td>Message {id} deleted successfully</td><td></td><td></td></tr>");
            return Content($"<tr><td></td><td></td><td>An error occurred while deleting message {id}. Try to refresh the page and try again</td><td></td><td></td></tr>");
        }

        [HttpPost]
        public ActionResult SearchMessages(long? publicId, string body)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var user = soc.Users.GetByPublicId((long)publicId);
            var messages = soc.Messages.GetAllMessagesByUserId(user.Id).Where(m => m.Body.ToLower().Contains(body.ToLower()));

            var models = messages.Select(message => new MessageModeratorModel
                {
                    Id = message.Id,
                    UserAvatar = message.FromUser.Avatar,
                    UserName = message.FromUser.Name,
                    UserSurname = message.FromUser.LastName,
                    UserPublicId = message.FromUser.PublicId,
                    RecipientAvatar = message.ToUser.Avatar,
                    RecipientName = message.ToUser.Name,
                    RecipientSurname = message.ToUser.LastName,
                    RecipientPublicId = message.ToUser.PublicId,
                    Body = message.Body,
                    PostedDate = message.PostedDate,
                    LastModifiedDate = message.ModifiedDate
                })
                .ToList();

            return PartialView("Partial/MessagesTable", models);
        }

        public ActionResult AutocompleteSearchMessages(string term)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var searchResult = soc.Users.Search(term);
            var users = searchResult.Select(u => new { name = u.Name, avatar = u.Avatar, lastName = u.LastName, publicId = u.PublicId, address = u.Address.Length > 30 ? u.Address.Substring(0, 30) : u.Address }).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(string search, int? ageFrom, int? ageTo, long? cityId, long? countryId, string activityConcurence, string aboutConcurence, int? sex, short? sort)
        {
            var soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var users = soc.Users.Search(search, ageFrom, ageTo, cityId, countryId, activityConcurence, aboutConcurence, sex, sort);

            var models = new List<UserDeleteModel>();

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