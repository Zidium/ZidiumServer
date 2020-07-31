namespace Zidium.Storage.Ef.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateIndexForLog : DbMigration
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
