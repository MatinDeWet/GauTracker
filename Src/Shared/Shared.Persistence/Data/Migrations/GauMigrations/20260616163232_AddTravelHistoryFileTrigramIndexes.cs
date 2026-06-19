using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Persistence.Data.Migrations.GauMigrations
{
    /// <inheritdoc />
    public partial class AddTravelHistoryFileTrigramIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateIndex(
                name: "IX_TravelHistoryFile_DisplayName",
                schema: "public",
                table: "TravelHistoryFile",
                column: "DisplayName")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_TravelHistoryFile_FileName",
                schema: "public",
                table: "TravelHistoryFile",
                column: "FileName")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TravelHistoryFile_DisplayName",
                schema: "public",
                table: "TravelHistoryFile");

            migrationBuilder.DropIndex(
                name: "IX_TravelHistoryFile_FileName",
                schema: "public",
                table: "TravelHistoryFile");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
        }
    }
}
