namespace Zidium.Storage.Ef.Migrations.PostgreSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VirusTotal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UnitTestVirusTotalRules",
                c => new
                    {
                        UnitTestId = c.Guid(nullable: false),
                        Url = c.String(nullable: false, maxLength: 2000),
                        Apikey = c.String(maxLength: 100),
                        LastRunErrorCode = c.Int(),
                    })
                .PrimaryKey(t => t.UnitTestId, clustered: false)
                .ForeignKey("dbo.UnitTests", t => t.UnitTestId)
                .Index(t => t.UnitTestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UnitTestVirusTotalRules", "UnitTestId", "dbo.UnitTests");
            DropIndex("dbo.UnitTestVirusTotalRules", new[] { "UnitTestId" });
            DropTable("dbo.UnitTestVirusTotalRules");
        }
    }
}
