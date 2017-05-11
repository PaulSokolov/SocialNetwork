using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IUserMessageRepository : IRepository<UserMessage>
    {
        IQueryable<UserMessage> GetAll();
        UserMessage Get(long messageId);
        IQueryable<UserMessage> GetAllByUserId(string userId);
        UserMessage Remove(long messageId);

        Task<IQueryable<UserMessage>> GetAllAsync();
        Task<UserMessage> GetAsync(long messageId);
        Task<IQueryable<UserMessage>> GetAllByUserAsync(string userId);
        Task<UserMessage> RemoveAsync(long messageId);
    }
}
