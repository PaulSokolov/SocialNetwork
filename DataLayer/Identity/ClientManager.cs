using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;

namespace DataLayer.Identity
{
    public class ClientManager : IClientManager
    {
        public IdentityContext Database { get; set; }
        public ClientManager(IdentityContext db)
        {
            Database = db;
        }

        public UserProfile Create(UserProfile item)
        {
            return Database.UserProfiles.Add(item);
        }
        public UserProfile Delete(string id)
        {
            var item = Database.UserProfiles.Find(id);
            return item != null ? Database.UserProfiles.Remove(item) : null;
        }
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
