namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class publicId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "PublicId", c => c.Long(nullable: false, identity: true));
            CreateIndex("dbo.UserProfiles", "PublicId", unique: true);
            DropColumn("dbo.AspNetUsers", "Avatar");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Avatar", c => c.String());
            DropIndex("dbo.UserProfiles", new[] { "PublicId" });
            DropColumn("dbo.UserProfiles", "PublicId");
        }
    }
}
