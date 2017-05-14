using System;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface ISocialNetwork : ITransaction, IDisposable
    {
        IFriendRepository Friends { get; }
        IUserMessageRepository Messages { get; }
        IUserProfileRepository UserProfiles { get; }
        ILanguageRepository Languages { get; }
        bool LazyLoad { get; set; }

    }
}
