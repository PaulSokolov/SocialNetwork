using System;
using System.Threading.Tasks;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.Identity
{
    public class ClientManager : IClientManager
    {
        private IdentityContext _context;
        public ClientManager(IdentityContext db)
        {
            _context = db;
        }

        public UserProfile Create(UserProfile item)
        {
            if(item == null)
                throw  new ArgumentNullException(nameof(item));
            return _context.UserProfiles.Add(item);
        }

        public async Task<UserProfile> CreateAsync(UserProfile item)
        {
            var task = new Task<UserProfile>(()=>Create(item));
            task.Start();
            return await task;
        }

        public UserProfile Delete(string id)
        {
            var item = _context.UserProfiles.Find(id);
            return item != null ? _context.UserProfiles.Remove(item) : null;
        }

        public async Task<UserProfile> DeleteAsync(string id)
        {
           var task = new Task<UserProfile>(()=>Delete(id));
            task.Start();
            return await task;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        

       
    }
}
