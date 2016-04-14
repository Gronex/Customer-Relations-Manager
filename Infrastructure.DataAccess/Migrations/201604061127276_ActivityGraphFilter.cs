namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivityGraphFilter : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ProductionViewSettingsOpportunityCategories", newName: "OpportunityCategoryProductionViewSettings");
            RenameTable(name: "dbo.UserGroupProductionViewSettings", newName: "ProductionViewSettingsUserGroups");
            DropIndex("dbo.ProductionViewSettings", new[] { "Name" });
            DropPrimaryKey("dbo.OpportunityCategoryProductionViewSettings");
            DropPrimaryKey("dbo.ProductionViewSettingsUserGroups");
            CreateTable(
                "dbo.ActivityViewSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Private = c.Boolean(nullable: false),
                        Name = c.String(maxLength: 100),
                        OwnerId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.UserGroupActivityViewSettings",
                c => new
                    {
                        UserGroup_Id = c.Int(nullable: false),
                        ActivityViewSettings_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserGroup_Id, t.ActivityViewSettings_Id })
                .ForeignKey("dbo.UserGroups", t => t.UserGroup_Id)
                .ForeignKey("dbo.ActivityViewSettings", t => t.ActivityViewSettings_Id)
                .Index(t => t.UserGroup_Id)
                .Index(t => t.ActivityViewSettings_Id);
            
            CreateTable(
                "dbo.ActivityViewSettingsUsers",
                c => new
                    {
                        ActivityViewSettings_Id = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.ActivityViewSettings_Id, t.User_Id })
                .ForeignKey("dbo.ActivityViewSettings", t => t.ActivityViewSettings_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.ActivityViewSettings_Id)
                .Index(t => t.User_Id);
            
            AddPrimaryKey("dbo.OpportunityCategoryProductionViewSettings", new[] { "OpportunityCategory_Id", "ProductionViewSettings_Id" });
            AddPrimaryKey("dbo.ProductionViewSettingsUserGroups", new[] { "ProductionViewSettings_Id", "UserGroup_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivityViewSettings", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ActivityViewSettingsUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ActivityViewSettingsUsers", "ActivityViewSettings_Id", "dbo.ActivityViewSettings");
            DropForeignKey("dbo.UserGroupActivityViewSettings", "ActivityViewSettings_Id", "dbo.ActivityViewSettings");
            DropForeignKey("dbo.UserGroupActivityViewSettings", "UserGroup_Id", "dbo.UserGroups");
            DropIndex("dbo.ActivityViewSettingsUsers", new[] { "User_Id" });
            DropIndex("dbo.ActivityViewSettingsUsers", new[] { "ActivityViewSettings_Id" });
            DropIndex("dbo.UserGroupActivityViewSettings", new[] { "ActivityViewSettings_Id" });
            DropIndex("dbo.UserGroupActivityViewSettings", new[] { "UserGroup_Id" });
            DropIndex("dbo.ActivityViewSettings", new[] { "OwnerId" });
            DropPrimaryKey("dbo.ProductionViewSettingsUserGroups");
            DropPrimaryKey("dbo.OpportunityCategoryProductionViewSettings");
            DropTable("dbo.ActivityViewSettingsUsers");
            DropTable("dbo.UserGroupActivityViewSettings");
            DropTable("dbo.ActivityViewSettings");
            AddPrimaryKey("dbo.ProductionViewSettingsUserGroups", new[] { "UserGroup_Id", "ProductionViewSettings_Id" });
            AddPrimaryKey("dbo.OpportunityCategoryProductionViewSettings", new[] { "ProductionViewSettings_Id", "OpportunityCategory_Id" });
            CreateIndex("dbo.ProductionViewSettings", "Name", unique: true);
            RenameTable(name: "dbo.ProductionViewSettingsUserGroups", newName: "UserGroupProductionViewSettings");
            RenameTable(name: "dbo.OpportunityCategoryProductionViewSettings", newName: "ProductionViewSettingsOpportunityCategories");
        }
    }
}
