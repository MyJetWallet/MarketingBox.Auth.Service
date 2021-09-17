using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Auth.Service.Postgre.Migrations
{
    public partial class ExternalUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalUserId",
                schema: "auth-service",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId_ExternalUserId",
                schema: "auth-service",
                table: "users",
                columns: new[] { "TenantId", "ExternalUserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_TenantId_ExternalUserId",
                schema: "auth-service",
                table: "users");

            migrationBuilder.DropColumn(
                name: "ExternalUserId",
                schema: "auth-service",
                table: "users");
        }
    }
}
