using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLayer.DTO;
using BusinessLayer.Infrastructure;

namespace BusinessLayer.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task<OperationDetails> Create(UserProfileDTO user);
        Task<OperationDetails> Delete(UserProfileDTO user);
        Task<OperationDetails> CreateRole(string role);
        Task<OperationDetails> DeleteRole(string role);
        Task<OperationDetails> AddToRoleAsync(string id, string role);
        Task<OperationDetails> RemoveFromRoleAsync(string id, string role);
        Task<ClaimsIdentity> Authenticate(UserProfileDTO user);
        Task<List<string>> GetRoles();
        Task<List<string>> GetRoles(string id);
        Task SetInitialData(UserProfileDTO admin, List<string> roles);
    }
}
