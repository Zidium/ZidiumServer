namespace Zidium.Storage.Ef.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTimeZones : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimeZones",
                c => new
                    {
                        OffsetMinutes = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.OffsetMinutes, clustered: false);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TimeZones");
        }
    }
}
