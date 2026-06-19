using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Shared.Persistence.Data.Migrations.GauMigrations
{
    /// <inheritdoc />
    public partial class RemoveTravelHistoryFileSearchVector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TravelHistoryFile_SearchVector",
                schema: "public",
                table: "TravelHistoryFile");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "public",
                table: "TravelHistoryFile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "public",
                table: "TravelHistoryFile",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "FileName", "DisplayName" });

            migrationBuilder.CreateIndex(
                name: "IX_TravelHistoryFile_SearchVector",
                schema: "public",
                table: "TravelHistoryFile",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }
    }
}
