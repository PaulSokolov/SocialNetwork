using BusinessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
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
        List<string> GetRoles();
        List<string> GetRoles(string id);
        Task SetInitialData(UserProfileDTO admin, List<string> roles);
    }
}
