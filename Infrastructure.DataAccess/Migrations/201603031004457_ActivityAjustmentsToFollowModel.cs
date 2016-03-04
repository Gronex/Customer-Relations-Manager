namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivityAjustmentsToFollowModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activities", "PersonId", "dbo.People");
            DropIndex("dbo.Activities", new[] { "PersonId" });
            CreateTable(
                "dbo.PersonActivities",
                c => new
                    {
                        Person_Id = c.Int(nullable: false),
                        Activity_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Activity_Id })
                .ForeignKey("dbo.People", t => t.Person_Id, cascadeDelete: true)
                .ForeignKey("dbo.Activities", t => t.Activity_Id, cascadeDelete: true)
                .Index(t => t.Person_Id)
                .Index(t => t.Activity_Id);
            
            AddColumn("dbo.Activities", "DueTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.Activities", "Time");
            DropColumn("dbo.Activities", "PersonId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activities", "PersonId", c => c.Int());
            AddColumn("dbo.Activities", "Time", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropForeignKey("dbo.PersonActivities", "Activity_Id", "dbo.Activities");
            DropForeignKey("dbo.PersonActivities", "Person_Id", "dbo.People");
            DropIndex("dbo.PersonActivities", new[] { "Activity_Id" });
            DropIndex("dbo.PersonActivities", new[] { "Person_Id" });
            DropColumn("dbo.Activities", "DueTime");
            DropTable("dbo.PersonActivities");
            CreateIndex("dbo.Activities", "PersonId");
            AddForeignKey("dbo.Activities", "PersonId", "dbo.People", "Id");
        }
    }
}
