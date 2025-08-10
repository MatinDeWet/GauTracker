using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GauTracker.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStationAddUpdatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateUpdated",
                schema: "public",
                table: "Station",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "public",
                table: "Station");
        }
    }
}
