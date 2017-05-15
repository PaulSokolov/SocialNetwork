using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using WEB.App_Start;

[assembly: OwinStartup(typeof(Startup))]
namespace WEB.App_Start
{
    public class Startup
    {
        private readonly IServiceCreator _serviceCreator = new ServiceCreator();

        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(CreateUserService);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            app.MapSignalR();
        }

        private IUserService CreateUserService()
        {
            return _serviceCreator.CreateUserService();
        }
    }
}