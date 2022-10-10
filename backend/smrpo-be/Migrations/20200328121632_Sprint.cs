using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace smrpo_be.Migrations
{
    public partial class Sprint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SprintId",
                table: "UserStories",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sprints",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    Velocity = table.Column<int>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sprints_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStories_SprintId",
                table: "UserStories",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProjectId",
                table: "Sprints",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStories_Sprints_SprintId",
                table: "UserStories",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStories_Sprints_SprintId",
                table: "UserStories");

            migrationBuilder.DropTable(
                name: "Sprints");

            migrationBuilder.DropIndex(
                name: "IX_UserStories_SprintId",
                table: "UserStories");

            migrationBuilder.DropColumn(
                name: "SprintId",
                table: "UserStories");
        }
    }
}
