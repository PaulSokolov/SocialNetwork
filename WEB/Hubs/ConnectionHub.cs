using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using SocialNetwork.Models.SignalRModels;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace WEB.Hubs
{
    [Authorize]
    public class ConnectionHub : Hub
    {
        List<User> Users = new List<User>();

        public override Task OnConnected()
        {
            var id = Context.ConnectionId;

            var userIdentityId = Context.User.Identity.GetUserId();
            var userIdentityName = Context.User.Identity.Name;

            if (!Users.Any(x => x.ConnectionId == id))
            {
                Users.Add(new User { ConnectionId = id, PublicId = userIdentityId, Name = userIdentityName });
            }

            Groups.Add(Context.ConnectionId, userIdentityId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if(item != null)
            {
                Users.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}