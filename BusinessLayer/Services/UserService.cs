using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.DTO;
using BusinessLayer.Infrastructure;
using BusinessLayer.Interfaces;
using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.AspNet.Identity;

namespace BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private IIdentityUoF Database { get; set; }
        private SemaphoreSlim _semaphore;
        public UserService(IIdentityUoF uow)
        {
            Database = uow;
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task<OperationDetails> Create(UserProfileDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user != null)
                return new OperationDetails(false, "Пользователь с таким логином уже существует", "Email");
            user = new ApplicationUser { Email = userDto.Email, UserName = userDto.Email };
            await Database.UserManager.CreateAsync(user, userDto.Password);

            await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<UserProfileDTO, UserProfile>();
                cfg.CreateMap<CityDTO, City>();
                cfg.CreateMap<CountryDTO, Country>();
                cfg.CreateMap<FriendDTO, Friend>();
                cfg.CreateMap<LanguageDTO, Language>();
                cfg.CreateMap<SexDTO, Sex>();
                cfg.CreateMap<UserMessageDTO, UserMessage>();
            });
            var clientProfile = Mapper.Map<UserProfile>(userDto);//new UserProfile { Id = user.Id, Address = userDto.Address, Name = userDto.Name, LastName = userDto.Name, About=userDto.About, Activity=userDto.Activity, City=new City { Id = userDto.City.Id, Country = new Country { Id = userDto.City.Country.Id, Name = userDto.City.Country.Name } } , BirthDate = DateTime.Now  };
            clientProfile.Id = user.Id;
            Database.ClientManager.Create(clientProfile);
            await Database.SaveAsync();
            return new OperationDetails(true, "Регистрация успешно пройдена", "");
        }

        public async Task<OperationDetails> Delete(UserProfileDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null) return new OperationDetails(false, "There is no such user", "");

            Database.ClientManager.Delete(user.Id);
            Database.UserManager.Delete(user);
            await Database.SaveAsync();
            return new OperationDetails(true, "User deleted successfully", "");
        }

        public async Task<OperationDetails> CreateRole(string role)
        {
            var applicationRole = await Database.RoleManager.FindByNameAsync(role);
            if (applicationRole != null) return new OperationDetails(false, "Such role already exists", "Role");

            await Database.RoleManager.CreateAsync(new ApplicationRole { Name = role });
            return new OperationDetails(true, "Role created succsessfully", "Role");
        }

        public async Task<OperationDetails> DeleteRole(string role)
        {
            var applicationRole = await Database.RoleManager.FindByNameAsync(role);
            if (applicationRole == null) return new OperationDetails(false, "There is no such role", "Role");

            await Database.RoleManager.DeleteAsync(applicationRole);
            await Database.SaveAsync();
            return new OperationDetails(true, "Role deleted succsessfully", "Role");
        }
        public async Task<ClaimsIdentity> Authenticate(UserProfileDTO userDto)
        {
            ClaimsIdentity claim = null;

            ApplicationUser user = await Database.UserManager.FindAsync(userDto.Email, userDto.Password);

            if (user != null)
                claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);

            return claim;
        }

        public async Task<List<string>> GetRoles()
        {
            try
            {
                await _semaphore.WaitAsync();
                return await Database.RoleManager.Roles.Select(r => r.Name).ToListAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<List<string>> GetRoles(string id)
        {
            try
            {
                await _semaphore.WaitAsync();
                return (await Database.UserManager.GetRolesAsync(id)).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task SetInitialData(UserProfileDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await Database.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }
            }

            await Create(adminDto);
        }

        public async Task<OperationDetails> AddToRoleAsync(string userId, string role)
        {
            if (!await Database.RoleManager.RoleExistsAsync(role))
                return new OperationDetails(false, "Role doesn't exist", "Role");

            await Database.UserManager.AddToRoleAsync(userId, role);

            return new OperationDetails(true, "User added to role successfully", "Role");
        }

        public async Task<OperationDetails> RemoveFromRoleAsync(string userId, string role)
        {
            if (!await Database.RoleManager.RoleExistsAsync(role))
                return new OperationDetails(false, "Role doesn't exist", "Role");

            await Database.UserManager.RemoveFromRoleAsync(userId, role);

            return new OperationDetails(true, "User removed from role successfully", "Role");
        }

        public void Dispose()
        {
            Database.Dispose();
        }

       
    }
}
