using Microsoft.EntityFrameworkCore.Migrations;

namespace Zidium.Storage.Ef.PostgreSql.Migrations
{
    public partial class EventIndexesOptimization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountBased",
                schema: "dbo",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_ForJoin",
                schema: "dbo",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_ForProcessing",
                schema: "dbo",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_OwnerBased",
                schema: "dbo",
                table: "Events");

            migrationBuilder.CreateIndex(
                name: "IX_ForDeletion",
                schema: "dbo",
                table: "Events",
                columns: new[] { "Category", "ActualDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ForJoin",
                schema: "dbo",
                table: "Events",
                columns: new[] { "OwnerId", "EventTypeId", "Importance" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerBased",
                schema: "dbo",
                table: "Events",
                columns: new[] { "OwnerId", "Category", "StartDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ForDeletion",
                schema: "dbo",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_ForJoin",
                schema: "dbo",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_OwnerBased",
                schema: "dbo",
                table: "Events");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBased",
                schema: "dbo",
                table: "Events",
                columns: new[] { "Category", "ActualDate", "StartDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ForJoin",
                schema: "dbo",
                table: "Events",
                columns: new[] { "OwnerId", "EventTypeId", "Importance", "ActualDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ForProcessing",
                schema: "dbo",
                table: "Events",
                columns: new[] { "IsUserHandled", "EventTypeId", "VersionLong" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerBased",
                schema: "dbo",
                table: "Events",
                columns: new[] { "OwnerId", "Category", "ActualDate", "StartDate" });
        }
    }
}
