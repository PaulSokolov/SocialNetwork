using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using System;
using System.Linq;

namespace DataLayer.Repository
{
    public class FriendRepository : UserInfoRepository<Friend>, IFriendRepository
    {
        public FriendRepository(UserProfileContext context) : base(context)
        {
        }
        public IQueryable<Friend> GetAll()
        {
            try
            {
                return Context.Friends;
            }
            catch (Exception ex)
            {

                throw new Exception($"GetAll() Failed {ex}");
            }
        }
        public IQueryable<Friend> GetAllByUserId(string id)
        {
            try
            {
                return Context.Friends.Where(f => f.UserId == id);
            }
            catch (Exception ex)
            {

                throw new Exception($"GetAllByUserId() Failed {ex}");
            }
        }

        public Friend GetFriend(string userId, string friendId)
        {
            try
            {
                Friend friend = Context.Friends.Where(f => f.UserId == userId && f.FriendId == friendId).FirstOrDefault();
                return friend;
            }
            catch (Exception ex)
            {

                throw new Exception($"GetFriend() Failed {ex}");
            }
        }
    }
}
