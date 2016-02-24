namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductionGoalHaveRealDates : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductionGoals", "YearMonthUser");
            AddColumn("dbo.ProductionGoals", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            CreateIndex("dbo.ProductionGoals", new[] { "StartDate", "UserId" }, unique: true, name: "DateUser");
            DropColumn("dbo.ProductionGoals", "Year");
            DropColumn("dbo.ProductionGoals", "Month");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductionGoals", "Month", c => c.Int(nullable: false));
            AddColumn("dbo.ProductionGoals", "Year", c => c.Int(nullable: false));
            DropIndex("dbo.ProductionGoals", "DateUser");
            DropColumn("dbo.ProductionGoals", "StartDate");
            CreateIndex("dbo.ProductionGoals", new[] { "Year", "Month", "UserId" }, unique: true, name: "YearMonthUser");
        }
    }
}
