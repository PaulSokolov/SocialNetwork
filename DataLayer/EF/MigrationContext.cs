using DataLayer.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.EF
{
    public class MigrationContext: IdentityDbContext<ApplicationUser>
    {
        //public DbSet<Entity> Entity { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public MigrationContext(string conectionString) : base(conectionString) { }
        public MigrationContext() : base(@"data source=(LocalDb)\MSSQLLocalDB;initial catalog=SocialNetwork;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework") { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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
            .WillCascadeOnDelete(false);

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

            //modelBuilder.Entity<Friend>()
            //            .HasRequired(m => m.Friended)
            //            .WithMany()
            //            .HasForeignKey(m => m.FriendId)
            //            .WillCascadeOnDelete(false);

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
           

            //modelBuilder.Entity<Friend>()
            //            .HasRequired(afc => afc.FriendToAdd)
            //            .WithMany()
            //            .HasForeignKey(afc => afc.FriendId)
            //            .WillCascadeOnDelete(false);
            //modelBuilder.Entity<Friend>()
            //            .HasRequired(afc => afc.RequestUser)
            //            .WithMany()
            //            .HasForeignKey(afc => afc.RequestUserId)
            //            .WillCascadeOnDelete(false);
        }
    }
}
