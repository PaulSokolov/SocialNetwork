using System;
using System.Threading.Tasks;
using DataLayer.EF;
using DataLayer.Entities;
using DataLayer.Identity;
using DataLayer.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataLayer.UnitOfWorks
{
    public class IdentityUnitOfWork : IIdentityUoF
    {
        private readonly IdentityContext _context;
        private bool _disposed;
        public IdentityUnitOfWork(string connectionString)

        {
            _context = new IdentityContext(connectionString);
            UserManager = new UserManager(new UserStore<ApplicationUser>(_context));
            RoleManager = new RoleManager(new RoleStore<ApplicationRole>(_context));
            ClientManager = new ClientManager(_context);
        }

        public UserManager UserManager { get; }

        public IClientManager ClientManager { get; }

        public RoleManager RoleManager { get; }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                UserManager.Dispose();
                RoleManager.Dispose();
                ClientManager.Dispose();
            }
            _disposed = true;
        }
    }
}
