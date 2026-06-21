using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shared.Persistence.Data.Migrations.GauMigrations
{
    /// <inheritdoc />
    public partial class AddStationAndService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Service",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Station",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    StationType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IsTerminal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationService",
                schema: "public",
                columns: table => new
                {
                    StationId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationService", x => new { x.StationId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_StationService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StationService_Station_StationId",
                        column: x => x.StationId,
                        principalSchema: "public",
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Service",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Rapid rail access via the personalised Gold Card (tap in / tap out).", "Train service" },
                    { 2, "Secure, access-controlled parking; fee auto-charged to Gold Card on exit.", "Parking" },
                    { 3, "Integrated Gautrain buses connecting the station to surrounding areas.", "Feeder & distribution buses" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Station",
                columns: new[] { "Id", "Address", "IsTerminal", "Latitude", "Longitude", "Name", "StationType" },
                values: new object[,]
                {
                    { 1, "Acacia & Grosvenor Street, east of Pretoria CBD", true, -25.747650m, 28.238000m, "Hatfield", "AtGrade" },
                    { 2, "Scheiding St, adjacent to Pretoria Main Station", false, -25.758167m, 28.189467m, "Pretoria", "AtGrade" },
                    { 3, "West Avenue & Gerard Street, near Centurion Lake", false, -25.851617m, 28.189733m, "Centurion", "Elevated" },
                    { 4, "Old Pretoria-Johannesburg Road (K101)", false, -25.996483m, 28.137583m, "Midrand", "AtGrade" },
                    { 5, "Cnr Football & Laduma, Alexandra", false, -26.083917m, 28.111633m, "Marlboro", "AtGrade" },
                    { 6, "Rivonia Road, between West St & Fifth Ave", false, -26.107800m, 28.057500m, "Sandton", "Underground" },
                    { 7, "Oxford Road, between Baker St & Tyrwhitt Ave", false, -26.145250m, 28.043867m, "Rosebank", "Underground" },
                    { 8, "Cnr Smit & Wolmarans St, Braamfontein", true, -26.195533m, 28.041550m, "Park", "Underground" },
                    { 9, "Anson Street, Kempton Park", false, -26.128333m, 28.225233m, "Rhodesfield", "AtGrade" },
                    { 10, "OR Tambo International Airport, Kempton Park", true, -26.132933m, 28.231567m, "OR Tambo", "Elevated" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "StationService",
                columns: new[] { "ServiceId", "StationId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 1, 3 },
                    { 2, 3 },
                    { 3, 3 },
                    { 1, 4 },
                    { 2, 4 },
                    { 3, 4 },
                    { 1, 5 },
                    { 2, 5 },
                    { 3, 5 },
                    { 1, 6 },
                    { 2, 6 },
                    { 3, 6 },
                    { 1, 7 },
                    { 2, 7 },
                    { 3, 7 },
                    { 1, 8 },
                    { 2, 8 },
                    { 3, 8 },
                    { 1, 9 },
                    { 2, 9 },
                    { 3, 9 },
                    { 1, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Service_Description",
                schema: "public",
                table: "Service",
                column: "Description")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Service_Name",
                schema: "public",
                table: "Service",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Station_Address",
                schema: "public",
                table: "Station",
                column: "Address")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Station_Name",
                schema: "public",
                table: "Station",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_StationService_ServiceId",
                schema: "public",
                table: "StationService",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StationService",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Service",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Station",
                schema: "public");
        }
    }
}
