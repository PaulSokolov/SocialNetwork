using BusinessLayer.Interfaces;
using DataLayer.UnitOfWorks;

namespace BusinessLayer.Services
{
    public class ServiceCreator : IServiceCreator
    {
        public const string Connection = "name=SocialNetwork";
        public IUserService CreateUserService()
        {
            return new UserService(new IdentityUnitOfWork(Connection));
        }
        
    }
}
