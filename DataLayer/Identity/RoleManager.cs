using DataLayer.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Identity
{
    public class RoleManager : RoleManager<ApplicationRole>
    {
        public RoleManager(RoleStore<ApplicationRole> store)
                    : base(store)
        { }
    }
}
