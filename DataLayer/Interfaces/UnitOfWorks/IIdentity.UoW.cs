using System;
using System.Threading.Tasks;
using DataLayer.Identity;

namespace DataLayer.Interfaces
{
    public interface IIdentityUoF : IDisposable
    {
        UserManager UserManager { get; }
        IClientManager ClientManager { get; }
        RoleManager RoleManager { get; }
        Task SaveAsync();
    }
}
