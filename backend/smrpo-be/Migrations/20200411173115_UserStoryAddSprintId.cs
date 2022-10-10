using Microsoft.EntityFrameworkCore.Migrations;

namespace smrpo_be.Migrations
{
    public partial class UserStoryAddSprintId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStories_Sprints_SprintId",
                table: "UserStories");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStories_Sprints_SprintId",
                table: "UserStories",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStories_Sprints_SprintId",
                table: "UserStories");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStories_Sprints_SprintId",
                table: "UserStories",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
