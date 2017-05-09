using System.Linq;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        IQueryable<UserProfile> GetAll();
        UserProfile Get(string id);
    }
}
