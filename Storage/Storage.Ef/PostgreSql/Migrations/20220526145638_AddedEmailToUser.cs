using Microsoft.EntityFrameworkCore.Migrations;

namespace Zidium.Storage.Ef.PostgreSql.Migrations
{
    public partial class AddedEmailToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Post",
                schema: "dbo",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                schema: "dbo",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EMail",
                schema: "dbo",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.Sql("UPDATE dbo.\"Users\" SET \"EMail\" = \"Login\"");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EMail",
                schema: "dbo",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                schema: "dbo",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "dbo",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "dbo",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                schema: "dbo",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Post",
                schema: "dbo",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
