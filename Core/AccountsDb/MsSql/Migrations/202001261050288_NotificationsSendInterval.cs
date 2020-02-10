namespace Zidium.Core.AccountsDb.Migrations.MsSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationsSendInterval : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "SendOnlyInInterval", c => c.Boolean(nullable: false));
            AddColumn("dbo.Subscriptions", "SendIntervalFromHour", c => c.Int());
            AddColumn("dbo.Subscriptions", "SendIntervalFromMinute", c => c.Int());
            AddColumn("dbo.Subscriptions", "SendIntervalToHour", c => c.Int());
            AddColumn("dbo.Subscriptions", "SendIntervalToMinute", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subscriptions", "SendIntervalToMinute");
            DropColumn("dbo.Subscriptions", "SendIntervalToHour");
            DropColumn("dbo.Subscriptions", "SendIntervalFromMinute");
            DropColumn("dbo.Subscriptions", "SendIntervalFromHour");
            DropColumn("dbo.Subscriptions", "SendOnlyInInterval");
        }
    }
}
