namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OnlyOneCompanyActivePerPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "StartDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.People", "CompanyId", c => c.Int());
            AlterColumn("dbo.Contracts", "EndDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            CreateIndex("dbo.People", "CompanyId");
            AddForeignKey("dbo.People", "CompanyId", "dbo.Companies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.People", "CompanyId", "dbo.Companies");
            DropIndex("dbo.People", new[] { "CompanyId" });
            AlterColumn("dbo.Contracts", "EndDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.People", "CompanyId");
            DropColumn("dbo.People", "StartDate");
        }
    }
}
