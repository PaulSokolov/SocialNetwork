using DataLayer.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataLayer.Identity
{
    public class RoleManager : RoleManager<ApplicationRole>
    {
        public RoleManager(RoleStore<ApplicationRole> store)
                    : base(store)
        { }
    }
}
