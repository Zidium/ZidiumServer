namespace Zidium.Core.AccountsDb.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTcpPortRules : DbMigration
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
