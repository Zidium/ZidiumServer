namespace Zidium.Storage.Ef.Migrations.PostgreSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTcpPortRules : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UnitTestTcpPortRules",
                c => new
                    {
                        UnitTestId = c.Guid(nullable: false),
                        Host = c.String(),
                        TimeoutMs = c.Int(nullable: false),
                        Port = c.Int(nullable: false),
                        Opened = c.Boolean(nullable: false),
                        LastRunErrorCode = c.Int(),
                    })
                .PrimaryKey(t => t.UnitTestId, clustered: false)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.UnitTestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UnitTestTcpPortRules", "UnitTestId", "dbo.UnitTests");
            DropIndex("dbo.UnitTestTcpPortRules", new[] { "UnitTestId" });
            DropTable("dbo.UnitTestTcpPortRules");
        }
    }
}
