namespace Zidium.Storage.Ef.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOpenedToTcpPortRules : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnitTestTcpPortRules", "Opened", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UnitTestTcpPortRules", "Opened");
        }
    }
}
