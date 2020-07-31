namespace Zidium.Storage.Ef.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedAccountTariffsTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AccountTariffs", "TariffLimitId", "dbo.TariffLimits");
            DropIndex("dbo.AccountTariffs", new[] { "TariffLimitId" });
            DropTable("dbo.AccountTariffs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AccountTariffs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TariffLimitId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id, clustered: false);
            
            CreateIndex("dbo.AccountTariffs", "TariffLimitId");
            AddForeignKey("dbo.AccountTariffs", "TariffLimitId", "dbo.TariffLimits", "Id");
        }
    }
}
