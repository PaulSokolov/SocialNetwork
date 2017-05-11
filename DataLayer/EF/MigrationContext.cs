using System.Data.Entity;
using DataLayer.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

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
        public MigrationContext() : base("name=SocialNetwork"/*@"data source=(LocalDb)\MSSQLLocalDB;initial catalog=SocialNetwork;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework"*/) { }
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
            
        }
    }
}
