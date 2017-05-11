using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IFriendRepository : IRepository<Friend>
    {
        IQueryable<Friend> GetAll();
        IQueryable<Friend> GetAllByUserId(string id);
        Friend GetFriend(string userId, string friendId);
        Task<IQueryable<Friend>> GetAllAsync();
        Task<IQueryable<Friend>> GetAllByUserIdAsync(string id);
    }
}
