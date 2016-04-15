namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefreshToken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 100),
                        Secret = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 100),
                        Subject = c.String(nullable: false, maxLength: 100),
                        ClientId = c.String(nullable: false, maxLength: 100),
                        IssuedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ExpiresUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ProtectedTicket = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.Clients");
        }
    }
}
