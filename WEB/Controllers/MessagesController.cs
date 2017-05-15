using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using SocialNetwork.Models;
using WEB.Filters;
using WEB.Hubs;

namespace WEB.Controllers
{
    [System.Web.Mvc.Authorize]
    public class MessagesController : Controller
    {

        public async Task<ActionResult> Index()
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());
            await soc.Friends.Counters.FriendsCounters();

            #region Parallel operations

            var unread = soc.Messages.GetUnreadAsync();
            var avatar = soc.Users.GetAvatarAsync();
            var newFriends = soc.Friends.Counters.CountRequestsAync();

            #endregion

            

            var dialogs = await soc.Messages.GetLastMessagesAsync();
            var dialogModels = dialogs.AsParallel().Select(lastMessage =>
            {
                if (lastMessage.FromUserId == soc.Id)
                    return new DialogModel
                    {
                        Name = lastMessage.ToUser.Name,
                        Surname = lastMessage.ToUser.LastName,
                        MyAvatar = lastMessage.FromUser.Avatar,
                        SenderAvatar = lastMessage.ToUser.Avatar,
                        Body = lastMessage.Body,
                        LastMessageTime = lastMessage.PostedDate,
                        PublicId = lastMessage.ToUser.PublicId,
                        IsRead = lastMessage.IsRead
                    };
                return new DialogModel
                {
                    Name = lastMessage.FromUser.Name,
                    Surname = lastMessage.FromUser.LastName,
                    SenderAvatar = lastMessage.FromUser.Avatar,
                    Body = lastMessage.Body,
                    LastMessageTime = lastMessage.PostedDate,
                    PublicId = lastMessage.FromUser.PublicId,
                    IsRead = lastMessage.IsRead
                };
            });
            await Task.WhenAll(unread, avatar, newFriends);

            ViewBag.Unread = unread.Result;
            ViewBag.Avatar = avatar.Result;
            ViewBag.NewFriends = newFriends.Result;

            return View(dialogModels.OrderByDescending(d => d.LastMessageTime));
        }

        public async Task<ActionResult> Dialog(long id)
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());

            await soc.Friends.Counters.FriendsCounters();
            #region Parallel operations
            
            var avatar = soc.Users.GetAvatarAsync();
            var newFriends = soc.Friends.Counters.CountRequestsAync(); 
            #endregion

            

            var messages = await soc.Messages.GetDialogAsync(id, 0);

            var dialog = messages.AsParallel().Select(mes =>
            {
                var message = new MessageModel
                {
                    Id = mes.Id,
                    Avatar = mes.FromUser.Avatar,
                    Body = mes.Body,
                    Name = mes.FromUser.Name,
                    PostedTime = mes.PostedDate,
                    Surname = mes.FromUser.LastName,
                    PublicId = mes.FromUser.PublicId,
                    IsRead = mes.IsRead
                };
                if (mes.FromUserId == soc.Id)
                {
                    message.IsMy = true;
                }
                if (!mes.IsRead && mes.FromUserId != soc.Id)
                {
                    message.IsRead = (soc.Messages.Read(mes.Id)).IsRead;
                }
                return message;
            });
            var unread = soc.Messages.GetUnreadAsync();
            await Task.WhenAll(unread, avatar, newFriends);

            ViewBag.UnRead = unread.Result;
            ViewBag.RecipientId = id;
            ViewBag.Avatar = avatar.Result;
            ViewBag.NewFriends = newFriends.Result;

            return View(dialog.OrderBy(m => m.PostedTime));
        }

        [HttpPost,AjaxOnly]
        public async Task<ActionResult> Get(long id, int lastIndex)
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());

            var messages = await soc.Messages.GetDialogAsync(id, lastIndex);

            var dialog = messages.AsParallel().OrderBy(mes=>mes.PostedDate).Select(mes =>
            {
                var message = new MessageModel
                {
                    Id = mes.Id,
                    Avatar = mes.FromUser.Avatar,
                    Body = mes.Body,
                    Name = mes.FromUser.Name,
                    PostedTime = mes.PostedDate,
                    Surname = mes.FromUser.LastName,
                    PublicId = mes.FromUser.PublicId,
                    IsRead = mes.IsRead
                };
                if (mes.FromUserId == soc.Id)
                {
                    message.IsMy = true;
                }
                if (!mes.IsRead && mes.FromUserId != soc.Id)
                {
                    message.IsRead = (soc.Messages.Read(mes.Id)).IsRead;
                }
                return message;
            });

            return PartialView("Partial/Messages",dialog);
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Send(long recipientId, string message)
        {
            Dictionary<string,Task<UserProfileDTO>> getUsersParallel = new Dictionary<string,Task<UserProfileDTO>>();

            var soc = new SocialNetworkManager(User.Identity.GetUserId());

            var mes = await soc.Messages.SendAsync(recipientId, message);

            getUsersParallel.Add("fromUser", soc.Users.GetAsync(mes.FromUserId));
            getUsersParallel.Add("toUser", soc.Users.GetAsync(mes.ToUserId));
            await Task.WhenAll(getUsersParallel.Select(u=>u.Value));

            mes.FromUser = getUsersParallel["fromUser"].Result;
            mes.ToUser = getUsersParallel["toUser"].Result;

            var messageModel = new MessageModel
            {
                Id = mes.Id,
                Avatar = mes.FromUser.Avatar,
                Body = mes.Body,
                Name = mes.FromUser.Name,
                PostedTime = mes.PostedDate,
                Surname = mes.FromUser.LastName,
                IsRead = mes.IsRead,
                PublicId = mes.FromUser.PublicId
            };

            string messageContent = RenderRazorViewToString("Partial/Message", messageModel);

            //SignalR methods
            SendMessage(mes.ToUser.Id, messageContent);
            ShowMessageNotification(mes.ToUser.Id, RenderRazorViewToString("../Shared/MessageNotification", messageModel));

            var recipient = new SocialNetworkManager(mes.ToUser.Id);

            UpdateMessageCounter(mes.ToUser.Id, await recipient.Messages.GetUnreadAsync());

            return Content(messageContent);
        }

        [HttpPost, AjaxOnly]
        public async Task<ActionResult> Read(long messageId)
        {
            var soc = new SocialNetworkManager(User.Identity.GetUserId());
            var mes = await soc.Messages.ReadAsync(messageId);
            //SignalR methods
            ReadMessage(mes.FromUserId, mes.Id);

            var recipient = new SocialNetworkManager(mes.ToUserId);

            UpdateMessageCounter(mes.ToUserId, await recipient.Messages.GetUnreadAsync());

            return Json(new{isRead = mes.IsRead});
        }

        

        #region SignalR methods
        private void SendMessage(string publicId, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(publicId).addMessage(message);
        }

        private void ReadMessage(string id, long messageId)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(id).readMessage(messageId);
        }

        private void ShowMessageNotification(string publicId, string notification)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(publicId).notification(notification);
        }

        private void UpdateMessageCounter(string publicId, int count)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(publicId).messageCounter(count);
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