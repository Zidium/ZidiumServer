namespace Zidium.Storage.Ef.Migrations.PostgreSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VirusTotal1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountSettings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id, clustered: false);
            
            AddColumn("dbo.UnitTests", "NextStepProcessDate", c => c.DateTime());
            AddColumn("dbo.UnitTestVirusTotalRules", "NextStep", c => c.Int(nullable: false));
            AddColumn("dbo.UnitTestVirusTotalRules", "ScanTime", c => c.DateTime());
            AddColumn("dbo.UnitTestVirusTotalRules", "ScanId", c => c.String(maxLength: 100));
            DropColumn("dbo.UnitTestVirusTotalRules", "Apikey");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UnitTestVirusTotalRules", "Apikey", c => c.String(maxLength: 100));
            DropColumn("dbo.UnitTestVirusTotalRules", "ScanId");
            DropColumn("dbo.UnitTestVirusTotalRules", "ScanTime");
            DropColumn("dbo.UnitTestVirusTotalRules", "NextStep");
            DropColumn("dbo.UnitTests", "NextStepProcessDate");
            DropTable("dbo.AccountSettings");
        }
    }
}
