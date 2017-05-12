using System;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface ISocialNetwork : ITransaction, IDisposable
    {
        IFriendRepository Friends { get; }
        IUserMessageRepository Messages { get; }
        IUserProfileRepository UserProfiles { get; }
        bool LazyLoad { get; set; }

    }
}
