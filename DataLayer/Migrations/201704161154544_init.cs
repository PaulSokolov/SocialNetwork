namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CountryId = c.Long(nullable: false),
                        AddedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        AddedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        Name = c.String(),
                        AddedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Address = c.String(),
                        ActivatedDate = c.DateTime(),
                        LastVisitDateTime = c.DateTime(),
                        Avatar = c.String(),
                        BirthDate = c.DateTime(),
                        BirthDateIsHidden = c.Boolean(nullable: false),
                        About = c.String(),
                        AboutIsHidden = c.Boolean(nullable: false),
                        Activity = c.String(),
                        ActivityIsHidden = c.Boolean(nullable: false),
                        CityId = c.Long(),
                        Email = c.String(),
                        EmailIsHidden = c.Boolean(nullable: false),
                        Sex = c.Int(),
                        AddedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .Index(t => t.Id)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Friends",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FriendId = c.String(nullable: false, maxLength: 128),
                        RequestUserId = c.String(nullable: false, maxLength: 128),
                        Confirmed = c.Boolean(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        ConfirmDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                        AddedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.UserId, t.FriendId })
                .ForeignKey("dbo.UserProfiles", t => t.FriendId)
                .ForeignKey("dbo.UserProfiles", t => t.RequestUserId)
                .ForeignKey("dbo.UserProfiles", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.FriendId)
                .Index(t => t.RequestUserId);
            
            CreateTable(
                "dbo.UserMessages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FromUserId = c.String(nullable: false, maxLength: 128),
                        ToUserId = c.String(nullable: false, maxLength: 128),
                        PostedDate = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        Body = c.String(),
                        AddedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.FromUserId)
                .ForeignKey("dbo.UserProfiles", t => t.ToUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.LanguageCountries",
                c => new
                    {
                        Language_Id = c.Long(nullable: false),
                        Country_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Language_Id, t.Country_Id })
                .ForeignKey("dbo.Languages", t => t.Language_Id, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.Country_Id, cascadeDelete: true)
                .Index(t => t.Language_Id)
                .Index(t => t.Country_Id);
            
            CreateTable(
                "dbo.UserProfileLanguages",
                c => new
                    {
                        UserProfile_Id = c.String(nullable: false, maxLength: 128),
                        Language_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserProfile_Id, t.Language_Id })
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id, cascadeDelete: true)
                .ForeignKey("dbo.Languages", t => t.Language_Id, cascadeDelete: true)
                .Index(t => t.UserProfile_Id)
                .Index(t => t.Language_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Cities", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.UserMessages", "ToUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserMessages", "FromUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfileLanguages", "Language_Id", "dbo.Languages");
            DropForeignKey("dbo.UserProfileLanguages", "UserProfile_Id", "dbo.UserProfiles");
            DropForeignKey("dbo.Friends", "UserId", "dbo.UserProfiles");
            DropForeignKey("dbo.Friends", "RequestUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.Friends", "FriendId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfiles", "CityId", "dbo.Cities");
            DropForeignKey("dbo.UserProfiles", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.LanguageCountries", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.LanguageCountries", "Language_Id", "dbo.Languages");
            DropIndex("dbo.UserProfileLanguages", new[] { "Language_Id" });
            DropIndex("dbo.UserProfileLanguages", new[] { "UserProfile_Id" });
            DropIndex("dbo.LanguageCountries", new[] { "Country_Id" });
            DropIndex("dbo.LanguageCountries", new[] { "Language_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.UserMessages", new[] { "ToUserId" });
            DropIndex("dbo.UserMessages", new[] { "FromUserId" });
            DropIndex("dbo.Friends", new[] { "RequestUserId" });
            DropIndex("dbo.Friends", new[] { "FriendId" });
            DropIndex("dbo.Friends", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.UserProfiles", new[] { "CityId" });
            DropIndex("dbo.UserProfiles", new[] { "Id" });
            DropIndex("dbo.Cities", new[] { "CountryId" });
            DropTable("dbo.UserProfileLanguages");
            DropTable("dbo.LanguageCountries");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.UserMessages");
            DropTable("dbo.Friends");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.Languages");
            DropTable("dbo.Countries");
            DropTable("dbo.Cities");
        }
    }
}
