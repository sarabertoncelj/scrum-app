using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace smrpo_be.Migrations
{
    public partial class UserStory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserStories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    BusinessValue = table.Column<int>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcceptanceTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    UserStoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptanceTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcceptanceTests_UserStories_UserStoryId",
                        column: x => x.UserStoryId,
                        principalTable: "UserStories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptanceTests_UserStoryId",
                table: "AcceptanceTests",
                column: "UserStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStories_ProjectId",
                table: "UserStories",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptanceTests");

            migrationBuilder.DropTable(
                name: "UserStories");
        }
    }
}
