namespace Zidium.Storage.Ef.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletedAttempsFromPingRule : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.HttpRequestUnitTests", "ProcessAllRulesOnError");
            DropColumn("dbo.UnitTestPingRules", "Attemps");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UnitTestPingRules", "Attemps", c => c.Int(nullable: false));
            AddColumn("dbo.HttpRequestUnitTests", "ProcessAllRulesOnError", c => c.Boolean(nullable: false));
        }
    }
}
