namespace Zidium.Storage.Ef.Migrations.PostgreSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBodyToHttpUnittest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HttpRequestUnitTestRules", "Body", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HttpRequestUnitTestRules", "Body");
        }
    }
}
