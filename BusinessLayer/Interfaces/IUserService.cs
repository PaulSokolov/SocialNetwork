using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using BusinessLayer.Infrastructure;

namespace BusinessLayer.Interfaces
{
    public interface IUserService : IAdmin, IDisposable
    {
        Task<ClaimsIdentity> Authenticate(UserProfileDTO user);
        Task SetInitialData(UserProfileDTO admin, List<string> roles);
        Task<OperationDetails> ChangePassword(string userId, string oldPassword, string newPassword);
    }
}
