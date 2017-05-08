using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using System;
using System.Linq;

namespace DataLayer.Repository
{
    public class UserMessageRepository : UserInfoRepository<UserMessage>, IUserMessageRepository
    {
        public UserMessageRepository(UserProfileContext context) : base(context)
        {
        }

        public IQueryable<UserMessage> GetAll()
        {
            try
            {
                return Context.UserMessages;
            }
            catch (Exception ex)
            {
                throw new Exception($"GetAll() Failed {ex}");
            }
        }

        public IQueryable<UserMessage> GetAllByUserId(string userId)
        {
            try
            {
                return Context.UserMessages.Where(m=>m.FromUserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"GetAllByUserId() Failed {ex}");
            }
        }

        public UserMessage Get(long messageId)
        {
            try
            {
                return Context.UserMessages.Find(messageId);
            }
            catch (Exception ex)
            {
                throw new Exception($"GetMessageByMessageId() Failed {ex}");
            }
        }

        public UserMessage Remove(long messageId)
        {
            var item = Get(messageId);
            return Context.UserMessages.Remove(item);
        }
    }
}
