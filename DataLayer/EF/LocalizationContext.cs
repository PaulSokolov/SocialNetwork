using System.Data.Entity;
using DataLayer.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataLayer.EF
{
    public class LocalizationContext:DbContext
    {
        public LocalizationContext(string conectionString) : base(conectionString) { }

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Language> Languages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserLogin>().HasKey(l => l.UserId);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

            modelBuilder.Entity<UserMessage>()
                .HasRequired(m => m.FromUser)
                .WithMany(t => t.SentMessages)
                .HasForeignKey(m => m.FromUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserMessage>()
                .HasRequired(m => m.ToUser)
                .WithMany(t => t.ReceivedMessages)
                .HasForeignKey(m => m.ToUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Friend>()
                .HasRequired(afc => afc.User)
                .WithMany()
                .HasForeignKey(afc => afc.UserId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Friend>()
                .HasRequired(m => m.RequestUser)
                .WithMany(t => t.RequestedFriends)
                .HasForeignKey(m => m.RequestUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Friend>()
                .HasRequired(m => m.Friended)
                .WithMany(t => t.Friends)
                .HasForeignKey(m => m.FriendId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserProfile>()
                .HasMany<Language>(s => s.Languages)
                .WithMany(c => c.Users)
                .Map(cs =>
                {
                    cs.MapLeftKey("UserProfileId");
                    cs.MapRightKey("LanguageId");
                    cs.ToTable("UserProfileLanguages");
                });

            base.OnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
