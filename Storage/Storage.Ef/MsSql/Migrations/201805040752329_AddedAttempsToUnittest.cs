namespace Zidium.Storage.Ef.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAttempsToUnittest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnitTests", "AttempCount", c => c.Int(nullable: false));
            AddColumn("dbo.UnitTests", "AttempMax", c => c.Int(nullable: false, defaultValue: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UnitTests", "AttempMax");
            DropColumn("dbo.UnitTests", "AttempCount");
        }
    }
}
