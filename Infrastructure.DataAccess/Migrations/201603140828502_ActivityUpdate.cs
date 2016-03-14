namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivityUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PersonActivities", "Person_Id", "dbo.People");
            DropForeignKey("dbo.PersonActivities", "Activity_Id", "dbo.Activities");
            DropIndex("dbo.PersonActivities", new[] { "Person_Id" });
            DropIndex("dbo.PersonActivities", new[] { "Activity_Id" });
            RenameColumn(table: "dbo.Activities", name: "ResponsibleId", newName: "PrimaryResponsibleId");
            RenameIndex(table: "dbo.Activities", name: "IX_ResponsibleId", newName: "IX_PrimaryResponsibleId");
            CreateTable(
                "dbo.ActivityPersons",
                c => new
                    {
                        Activity_Id = c.Int(nullable: false),
                        Person_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Activity_Id, t.Person_Id })
                .ForeignKey("dbo.Activities", t => t.Activity_Id)
                .ForeignKey("dbo.People", t => t.Person_Id)
                .Index(t => t.Activity_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "dbo.ActivityUsers",
                c => new
                    {
                        Activity_Id = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.Activity_Id, t.User_Id })
                .ForeignKey("dbo.Activities", t => t.Activity_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Activity_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.Activities", "PrimaryContactId", c => c.Int());
            CreateIndex("dbo.Activities", "PrimaryContactId");
            AddForeignKey("dbo.Activities", "PrimaryContactId", "dbo.People", "Id");
            DropTable("dbo.PersonActivities");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PersonActivities",
                c => new
                    {
                        Person_Id = c.Int(nullable: false),
                        Activity_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Activity_Id });
            
            DropForeignKey("dbo.ActivityUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ActivityUsers", "Activity_Id", "dbo.Activities");
            DropForeignKey("dbo.ActivityPersons", "Person_Id", "dbo.People");
            DropForeignKey("dbo.ActivityPersons", "Activity_Id", "dbo.Activities");
            DropForeignKey("dbo.Activities", "PrimaryContactId", "dbo.People");
            DropIndex("dbo.ActivityUsers", new[] { "User_Id" });
            DropIndex("dbo.ActivityUsers", new[] { "Activity_Id" });
            DropIndex("dbo.ActivityPersons", new[] { "Person_Id" });
            DropIndex("dbo.ActivityPersons", new[] { "Activity_Id" });
            DropIndex("dbo.Activities", new[] { "PrimaryContactId" });
            DropColumn("dbo.Activities", "PrimaryContactId");
            DropTable("dbo.ActivityUsers");
            DropTable("dbo.ActivityPersons");
            RenameIndex(table: "dbo.Activities", name: "IX_PrimaryResponsibleId", newName: "IX_ResponsibleId");
            RenameColumn(table: "dbo.Activities", name: "PrimaryResponsibleId", newName: "ResponsibleId");
            CreateIndex("dbo.PersonActivities", "Activity_Id");
            CreateIndex("dbo.PersonActivities", "Person_Id");
            AddForeignKey("dbo.PersonActivities", "Activity_Id", "dbo.Activities", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonActivities", "Person_Id", "dbo.People", "Id", cascadeDelete: true);
        }
    }
}
