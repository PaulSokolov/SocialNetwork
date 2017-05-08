namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.UserProfileLanguages", name: "UserProfile_Id", newName: "UserProfileId");
            RenameColumn(table: "dbo.UserProfileLanguages", name: "Language_Id", newName: "LanguageId");
            RenameIndex(table: "dbo.UserProfileLanguages", name: "IX_UserProfile_Id", newName: "IX_UserProfileId");
            RenameIndex(table: "dbo.UserProfileLanguages", name: "IX_Language_Id", newName: "IX_LanguageId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.UserProfileLanguages", name: "IX_LanguageId", newName: "IX_Language_Id");
            RenameIndex(table: "dbo.UserProfileLanguages", name: "IX_UserProfileId", newName: "IX_UserProfile_Id");
            RenameColumn(table: "dbo.UserProfileLanguages", name: "LanguageId", newName: "Language_Id");
            RenameColumn(table: "dbo.UserProfileLanguages", name: "UserProfileId", newName: "UserProfile_Id");
        }
    }
}
