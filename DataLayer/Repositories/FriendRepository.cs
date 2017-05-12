using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.Repository
{
    public class FriendRepository : UserInfoRepository<Friend, string>, IFriendRepository
    {
        public FriendRepository(UserProfileContext context) : base(context)
        {
        }

        public async Task<Friend> GetFriend(string userId, string friendId)
        {
            try
            {
                return await Context.Friends.FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
                
            }
            catch (Exception ex)
            {

                throw new Exception($"GetFriend() Failed {ex}");
            }
        }
    }
}
