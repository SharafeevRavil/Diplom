using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScaWebAngular.Data.Migrations
{
    public partial class fixProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Projects_ProjectId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProjectId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ApplicationUserProject",
                columns: table => new
                {
                    AllowedProjectsId = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedUsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserProject", x => new { x.AllowedProjectsId, x.AllowedUsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserProject_AspNetUsers_AllowedUsersId",
                        column: x => x.AllowedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserProject_Projects_AllowedProjectsId",
                        column: x => x.AllowedProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserProject_AllowedUsersId",
                table: "ApplicationUserProject",
                column: "AllowedUsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserProject");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProjectId",
                table: "AspNetUsers",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Projects_ProjectId",
                table: "AspNetUsers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
