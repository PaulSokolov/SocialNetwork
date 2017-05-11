using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IUserMessageRepository : IRepository<UserMessage>
    {
        IQueryable<UserMessage> GetAll();
        UserMessage GetMessage(long messageId);
        Task<UserMessage> GetMessageAsync(long messageId);
        IQueryable<UserMessage> GetAllByUserId(string userId);
        UserMessage Remove(long messageId);
        Task<UserMessage> RemoveAsync(long messageId);
    }
}
