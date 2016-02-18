namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StageNameUniqueue : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Stages", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Stages", new[] { "Name" });
        }
    }
}
