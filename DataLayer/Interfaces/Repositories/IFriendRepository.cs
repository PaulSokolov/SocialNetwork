using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IFriendRepository : IRepository<Friend, string>
    {
        Task<Friend> GetFriend(string userId, string friendId);
    }
}
