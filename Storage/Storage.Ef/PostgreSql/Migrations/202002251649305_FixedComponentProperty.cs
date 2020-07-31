namespace Zidium.Storage.Ef.Migrations.PostgreSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedComponentProperty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ComponentProperties", "Name", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ComponentProperties", "Name", c => c.String());
        }
    }
}
