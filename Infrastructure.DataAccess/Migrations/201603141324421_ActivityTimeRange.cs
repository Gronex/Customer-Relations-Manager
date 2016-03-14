namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivityTimeRange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activities", "DueTimeStart", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Activities", "DueTimeEnd", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.Activities", "DueTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activities", "DueTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.Activities", "DueTimeEnd");
            DropColumn("dbo.Activities", "DueTimeStart");
        }
    }
}
