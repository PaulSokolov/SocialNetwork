using System.Data.Entity;
using DataLayer.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataLayer.EF
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(string conectionString) : base(conectionString) { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ApplicationUser>().HasKey<string>(l => l.Id);
        //    //modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
        //    modelBuilder.Entity<ApplicationRole>().HasKey(r => new { r.Id});
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
