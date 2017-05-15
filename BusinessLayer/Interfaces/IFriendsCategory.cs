using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTO;

namespace BusinessLayer.Interfaces
{
    public interface IFriendsCategory
    {
        IFriendCounter Counters { get;  }
        Task<FriendDTO> AddAsync(string userToAddId);
        Task<FriendDTO> AddAsync(long userToAddPublicId);
        Task<FriendDTO> ConfirmAsync(long userToAddPublicId);
        Task<FriendDTO> DeleteAsync(long userToDeletePublicId);
        Task UnsubscribeAsync(long unsubscribeId);
        Task<ICollection<UserProfileDTO>> GetFriendsAsync();
        Task<ICollection<UserProfileDTO>> GetFriendsAsync(int count);
        Task<ICollection<UserProfileDTO>> GetFollowedAsync();
        Task<ICollection<UserProfileDTO>> GetFollowersAsync();
    }
}
