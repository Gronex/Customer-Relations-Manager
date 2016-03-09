namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactAddedToOpportunity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Opportunities", "ContactId", c => c.Int());
            CreateIndex("dbo.Opportunities", "ContactId");
            AddForeignKey("dbo.Opportunities", "ContactId", "dbo.People", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Opportunities", "ContactId", "dbo.People");
            DropIndex("dbo.Opportunities", new[] { "ContactId" });
            DropColumn("dbo.Opportunities", "ContactId");
        }
    }
}
