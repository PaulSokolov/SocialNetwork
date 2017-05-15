using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace BusinessLayer.BusinessModels
{

    public partial class SocialNetworkManager
    {
        public partial class FriendsCategory
        {
            public class FriendCounters
            {
                private SemaphoreSlim Semaphore => _friendsCategory.Semaphore;
                private readonly FriendsCategory _friendsCategory;
                private ICollection<Friend> _friends;
                private string CurrentUserId => _friendsCategory.CurrentUserId;

                public long Friends => _friends.Count(f => f.Confirmed && f.Deleted == false);

                public long Followers => _friends.Count(f => (f.Confirmed == false || f.Deleted) && f.RequestUserId != CurrentUserId);

                public long Followed => _friends.Count(f => (f.Confirmed == false || f.Deleted) && f.RequestUserId == CurrentUserId);

                public long Requests => _friends.Count(f => f.Confirmed == false && f.Deleted == false && f.RequestUserId != CurrentUserId);

                public FriendCounters(FriendsCategory parent)
                {
                    _friendsCategory = parent;
                }

                public async Task FriendsCounters()
                {
                    await Semaphore.WaitAsync();
                    _friends = await _friendsCategory.SocialNetwork.UserProfiles.GetAll()
                        .Where(u => u.Id == CurrentUserId)
                        .Select(u => u.Friends)
                        .FirstOrDefaultAsync();
                    Semaphore.Release();
                }
                public async Task<long> CountFriendsAync()
                {
                    return await Task.Run(() => Friends);
                }

                public async Task<long> CountFollowersAync()
                {
                    return await Task.Run(() => Followers);
                }

                public async Task<long> CountFollowedAync()
                {
                    return await Task.Run(() => Followed);
                }

                public async Task<long> CountRequestsAync()
                {
                    return await Task.Run(() => Requests);
                }

            }
        }

    }
}
