namespace Zidium.Core.AccountsDb.Migrations.PostgreSql
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SendMessageCommandTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SendMessageCommand",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Channel = c.Int(nullable: false),
                        To = c.String(nullable: false, maxLength: 255),
                        Body = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        ErrorMessage = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        SendDate = c.DateTime(),
                        ReferenceId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .Index(t => new { t.Channel, t.Status }, name: "IX_ForSend");
            
            AddColumn("dbo.Notifications", "SendEmailCommandId", c => c.Guid());
            AddColumn("dbo.Notifications", "SendMessageCommandId", c => c.Guid());
            CreateIndex("dbo.Notifications", "SendEmailCommandId");
            CreateIndex("dbo.Notifications", "SendMessageCommandId");
            AddForeignKey("dbo.Notifications", "SendEmailCommandId", "dbo.SendEmailCommand", "Id");
            AddForeignKey("dbo.Notifications", "SendMessageCommandId", "dbo.SendMessageCommand", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "SendMessageCommandId", "dbo.SendMessageCommand");
            DropForeignKey("dbo.Notifications", "SendEmailCommandId", "dbo.SendEmailCommand");
            DropIndex("dbo.SendMessageCommand", "IX_ForSend");
            DropIndex("dbo.Notifications", new[] { "SendMessageCommandId" });
            DropIndex("dbo.Notifications", new[] { "SendEmailCommandId" });
            DropColumn("dbo.Notifications", "SendMessageCommandId");
            DropColumn("dbo.Notifications", "SendEmailCommandId");
            DropTable("dbo.SendMessageCommand");
        }
    }
}
