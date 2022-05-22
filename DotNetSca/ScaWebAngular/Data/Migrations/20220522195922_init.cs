using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScaWebAngular.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InitiatorId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsFilledByServer = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Signatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ReportId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signatures_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PackageId = table.Column<string>(type: "TEXT", nullable: true),
                    PackageVersion = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ReportId = table.Column<int>(type: "INTEGER", nullable: true),
                    SignatureId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Packages_Signatures_SignatureId",
                        column: x => x.SignatureId,
                        principalTable: "Signatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vulnerabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CvssScore = table.Column<double>(type: "REAL", nullable: false),
                    CvssVector = table.Column<string>(type: "TEXT", nullable: true),
                    Cve = table.Column<string>(type: "TEXT", nullable: true),
                    Cwe = table.Column<string>(type: "TEXT", nullable: true),
                    Reference = table.Column<string>(type: "TEXT", nullable: true),
                    PackageId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vulnerabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vulnerabilities_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProjectId",
                table: "AspNetUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ReportId",
                table: "Packages",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_SignatureId",
                table: "Packages",
                column: "SignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_InitiatorId",
                table: "Reports",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ProjectId",
                table: "Reports",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_ReportId",
                table: "Signatures",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerabilities_PackageId",
                table: "Vulnerabilities",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Projects_ProjectId",
                table: "AspNetUsers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Projects_ProjectId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Vulnerabilities");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Signatures");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProjectId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "AspNetUsers");
        }
    }
}
