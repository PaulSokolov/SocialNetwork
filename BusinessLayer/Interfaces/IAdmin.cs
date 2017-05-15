using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTO;
using BusinessLayer.Infrastructure;

namespace BusinessLayer.Interfaces
{
    public interface IAdmin
    {
        Task<OperationDetails> Create(UserProfileDTO user);
        Task<OperationDetails> Delete(UserProfileDTO user);
        Task<OperationDetails> CreateRole(string role);
        Task<OperationDetails> DeleteRole(string role);
        Task<OperationDetails> AddToRoleAsync(string id, string role);
        Task<OperationDetails> RemoveFromRoleAsync(string id, string role);
        Task<List<string>> GetRoles();
        Task<List<string>> GetRoles(string id);
    }
}
