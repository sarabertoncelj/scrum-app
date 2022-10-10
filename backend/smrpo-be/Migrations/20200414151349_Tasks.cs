using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace smrpo_be.Migrations
{
    public partial class Tasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sprints_Projects_ProjectId",
                table: "Sprints");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Sprints",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    RemainingTime = table.Column<float>(nullable: false),
                    ActiveFrom = table.Column<DateTime>(nullable: false),
                    Accepted = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UserStoryId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_UserStories_UserStoryId",
                        column: x => x.UserStoryId,
                        principalTable: "UserStories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserStoryId",
                table: "Tasks",
                column: "UserStoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sprints_Projects_ProjectId",
                table: "Sprints",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sprints_Projects_ProjectId",
                table: "Sprints");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "Sprints",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Sprints_Projects_ProjectId",
                table: "Sprints",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
