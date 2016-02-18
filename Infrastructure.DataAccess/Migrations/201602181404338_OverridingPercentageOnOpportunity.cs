namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverridingPercentageOnOpportunity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Opportunities", "Percentage", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Opportunities", "Percentage");
        }
    }
}
