namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyRequiredFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Companies", "City", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Companies", "Country", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Companies", "PostalCode", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Companies", "PostalCode", c => c.String(maxLength: 100));
            AlterColumn("dbo.Companies", "Country", c => c.String(maxLength: 100));
            AlterColumn("dbo.Companies", "City", c => c.String(maxLength: 100));
        }
    }
}
