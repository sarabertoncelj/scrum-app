using Microsoft.EntityFrameworkCore.Migrations;

namespace smrpo_be.Migrations
{
    public partial class UserStoryEstimation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estimation",
                table: "UserStories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estimation",
                table: "UserStories");
        }
    }
}
