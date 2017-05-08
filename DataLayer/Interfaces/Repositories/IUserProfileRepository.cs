using DataLayer.Entities;
using System;
using System.Linq;

namespace DataLayer.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        IQueryable<UserProfile> GetAll();
        UserProfile Get(string id);
    }
}
