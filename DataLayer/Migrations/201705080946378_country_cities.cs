namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class country_cities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Friends", "FriendId", "dbo.UserProfiles");
            DropIndex("dbo.Friends", new[] { "FriendId" });
            DropColumn("dbo.Friends", "UserId");
            RenameColumn(table: "dbo.Friends", name: "FriendId", newName: "UserId");
            CreateIndex("dbo.Friends", "FriendId");
            AddForeignKey("dbo.Friends", "UserId", "dbo.UserProfiles", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Friends", "UserId", "dbo.UserProfiles");
            DropIndex("dbo.Friends", new[] { "FriendId" });
            RenameColumn(table: "dbo.Friends", name: "UserId", newName: "FriendId");
            AddColumn("dbo.Friends", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Friends", "FriendId");
            AddForeignKey("dbo.Friends", "FriendId", "dbo.UserProfiles", "Id");
        }
    }
}
