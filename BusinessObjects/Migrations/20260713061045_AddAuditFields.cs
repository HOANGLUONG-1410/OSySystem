using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByAccountId",
                table: "TransferRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedByAccountId",
                table: "TransferRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SentByAccountId",
                table: "TransferRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByAccountId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformedByAccountId",
                table: "InventoryLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByAccountId",
                table: "Adjustments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_CreatedByAccountId",
                table: "TransferRequests",
                column: "CreatedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_ReceivedByAccountId",
                table: "TransferRequests",
                column: "ReceivedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_SentByAccountId",
                table: "TransferRequests",
                column: "SentByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedByAccountId",
                table: "Orders",
                column: "CreatedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLogs_PerformedByAccountId",
                table: "InventoryLogs",
                column: "PerformedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Adjustments_CreatedByAccountId",
                table: "Adjustments",
                column: "CreatedByAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Adjustments_SystemAccounts_CreatedByAccountId",
                table: "Adjustments",
                column: "CreatedByAccountId",
                principalTable: "SystemAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLogs_SystemAccounts_PerformedByAccountId",
                table: "InventoryLogs",
                column: "PerformedByAccountId",
                principalTable: "SystemAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_SystemAccounts_CreatedByAccountId",
                table: "Orders",
                column: "CreatedByAccountId",
                principalTable: "SystemAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_SystemAccounts_CreatedByAccountId",
                table: "TransferRequests",
                column: "CreatedByAccountId",
                principalTable: "SystemAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_SystemAccounts_ReceivedByAccountId",
                table: "TransferRequests",
                column: "ReceivedByAccountId",
                principalTable: "SystemAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_SystemAccounts_SentByAccountId",
                table: "TransferRequests",
                column: "SentByAccountId",
                principalTable: "SystemAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adjustments_SystemAccounts_CreatedByAccountId",
                table: "Adjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLogs_SystemAccounts_PerformedByAccountId",
                table: "InventoryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_SystemAccounts_CreatedByAccountId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_SystemAccounts_CreatedByAccountId",
                table: "TransferRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_SystemAccounts_ReceivedByAccountId",
                table: "TransferRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_SystemAccounts_SentByAccountId",
                table: "TransferRequests");

            migrationBuilder.DropIndex(
                name: "IX_TransferRequests_CreatedByAccountId",
                table: "TransferRequests");

            migrationBuilder.DropIndex(
                name: "IX_TransferRequests_ReceivedByAccountId",
                table: "TransferRequests");

            migrationBuilder.DropIndex(
                name: "IX_TransferRequests_SentByAccountId",
                table: "TransferRequests");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CreatedByAccountId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_InventoryLogs_PerformedByAccountId",
                table: "InventoryLogs");

            migrationBuilder.DropIndex(
                name: "IX_Adjustments_CreatedByAccountId",
                table: "Adjustments");

            migrationBuilder.DropColumn(
                name: "CreatedByAccountId",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "ReceivedByAccountId",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "SentByAccountId",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "CreatedByAccountId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PerformedByAccountId",
                table: "InventoryLogs");

            migrationBuilder.DropColumn(
                name: "CreatedByAccountId",
                table: "Adjustments");
        }
    }
}
