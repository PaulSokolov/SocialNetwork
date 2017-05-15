using System;
using DataLayer.EF;
using DataLayer.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace BusinessLayer.Services
{
    //public class UserManager:IDisposable
    //{
    //    public static DataLayer.Identity.UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
    //    {
    //        var manager = new DataLayer.Identity.UserManager(new UserStore<ApplicationUser>(context.Get<IdentityContext>()));

    //        manager.UserValidator = new UserValidator<ApplicationUser>(manager)
    //        {
    //            AllowOnlyAlphanumericUserNames = false,
    //            RequireUniqueEmail = true
    //        };

    //        manager.PasswordValidator = new PasswordValidator
    //        {
    //            RequiredLength = 4,
    //            //RequireNonLetterOrDigit = true,
    //            RequireDigit = false,
    //            RequireLowercase = true,
    //            RequireUppercase = false
    //        };


    //        manager.UserLockoutEnabledByDefault = true;
    //        manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //        manager.MaxFailedAccessAttemptsBeforeLockout = 5;

    //        return manager;
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
