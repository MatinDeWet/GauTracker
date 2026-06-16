using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.infrastructure.Data.Migrations.GauMigrations
{
    /// <inheritdoc />
    public partial class AddUserIdentityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                schema: "public",
                table: "User",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdentityId",
                schema: "public",
                table: "User",
                column: "IdentityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_IdentityId",
                schema: "public",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                schema: "public",
                table: "User");
        }
    }
}
