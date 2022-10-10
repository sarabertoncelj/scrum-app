using Microsoft.EntityFrameworkCore.Migrations;

namespace smrpo_be.Migrations
{
    public partial class UserStoryDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserStories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserStories");
        }
    }
}
