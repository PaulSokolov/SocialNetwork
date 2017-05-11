using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

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

        public UserMessage GetMessage(long messageId)
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

        public async Task<UserMessage> GetMessageAsync(long messageId)
        {
            try
            {
                return await Context.UserMessages.FindAsync(messageId);
            }
            catch (Exception ex)
            {
                throw new Exception($"GetMessageByMessageIdAsync() Failed {ex}");
            }
        }

        public UserMessage Remove(long messageId)
        {
            var item = GetMessage(messageId);
            return Context.UserMessages.Remove(item);
        }

        public async Task<UserMessage> RemoveAsync(long messageId)
        {
            var item = await GetMessageAsync(messageId);
            var task = new Task<UserMessage>(()=> Context.UserMessages.Remove(item));
            task.Start();
            return await task;
        }
    }
}
