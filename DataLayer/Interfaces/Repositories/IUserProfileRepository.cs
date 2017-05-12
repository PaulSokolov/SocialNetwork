using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile,string>
    {
        Task<UserProfile> GetAsync(long publicId);
    }
}
