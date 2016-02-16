namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueYearMonthUser : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductionGoals", new[] { "UserId" });
            CreateIndex("dbo.ProductionGoals", new[] { "Year", "Month", "UserId" }, unique: true, name: "YearMonthUser");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductionGoals", "YearMonthUser");
            CreateIndex("dbo.ProductionGoals", "UserId");
        }
    }
}
