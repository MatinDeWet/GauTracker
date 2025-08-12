using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GauTracker.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionHistoryImportBatchCardId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CardId",
                schema: "public",
                table: "TransactionHistoryImportBatch",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistoryImportBatch_CardId",
                schema: "public",
                table: "TransactionHistoryImportBatch",
                column: "CardId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistoryImportBatch_Card_CardId",
                schema: "public",
                table: "TransactionHistoryImportBatch",
                column: "CardId",
                principalSchema: "public",
                principalTable: "Card",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistoryImportBatch_Card_CardId",
                schema: "public",
                table: "TransactionHistoryImportBatch");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistoryImportBatch_CardId",
                schema: "public",
                table: "TransactionHistoryImportBatch");

            migrationBuilder.DropColumn(
                name: "CardId",
                schema: "public",
                table: "TransactionHistoryImportBatch");
        }
    }
}
