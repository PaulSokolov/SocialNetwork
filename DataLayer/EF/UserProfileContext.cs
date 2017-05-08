using DataLayer.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DataLayer.EF
{
    public class UserProfileContext : DbContext
    {
        public UserProfileContext(string conectionString) : base(conectionString) { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
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
            .WithMany(t => t.Friends)
            .HasForeignKey(afc => afc.UserId)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<Friend>()
                    .HasRequired(m => m.RequestUser)
                    .WithMany(t => t.RequestedFriends)
                    .HasForeignKey(m => m.RequestUserId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Friend>()
                        .HasRequired(m => m.Friended)
                        .WithMany()
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
        }
    }
}
