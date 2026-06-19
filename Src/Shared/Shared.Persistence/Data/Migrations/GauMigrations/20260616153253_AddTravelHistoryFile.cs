using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace Shared.Persistence.Data.Migrations.GauMigrations
{
    /// <inheritdoc />
    public partial class AddTravelHistoryFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TravelHistoryFile",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CardId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    BlobContainer = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    BlobKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DateUploaded = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "FileName", "DisplayName" }),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelHistoryFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelHistoryFile_Card_CardId",
                        column: x => x.CardId,
                        principalSchema: "public",
                        principalTable: "Card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TravelHistoryFile_BlobKey",
                schema: "public",
                table: "TravelHistoryFile",
                column: "BlobKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TravelHistoryFile_CardId",
                schema: "public",
                table: "TravelHistoryFile",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelHistoryFile_SearchVector",
                schema: "public",
                table: "TravelHistoryFile",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelHistoryFile",
                schema: "public");
        }
    }
}
