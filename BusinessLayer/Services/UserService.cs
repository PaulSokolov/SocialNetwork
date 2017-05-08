using BusinessLayer.DTO;
using BusinessLayer.Infrastructure;
using BusinessLayer.Interfaces;
using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserService : IUserService
    {
        IIdentityUoF Database { get; set; }

        public UserService(IIdentityUoF uow)
        {
            Database = uow;
        }

        public async Task<OperationDetails> Create(UserProfileDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.Email };
                await Database.UserManager.CreateAsync(user, userDto.Password);
                //userDto.LastName = "asdsdfsdf";//await Database.SaveAsync();
                // добавляем роль
                try
                {
                    await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);
                }
                catch(Exception ex)
                {
                    string str = ex.Message;
                }
                // создаем профиль клиента
                AutoMapper.Mapper.Initialize(cfg => {
                    cfg.CreateMap<UserProfileDTO, UserProfile>();
                    cfg.CreateMap<CityDTO, City>();
                    cfg.CreateMap<CountryDTO, Country>();
                    cfg.CreateMap<FriendDTO, Friend>();
                    cfg.CreateMap<LanguageDTO, Language>();
                    cfg.CreateMap<SexDTO, Sex>();
                    cfg.CreateMap<UserMessageDTO, UserMessage>();
                });
                UserProfile clientProfile = AutoMapper.Mapper.Map<UserProfile>(userDto);//new UserProfile { Id = user.Id, Address = userDto.Address, Name = userDto.Name, LastName = userDto.Name, About=userDto.About, Activity=userDto.Activity, City=new City { Id = userDto.City.Id, Country = new Country { Id = userDto.City.Country.Id, Name = userDto.City.Country.Name } } , BirthDate = DateTime.Now  };
                clientProfile.Id = user.Id;
                Database.ClientManager.Create(clientProfile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Регистрация успешно пройдена", "");

            }
            else
            {
                return new OperationDetails(false, "Пользователь с таким логином уже существует", "Email");
            }
        }

        public async Task<OperationDetails> Delete(UserProfileDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user != null)
            {
                Database.ClientManager.Delete(user.Id);
                Database.UserManager.Delete(user);
                await Database.SaveAsync();
                return new OperationDetails(true, "User deleted successfully", "");

            }
            else
            {
                return new OperationDetails(false, "There is no such user", "");
            }
        }

        public async Task<OperationDetails> CreateRole(string role)
        {
            var applicationRole = await Database.RoleManager.FindByNameAsync(role);
            if (applicationRole == null)
            {
                await Database.RoleManager.CreateAsync(new ApplicationRole { Name = role });
                return new OperationDetails(true, "Role created succsessfully", "Role");
            }
            else
                return new OperationDetails(false, "Such role already exists", "Role");
        }

        public async Task<OperationDetails> DeleteRole(string role)
        {
                var applicationRole = await Database.RoleManager.FindByNameAsync(role);
                if (applicationRole != null)
                {
                    await Database.RoleManager.DeleteAsync(applicationRole);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Role deleted succsessfully", "Role");
                }
                else
                {
                    return new OperationDetails(false, "There is no such role", "Role");
                }
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

        public List<string> GetRoles()
        {
            var roles = Database.RoleManager.Roles.Select(r => r.Name);
            return roles.ToList();
        }
        public List<string> GetRoles(string id)
        {
            var roles = Database.UserManager.GetRoles(id);
            return roles.ToList();
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



        public void Dispose()
        {
            Database.Dispose();
        }

        public async Task<OperationDetails> AddToRoleAsync(string userId, string role)
        {
            if (await Database.RoleManager.RoleExistsAsync(role))
            {
                await Database.UserManager.AddToRoleAsync(userId, role);
                return new OperationDetails(true, "User added to role successfully", "Role");
            }
            else
                return new OperationDetails(false, "Role doesn't exist", "Role");
        }

        public async Task<OperationDetails> RemoveFromRoleAsync(string userId, string role)
        {
            if (await Database.RoleManager.RoleExistsAsync(role))
            {
                await Database.UserManager.RemoveFromRoleAsync(userId, role);
                return new OperationDetails(true, "User removed from role successfully", "Role");
            }
            else
                return new OperationDetails(false, "Role doesn't exist", "Role");
        }
    }
}
