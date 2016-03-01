namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedValueFromOpportunityCategory : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OpportunityCategories", "Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OpportunityCategories", "Value", c => c.Int(nullable: false));
        }
    }
}
