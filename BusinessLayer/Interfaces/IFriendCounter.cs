using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IFriendCounter
    {
        long Friends { get; }
        long Followers { get; }
        long Followed { get; }
        long Requests { get; }
        Task FriendsCounters();
        Task<long> CountFriendsAync();
        Task<long> CountFollowersAync();
        Task<long> CountFollowedAync();
        Task<long> CountRequestsAync();
    }
}
