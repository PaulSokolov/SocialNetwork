using DataLayer.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.BusinessModels
{

    public partial class SocialNetworkFunctionalityUser
    {
        public partial class FriendsCategory
        {
            public class FriendCounters
            {
                private FriendsCategory _friendsCategory;
                private ICollection<Friend> _friends;

                public long Friends
                {
                    get
                    {
                        return _friends.Where(f => f.Confirmed == true && f.Deleted == false).Count();
                    }
                }
                public long Followers
                {
                    get
                    {
                        return _friends.Where(f => (f.Confirmed == false || f.Deleted == true) && f.RequestUserId != _friendsCategory._socialNetworkFunctionality.Id).Count();
                    }
                }
                public long Followed
                {
                    get
                    {
                        return _friends.Where(f => (f.Confirmed == false || f.Deleted == true) && f.RequestUserId == _friendsCategory._socialNetworkFunctionality.Id).Count();
                    }
                }
                public long Requests
                {
                    get
                    {
                        return _friends.Where(f => f.Confirmed == false && f.Deleted == false && f.RequestUserId != _friendsCategory._socialNetworkFunctionality.Id).Count();
                    }
                }

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
