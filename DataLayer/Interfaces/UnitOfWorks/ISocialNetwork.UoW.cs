using System;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface ISocialNetwork : ITransaction, IDisposable
    {
        IFriendRepository GetFriendRepository();
        Task<IFriendRepository> GetFriendRepositoryAsync();
        IUserMessageRepository GetUserMessageRepository();
        Task<IUserMessageRepository> GetUserMessageRepositoryAsync();
        IUserProfileRepository GetUserProfileRepository();
        Task<IUserProfileRepository> GetUserProfileRepositoryAsync();
        bool LazyLoad { get; set; }

    }
}
