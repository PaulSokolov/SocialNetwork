using System;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IClientManager : IDisposable
    {
        UserProfile Create(UserProfile item);
        UserProfile Delete(string id);

        Task<UserProfile> CreateAsync(UserProfile item);
        Task<UserProfile> DeleteAsync(string id);
    }
}

