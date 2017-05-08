using DataLayer.Identity;

namespace BusinessLayer.Interfaces
{
    public interface IUserManagerCreator
    {
        UserManager CreateUserManager(string connection);
    }
}