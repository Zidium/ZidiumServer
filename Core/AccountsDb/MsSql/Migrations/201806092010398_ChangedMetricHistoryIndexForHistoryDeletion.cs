namespace Zidium.Core.AccountsDb.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedMetricHistoryIndexForHistoryDeletion : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.MetricHistory", "IX_ForHistoryDeletion");
            CreateIndex("dbo.MetricHistory", "BeginDate");
        }
        
        public override void Down()
        {
            DropIndex("dbo.MetricHistory", new[] { "BeginDate" });
            CreateIndex("dbo.MetricHistory", new[] { "ComponentId", "ActualDate" }, name: "IX_ForHistoryDeletion");
        }
    }
}
