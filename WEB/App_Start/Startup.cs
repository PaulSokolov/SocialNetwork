using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
[assembly: OwinStartup(typeof(WEB.App_Start.Startup))]
namespace WEB.App_Start
{
    public class Startup
    {
        IServiceCreator serviceCreator = new ServiceCreator();

        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<IUserService>(CreateUserService);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
            app.MapSignalR();
        }

        private IUserService CreateUserService()
        {
            return serviceCreator.CreateUserService(@"data source=(LocalDb)\MSSQLLocalDB;initial catalog=SocialNetwork;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
        }
    }
}