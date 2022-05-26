using Microsoft.EntityFrameworkCore.Migrations;

namespace Zidium.Storage.Ef.Sqlite.Migrations
{
    public partial class AddedEmailToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountBased",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_ForJoin",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_ForProcessing",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_OwnerBased",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Post",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "EMail",
                table: "Users",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                collation: "UTF8CI");

            migrationBuilder.CreateIndex(
                name: "IX_ForDeletion",
                table: "Events",
                columns: new[] { "Category", "ActualDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ForJoin",
                table: "Events",
                columns: new[] { "OwnerId", "EventTypeId", "Importance" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerBased",
                table: "Events",
                columns: new[] { "OwnerId", "Category", "StartDate" });

            migrationBuilder.Sql("UPDATE \"Users\" SET \"EMail\" = \"Login\"");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ForDeletion",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_ForJoin",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_OwnerBased",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EMail",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                collation: "UTF8CI");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                collation: "UTF8CI");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                collation: "UTF8CI");

            migrationBuilder.AddColumn<string>(
                name: "Post",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                collation: "UTF8CI");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBased",
                table: "Events",
                columns: new[] { "Category", "ActualDate", "StartDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ForJoin",
                table: "Events",
                columns: new[] { "OwnerId", "EventTypeId", "Importance", "ActualDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ForProcessing",
                table: "Events",
                columns: new[] { "IsUserHandled", "EventTypeId", "VersionLong" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerBased",
                table: "Events",
                columns: new[] { "OwnerId", "Category", "ActualDate", "StartDate" });
        }
    }
}
