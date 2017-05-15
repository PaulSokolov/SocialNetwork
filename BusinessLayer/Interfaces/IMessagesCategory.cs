using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTO;

namespace BusinessLayer.Interfaces
{
    public interface IMessagesCategory:IModerator
    {
        Task<int> GetUnreadAsync();
        Task<UserMessageDTO> GetAsync(long id);
        Task<List<UserMessageDTO>> GetAllMessagesByUserIdAsync(string id);
        Task<UserMessageDTO> SendAsync(long recipientId, string body);
        Task<List<UserMessageDTO>> GetDialogAsync(string friendId);
        Task<List<UserMessageDTO>> GetDialogAsync(long publicFriendId, int lastIndex);
        Task<List<UserMessageDTO>> GetLastMessagesAsync();
        UserMessageDTO Read(long id);
        Task<UserMessageDTO> ReadAsync(long id);
    }
}
