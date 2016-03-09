namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueueCategoryName : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ActivityCategories", "Name", unique: true);
            CreateIndex("dbo.OpportunityCategories", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.OpportunityCategories", new[] { "Name" });
            DropIndex("dbo.ActivityCategories", new[] { "Name" });
        }
    }
}
