using System;
namespace DataLayer.Interfaces
{
    public interface ISocialNetwork : ITransaction, IDisposable
    {
        IFriendRepository GetFriendRepository();
        IUserMessageRepository GetUserMessageRepository();
        IUserProfileRepository GetUserProfileRepository();

        bool LazyLoad { get; set; }
    }
}
