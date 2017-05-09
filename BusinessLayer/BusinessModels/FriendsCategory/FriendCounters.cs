using System.Collections.Generic;
using System.Linq;
using DataLayer.Entities;

namespace BusinessLayer.BusinessModels
{

    public partial class SocialNetworkFunctionalityUser
    {
        public partial class FriendsCategory
        {
            public class FriendCounters
            {
                private readonly FriendsCategory _friendsCategory;
                private readonly ICollection<Friend> _friends;

                public long Friends => _friends.Count(f => f.Confirmed && f.Deleted == false);

                public long Followers => _friends.Count(f => (f.Confirmed == false || f.Deleted) && f.RequestUserId != _friendsCategory._socialNetworkFunctionality.Id);

                public long Followed => _friends.Count(f => (f.Confirmed == false || f.Deleted) && f.RequestUserId == _friendsCategory._socialNetworkFunctionality.Id);

                public long Requests => _friends.Count(f => f.Confirmed == false && f.Deleted == false && f.RequestUserId != _friendsCategory._socialNetworkFunctionality.Id);

                public FriendCounters(FriendsCategory parent)
                {
                    _friendsCategory = parent;
                    _friends = _friendsCategory._socialNetwork.GetUserProfileRepository().GetAll()
                        .Where(u => u.Id == _friendsCategory._socialNetworkFunctionality.Id)
                        .Select(u => u.Friends)
                        .FirstOrDefault();
                }
            }
        }

    }
}
