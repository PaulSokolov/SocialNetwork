using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.BasicRepositories;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

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

        public UserProfile GetUserProfile(string id)
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

        public async Task<UserProfile> GetUserProfileAsync(string id)
        {
            try
            {
                return await Context.UserProfiles.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"GetUserProfileAsync() Failed {ex}");
            }
        }
    }
}
