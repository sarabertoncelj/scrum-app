using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace smrpo_be.Migrations
{
    public partial class MultipleProjectRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProjectRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    UserProjectUserId = table.Column<Guid>(nullable: true),
                    UserProjectProjectId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProjectRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProjectRole_UserProjects_UserProjectUserId_UserProjectPr~",
                        columns: x => new { x.UserProjectUserId, x.UserProjectProjectId },
                        principalTable: "UserProjects",
                        principalColumns: new[] { "UserId", "ProjectId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectRole_UserProjectUserId_UserProjectProjectId",
                table: "UserProjectRole",
                columns: new[] { "UserProjectUserId", "UserProjectProjectId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProjectRole");
        }
    }
}
