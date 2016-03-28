namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductionViewSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductionViewSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Weighted = c.Boolean(nullable: false),
                        Private = c.Boolean(nullable: false),
                        Name = c.String(maxLength: 100),
                        OwnerId = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.Name, unique: true)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.ProductionViewSettingsOpportunityCategories",
                c => new
                    {
                        ProductionViewSettings_Id = c.Int(nullable: false),
                        OpportunityCategory_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductionViewSettings_Id, t.OpportunityCategory_Id })
                .ForeignKey("dbo.ProductionViewSettings", t => t.ProductionViewSettings_Id)
                .ForeignKey("dbo.OpportunityCategories", t => t.OpportunityCategory_Id)
                .Index(t => t.ProductionViewSettings_Id)
                .Index(t => t.OpportunityCategory_Id);
            
            CreateTable(
                "dbo.DepartmentProductionViewSettings",
                c => new
                    {
                        Department_Id = c.Int(nullable: false),
                        ProductionViewSettings_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Department_Id, t.ProductionViewSettings_Id })
                .ForeignKey("dbo.Departments", t => t.Department_Id)
                .ForeignKey("dbo.ProductionViewSettings", t => t.ProductionViewSettings_Id)
                .Index(t => t.Department_Id)
                .Index(t => t.ProductionViewSettings_Id);
            
            CreateTable(
                "dbo.StageProductionViewSettings",
                c => new
                    {
                        Stage_Id = c.Int(nullable: false),
                        ProductionViewSettings_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Stage_Id, t.ProductionViewSettings_Id })
                .ForeignKey("dbo.Stages", t => t.Stage_Id)
                .ForeignKey("dbo.ProductionViewSettings", t => t.ProductionViewSettings_Id)
                .Index(t => t.Stage_Id)
                .Index(t => t.ProductionViewSettings_Id);
            
            CreateTable(
                "dbo.UserGroupProductionViewSettings",
                c => new
                    {
                        UserGroup_Id = c.Int(nullable: false),
                        ProductionViewSettings_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserGroup_Id, t.ProductionViewSettings_Id })
                .ForeignKey("dbo.UserGroups", t => t.UserGroup_Id)
                .ForeignKey("dbo.ProductionViewSettings", t => t.ProductionViewSettings_Id)
                .Index(t => t.UserGroup_Id)
                .Index(t => t.ProductionViewSettings_Id);
            
            CreateTable(
                "dbo.ProductionViewSettingsUsers",
                c => new
                    {
                        ProductionViewSettings_Id = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.ProductionViewSettings_Id, t.User_Id })
                .ForeignKey("dbo.ProductionViewSettings", t => t.ProductionViewSettings_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.ProductionViewSettings_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductionViewSettings", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductionViewSettingsUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductionViewSettingsUsers", "ProductionViewSettings_Id", "dbo.ProductionViewSettings");
            DropForeignKey("dbo.UserGroupProductionViewSettings", "ProductionViewSettings_Id", "dbo.ProductionViewSettings");
            DropForeignKey("dbo.UserGroupProductionViewSettings", "UserGroup_Id", "dbo.UserGroups");
            DropForeignKey("dbo.StageProductionViewSettings", "ProductionViewSettings_Id", "dbo.ProductionViewSettings");
            DropForeignKey("dbo.StageProductionViewSettings", "Stage_Id", "dbo.Stages");
            DropForeignKey("dbo.DepartmentProductionViewSettings", "ProductionViewSettings_Id", "dbo.ProductionViewSettings");
            DropForeignKey("dbo.DepartmentProductionViewSettings", "Department_Id", "dbo.Departments");
            DropForeignKey("dbo.ProductionViewSettingsOpportunityCategories", "OpportunityCategory_Id", "dbo.OpportunityCategories");
            DropForeignKey("dbo.ProductionViewSettingsOpportunityCategories", "ProductionViewSettings_Id", "dbo.ProductionViewSettings");
            DropIndex("dbo.ProductionViewSettingsUsers", new[] { "User_Id" });
            DropIndex("dbo.ProductionViewSettingsUsers", new[] { "ProductionViewSettings_Id" });
            DropIndex("dbo.UserGroupProductionViewSettings", new[] { "ProductionViewSettings_Id" });
            DropIndex("dbo.UserGroupProductionViewSettings", new[] { "UserGroup_Id" });
            DropIndex("dbo.StageProductionViewSettings", new[] { "ProductionViewSettings_Id" });
            DropIndex("dbo.StageProductionViewSettings", new[] { "Stage_Id" });
            DropIndex("dbo.DepartmentProductionViewSettings", new[] { "ProductionViewSettings_Id" });
            DropIndex("dbo.DepartmentProductionViewSettings", new[] { "Department_Id" });
            DropIndex("dbo.ProductionViewSettingsOpportunityCategories", new[] { "OpportunityCategory_Id" });
            DropIndex("dbo.ProductionViewSettingsOpportunityCategories", new[] { "ProductionViewSettings_Id" });
            DropIndex("dbo.ProductionViewSettings", new[] { "OwnerId" });
            DropIndex("dbo.ProductionViewSettings", new[] { "Name" });
            DropTable("dbo.ProductionViewSettingsUsers");
            DropTable("dbo.UserGroupProductionViewSettings");
            DropTable("dbo.StageProductionViewSettings");
            DropTable("dbo.DepartmentProductionViewSettings");
            DropTable("dbo.ProductionViewSettingsOpportunityCategories");
            DropTable("dbo.ProductionViewSettings");
        }
    }
}
