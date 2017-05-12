using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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
                private ICollection<Friend> _friends;

                public long Friends => _friends.Count(f => f.Confirmed && f.Deleted == false);

                public long Followers => _friends.Count(f => (f.Confirmed == false || f.Deleted) && f.RequestUserId != _friendsCategory._socialNetworkFunctionality.Id);

                public long Followed => _friends.Count(f => (f.Confirmed == false || f.Deleted) && f.RequestUserId == _friendsCategory._socialNetworkFunctionality.Id);

                public long Requests => _friends.Count(f => f.Confirmed == false && f.Deleted == false && f.RequestUserId != _friendsCategory._socialNetworkFunctionality.Id);

                public FriendCounters(FriendsCategory parent)
                {
                    _friendsCategory = parent;
                }

                public async Task FriendsCounters()
                {
                    _friends = await _friendsCategory._socialNetwork.UserProfiles.GetAll()
                        .Where(u => u.Id == _friendsCategory._socialNetworkFunctionality.Id)
                        .Select(u => u.Friends)
                        .FirstOrDefaultAsync();
                }
                public async Task<long> CountFriendsAync()
                {
                    return await Task.Run(() => this.Friends);
                }

                public async Task<long> CountFollowersAync()
                {
                    return await Task.Run(() => this.Followers);
                }

                public async Task<long> CountFollowedAync()
                {
                    return await Task.Run(() => this.Followed);
                }

                public async Task<long> CountRequestsAync()
                {
                    return await Task.Run(() => this.Requests);
                }

            }
        }

    }
}
