using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        IQueryable<UserProfile> GetAll();
        UserProfile Get(string id);

        Task<IQueryable<UserProfile>> GetAllAsync();
        Task<UserProfile> GetAsync(string id);
    }
}
