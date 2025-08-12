using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GauTracker.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionHistoryImportBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionHistoryImportBatch",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BlobContainer = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BlobName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OutcomeFlags = table.Column<int>(type: "integer", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UploadedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    QueuedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CanceledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SupersededAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    HangfireJobId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionHistoryImportBatch", x => x.Id);
                    table.CheckConstraint("CK_TransactionHistoryImportBatch_Sha256_Hex", "\"Sha256\" ~ '^[0-9A-Fa-f]{64}$'");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistoryImportBatch_Sha256",
                schema: "public",
                table: "TransactionHistoryImportBatch",
                column: "Sha256",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistoryImportBatch_Status",
                schema: "public",
                table: "TransactionHistoryImportBatch",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistoryImportBatch_UploadedAt",
                schema: "public",
                table: "TransactionHistoryImportBatch",
                column: "UploadedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionHistoryImportBatch",
                schema: "public");
        }
    }
}
