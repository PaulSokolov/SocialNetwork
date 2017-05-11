using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        IQueryable<UserProfile> GetAll();
        UserProfile GetUserProfile(string id);
        Task<UserProfile> GetUserProfileAsync(string id);
    }
}
