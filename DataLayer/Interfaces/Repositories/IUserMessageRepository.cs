using System.Linq;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IUserMessageRepository : IRepository<UserMessage>
    {
        IQueryable<UserMessage> GetAll();
        UserMessage Get(long messageId);

        IQueryable<UserMessage> GetAllByUserId(string userId);

        UserMessage Remove(long messageId);
    }
}
