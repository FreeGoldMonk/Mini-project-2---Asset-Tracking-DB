using Microsoft.EntityFrameworkCore.Migrations;

namespace Mini_project_2__Asset_Tracking_DB.Migrations
{
    public partial class WithSortingId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortingId",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortingId",
                table: "Assets");
        }
    }
}
