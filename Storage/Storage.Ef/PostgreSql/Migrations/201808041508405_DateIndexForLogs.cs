namespace Zidium.Storage.Ef.Migrations.PostgreSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateIndexForLogs : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Logs", "Date");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Logs", new[] { "Date" });
        }
    }
}
