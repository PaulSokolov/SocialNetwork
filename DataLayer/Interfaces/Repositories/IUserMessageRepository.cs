using DataLayer.Entities;
using System;
using System.Linq;

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
