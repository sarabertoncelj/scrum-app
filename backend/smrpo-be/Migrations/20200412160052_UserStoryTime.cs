using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace smrpo_be.Migrations
{
    public partial class UserStoryTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estimation",
                table: "UserStories");

            migrationBuilder.CreateTable(
                name: "UserStoryTimes",
                columns: table => new
                {
                    UserStoryId = table.Column<Guid>(nullable: false),
                    SprintId = table.Column<Guid>(nullable: false),
                    Estimation = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStoryTimes", x => new { x.UserStoryId, x.SprintId });
                    table.ForeignKey(
                        name: "FK_UserStoryTimes_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserStoryTimes_UserStories_UserStoryId",
                        column: x => x.UserStoryId,
                        principalTable: "UserStories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStoryTimes_SprintId",
                table: "UserStoryTimes",
                column: "SprintId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStoryTimes");

            migrationBuilder.AddColumn<int>(
                name: "Estimation",
                table: "UserStories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
