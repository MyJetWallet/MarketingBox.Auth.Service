using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Auth.Service.Postgre.Migrations
{
    public partial class UniqueKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_TenantId_Username",
                schema: "auth-service",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId_Username",
                schema: "auth-service",
                table: "users",
                columns: new[] { "TenantId", "Username" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_TenantId_Username",
                schema: "auth-service",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId_Username",
                schema: "auth-service",
                table: "users",
                columns: new[] { "TenantId", "Username" });
        }
    }
}
