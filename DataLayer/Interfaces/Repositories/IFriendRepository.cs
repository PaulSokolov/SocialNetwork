using DataLayer.Entities;
using System;
using System.Linq;

namespace DataLayer.Interfaces
{
    public interface IFriendRepository : IRepository<Friend>
    {
        IQueryable<Friend> GetAll();
        IQueryable<Friend> GetAllByUserId(string id);
        Friend GetFriend(string userId, string friendId);
    }
}
