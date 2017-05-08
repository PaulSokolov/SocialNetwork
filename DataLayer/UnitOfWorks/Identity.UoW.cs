using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Interfaces;
using DataLayer.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Threading.Tasks;

namespace DataLayer.UnitOfWorks
{
    public class IdentityUnitOfWork : IIdentityUoF
    {
        private IdentityContext db;

        private UserManager userManager;
        private RoleManager roleManager;
        private IClientManager clientManager;

        public IdentityUnitOfWork(string connectionString)

        {
            db = new IdentityContext(connectionString);
            userManager = new UserManager(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager(new RoleStore<ApplicationRole>(db));
            clientManager = new ClientManager(db);
        }

        public UserManager UserManager
        {
            get { return userManager; }
        }

        public IClientManager ClientManager
        {
            get { return clientManager; }
        }

        public RoleManager RoleManager
        {
            get { return roleManager; }
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    userManager.Dispose();
                    roleManager.Dispose();
                    clientManager.Dispose();
                }
                this.disposed = true;
            }
        }
    }
}
