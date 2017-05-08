﻿using BusinessLayer.BusinessModels;
using Microsoft.AspNet.Identity;
using SocialNetwork.Models;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using WEB.Hubs;

namespace WEB.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {

        public ActionResult Index()
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            ViewBag.Unread = soc.Messages.UnRead;
            ViewBag.Avatar = soc.Users.Avatar;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;

            List<DialogModel> dialogs = new List<DialogModel>();
            foreach (var lastMessage in soc.Messages.GetLastMessages())
            {
                if (lastMessage.FromUserId == soc.Id)
                    dialogs.Add(new DialogModel
                    {
                        Name = lastMessage.ToUser.Name,
                        Surname = lastMessage.ToUser.LastName,
                        MyAvatar = lastMessage.FromUser.Avatar,
                        SenderAvatar = lastMessage.ToUser.Avatar,
                        Body = lastMessage.Body,
                        LastMessageTime = lastMessage.PostedDate,
                        PublicId = lastMessage.ToUser.PublicId,
                        IsRead = lastMessage.IsRead
                    });
                else
                    dialogs.Add(new DialogModel
                    {
                        Name = lastMessage.FromUser.Name,
                        Surname = lastMessage.FromUser.LastName,
                        SenderAvatar = lastMessage.FromUser.Avatar,
                        Body = lastMessage.Body,
                        LastMessageTime = lastMessage.PostedDate,
                        PublicId = lastMessage.FromUser.PublicId,
                        IsRead = lastMessage.IsRead
                    });
            }
            return View(dialogs);
        }

        public ActionResult Dialog(long id)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            List<MessageModel> dialog = new List<MessageModel>();

            ViewBag.UnRead = soc.Messages.UnRead;
            ViewBag.RecipientId = id;
            ViewBag.Avatar = soc.Users.Avatar;
            ViewBag.NewFriends = soc.Friends.Counters.Requests;

            var messages = soc.Messages.GetDialog(id);

            foreach (var mes in messages)
            {
                MessageModel message = new MessageModel
                {
                    Avatar = mes.FromUser.Avatar,
                    Body = mes.Body,
                    Name = mes.FromUser.Name,
                    PostedTime = mes.PostedDate,
                    Surname = mes.FromUser.LastName,
                    PublicId = mes.FromUser.PublicId,
                    IsRead = mes.IsRead
                };
                if (!mes.IsRead && mes.FromUserId != soc.Id)
                {
                    message.IsRead = soc.Messages.ReadMessage(mes.Id).IsRead;
                }
                dialog.Add(message);
            }

            return View(dialog);
        }

        [HttpPost]
        public ActionResult Send(long recipientId, string message)
        {
            SocialNetworkFunctionalityUser soc = new SocialNetworkFunctionalityUser(User.Identity.GetUserId());

            var mes = soc.Messages.Send(recipientId, message);

            mes.FromUser = soc.Users.Get(mes.FromUserId);
            mes.ToUser = soc.Users.Get(mes.ToUserId);

            MessageModel messageModel = new MessageModel
            {
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

            SocialNetworkFunctionalityUser recipient = new SocialNetworkFunctionalityUser(mes.ToUser.Id);

            UpdateMessageCounter(mes.ToUser.Id, recipient.Messages.UnRead);

            return Content(messageContent);
        }

        #region SignalR methods
        private void SendMessage(string publicId, string message)
        {
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(publicId).addMessage(publicId, message);
        }

        private void ShowMessageNotification(string publicId, string notification)
        {
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
            context.Clients.Group(publicId).notification(notification);
        }
        private void UpdateMessageCounter(string publicId, int count)
        {
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
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