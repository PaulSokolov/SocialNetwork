using System;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IClientManager : IDisposable
    {
        UserProfile Create(UserProfile item);
        Task<UserProfile> CreateAsync(UserProfile item);
        UserProfile Delete(string id);
        Task<UserProfile> DeleteAsync(string id);

    }
}

