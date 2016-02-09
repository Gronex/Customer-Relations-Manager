namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Done = c.Boolean(nullable: false),
                        DueDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Time = c.DateTime(precision: 7, storeType: "datetime2"),
                        ResponsibleId = c.String(nullable: false, maxLength: 100),
                        CompanyId = c.Int(),
                        PersonId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityCategories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.People", t => t.PersonId)
                .ForeignKey("dbo.AspNetUsers", t => t.ResponsibleId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.ResponsibleId)
                .Index(t => t.CompanyId)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.ActivityCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false, maxLength: 500),
                        Sent = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        UserId = c.String(maxLength: 100),
                        ActivityId = c.Int(),
                        OpportunityId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activities", t => t.ActivityId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Opportunities", t => t.OpportunityId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ActivityId)
                .Index(t => t.OpportunityId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 100),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(maxLength: 100),
                        SecurityStamp = c.String(maxLength: 100),
                        PhoneNumber = c.String(maxLength: 100),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 100),
                        ClaimType = c.String(maxLength: 100),
                        ClaimValue = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Opportunities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 500),
                        Amount = c.Double(nullable: false),
                        HourlyPrice = c.Double(nullable: false),
                        StartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ExpectedClose = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        OwnerId = c.String(nullable: false, maxLength: 100),
                        CompanyId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        StageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OpportunityCategories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .ForeignKey("dbo.Stages", t => t.StageId, cascadeDelete: true)
                .Index(t => t.OwnerId)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.CategoryId)
                .Index(t => t.StageId);
            
            CreateTable(
                "dbo.OpportunityCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        PhoneNumber = c.String(maxLength: 100),
                        Address = c.String(maxLength: 100),
                        City = c.String(maxLength: 100),
                        Country = c.String(maxLength: 100),
                        PostalCode = c.String(maxLength: 100),
                        WebSite = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        PersonId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        PhoneNumber = c.String(maxLength: 100),
                        Email = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserGroupOpportunities",
                c => new
                    {
                        OpportunityId = c.Int(nullable: false),
                        UserGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OpportunityId, t.UserGroupId })
                .ForeignKey("dbo.Opportunities", t => t.OpportunityId, cascadeDelete: true)
                .ForeignKey("dbo.UserGroups", t => t.UserGroupId, cascadeDelete: true)
                .Index(t => t.OpportunityId)
                .Index(t => t.UserGroupId);
            
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.UserGroupUsers",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 100),
                        UserGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.UserGroupId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.UserGroups", t => t.UserGroupId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.UserGroupId);
            
            CreateTable(
                "dbo.ProductionGoals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Goal = c.Double(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 100),
                        ProviderKey = c.String(nullable: false, maxLength: 100),
                        UserId = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 100),
                        RoleId = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ViewSettings",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 100),
                        Month = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.FileIndexes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OriginalName = c.String(nullable: false, maxLength: 100),
                        FilePath = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Activities", "ResponsibleId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Activities", "PersonId", "dbo.People");
            DropForeignKey("dbo.Activities", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ViewSettings", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductionGoals", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "OpportunityId", "dbo.Opportunities");
            DropForeignKey("dbo.UserGroupOpportunities", "UserGroupId", "dbo.UserGroups");
            DropForeignKey("dbo.UserGroupUsers", "UserGroupId", "dbo.UserGroups");
            DropForeignKey("dbo.UserGroupUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserGroupOpportunities", "OpportunityId", "dbo.Opportunities");
            DropForeignKey("dbo.Opportunities", "StageId", "dbo.Stages");
            DropForeignKey("dbo.Opportunities", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Opportunities", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Opportunities", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Contracts", "PersonId", "dbo.People");
            DropForeignKey("dbo.Contracts", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Opportunities", "CategoryId", "dbo.OpportunityCategories");
            DropForeignKey("dbo.Comments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "ActivityId", "dbo.Activities");
            DropForeignKey("dbo.Activities", "CategoryId", "dbo.ActivityCategories");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ViewSettings", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.ProductionGoals", new[] { "UserId" });
            DropIndex("dbo.UserGroupUsers", new[] { "UserGroupId" });
            DropIndex("dbo.UserGroupUsers", new[] { "UserId" });
            DropIndex("dbo.UserGroups", new[] { "Name" });
            DropIndex("dbo.UserGroupOpportunities", new[] { "UserGroupId" });
            DropIndex("dbo.UserGroupOpportunities", new[] { "OpportunityId" });
            DropIndex("dbo.Contracts", new[] { "PersonId" });
            DropIndex("dbo.Contracts", new[] { "CompanyId" });
            DropIndex("dbo.Opportunities", new[] { "StageId" });
            DropIndex("dbo.Opportunities", new[] { "CategoryId" });
            DropIndex("dbo.Opportunities", new[] { "DepartmentId" });
            DropIndex("dbo.Opportunities", new[] { "CompanyId" });
            DropIndex("dbo.Opportunities", new[] { "OwnerId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Comments", new[] { "OpportunityId" });
            DropIndex("dbo.Comments", new[] { "ActivityId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropIndex("dbo.Activities", new[] { "PersonId" });
            DropIndex("dbo.Activities", new[] { "CompanyId" });
            DropIndex("dbo.Activities", new[] { "ResponsibleId" });
            DropIndex("dbo.Activities", new[] { "CategoryId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.FileIndexes");
            DropTable("dbo.ViewSettings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.ProductionGoals");
            DropTable("dbo.UserGroupUsers");
            DropTable("dbo.UserGroups");
            DropTable("dbo.UserGroupOpportunities");
            DropTable("dbo.Stages");
            DropTable("dbo.Departments");
            DropTable("dbo.People");
            DropTable("dbo.Contracts");
            DropTable("dbo.Companies");
            DropTable("dbo.OpportunityCategories");
            DropTable("dbo.Opportunities");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Comments");
            DropTable("dbo.ActivityCategories");
            DropTable("dbo.Activities");
        }
    }
}
