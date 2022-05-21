using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetScaServerData.Migrations
{
    public partial class loadcount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoadCount",
                table: "NuGetLoads",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadCount",
                table: "NuGetLoads");
        }
    }
}
