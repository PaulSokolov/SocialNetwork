using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using System;
using System.Linq;

namespace DataLayer.Repository
{
    public class UserProfileRepository : UserInfoRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(UserProfileContext context) : base(context)
        {
        }

        public IQueryable<UserProfile> GetAll()
        {
            try
            {
                return Context.UserProfiles;
            }
            catch (Exception ex)
            {
                throw new Exception($"GetMessageByMessageId() Failed {ex}");
            }
        }

        public UserProfile Get(string id)
        {
            try
            {
                return Context.UserProfiles.Find(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"GetMessageByMessageId() Failed {ex}");
            }
        }
    }
}
