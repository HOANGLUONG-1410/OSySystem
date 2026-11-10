using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class RemoveScheduleFkFromScheduleLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleLogs_Schedules_ScheduleId",
                table: "ScheduleLogs");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleLogs_ScheduleId",
                table: "ScheduleLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ScheduleLogs_ScheduleId",
                table: "ScheduleLogs",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleLogs_Schedules_ScheduleId",
                table: "ScheduleLogs",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
