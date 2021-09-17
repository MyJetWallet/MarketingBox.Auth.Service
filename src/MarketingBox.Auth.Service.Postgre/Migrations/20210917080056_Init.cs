using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Auth.Service.Postgre.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth-service");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "auth-service",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    EmailEncrypted = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Salt = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => new { x.TenantId, x.EmailEncrypted });
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId_Username",
                schema: "auth-service",
                table: "users",
                columns: new[] { "TenantId", "Username" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users",
                schema: "auth-service");
        }
    }
}
