using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Auth.Service.Postgre.Migrations
{
    public partial class ExternalUserIdMainKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                schema: "auth-service",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_TenantId_ExternalUserId",
                schema: "auth-service",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalUserId",
                schema: "auth-service",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailEncrypted",
                schema: "auth-service",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                schema: "auth-service",
                table: "users",
                columns: new[] { "TenantId", "ExternalUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId_EmailEncrypted",
                schema: "auth-service",
                table: "users",
                columns: new[] { "TenantId", "EmailEncrypted" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                schema: "auth-service",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_TenantId_EmailEncrypted",
                schema: "auth-service",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "EmailEncrypted",
                schema: "auth-service",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalUserId",
                schema: "auth-service",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                schema: "auth-service",
                table: "users",
                columns: new[] { "TenantId", "EmailEncrypted" });

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId_ExternalUserId",
                schema: "auth-service",
                table: "users",
                columns: new[] { "TenantId", "ExternalUserId" },
                unique: true);
        }
    }
}
