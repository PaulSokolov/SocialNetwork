using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using SocialNetwork.Models.SignalRModels;

namespace WEB.Hubs
{
    [Authorize]
    public class ConnectionHub : Hub
    {
        private readonly List<User> _users = new List<User>();

        public override Task OnConnected()
        {
            var id = Context.ConnectionId;

            var userIdentityId = Context.User.Identity.GetUserId();
            var userIdentityName = Context.User.Identity.Name;

            if (_users.All(x => x.ConnectionId != id))
            {
                _users.Add(new User { ConnectionId = id, PublicId = userIdentityId, Name = userIdentityName });
            }

            Groups.Add(Context.ConnectionId, userIdentityId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = _users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (item == null) return base.OnDisconnected(stopCalled);

            _users.Remove(item);
            var id = Context.ConnectionId;
            Clients.All.onUserDisconnected(id, item.Name);

            return base.OnDisconnected(stopCalled);
        }
    }
}