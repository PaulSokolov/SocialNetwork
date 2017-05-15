using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.DTO;

namespace BusinessLayer.Interfaces
{
    public interface IModerator
    {
        Task<UserMessageDTO> ModerateAsync(long id, string body);
        Task<bool> DeleteAsync(long id);
        Task<List<IGrouping<string, UserMessageDTO>>> GetAllDialogsAsync();
    }
}
