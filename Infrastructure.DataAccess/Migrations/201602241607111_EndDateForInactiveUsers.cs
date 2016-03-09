namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EndDateForInactiveUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "EndDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "EndDate");
        }
    }
}
