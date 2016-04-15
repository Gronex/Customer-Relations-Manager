namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefreshTokenTicketLimitIncrease : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RefreshTokens", "ProtectedTicket", c => c.String(nullable: false, maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RefreshTokens", "ProtectedTicket", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
