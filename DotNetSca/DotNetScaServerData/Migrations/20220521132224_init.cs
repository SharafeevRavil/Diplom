using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DotNetScaServerData.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NuGetLoads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NuGetLoads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NuGetPackageToLoads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PackageId = table.Column<string>(type: "text", nullable: true),
                    PackageVersion = table.Column<string>(type: "text", nullable: true),
                    IsLoading = table.Column<bool>(type: "boolean", nullable: false),
                    IsLoaded = table.Column<bool>(type: "boolean", nullable: false),
                    LoadTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AddTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NuGetPackageToLoads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAuthentications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GeneratedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AuthHash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuthentications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NuGetPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PackageId = table.Column<string>(type: "text", nullable: true),
                    PackageVersion = table.Column<string>(type: "text", nullable: true),
                    LoadRequestId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NuGetPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NuGetPackages_NuGetPackageToLoads_LoadRequestId",
                        column: x => x.LoadRequestId,
                        principalTable: "NuGetPackageToLoads",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Signatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Hash = table.Column<byte[]>(type: "bytea", nullable: true),
                    PackageId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signatures_NuGetPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "NuGetPackages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NuGetPackages_LoadRequestId",
                table: "NuGetPackages",
                column: "LoadRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_PackageId",
                table: "Signatures",
                column: "PackageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NuGetLoads");

            migrationBuilder.DropTable(
                name: "Signatures");

            migrationBuilder.DropTable(
                name: "UserAuthentications");

            migrationBuilder.DropTable(
                name: "NuGetPackages");

            migrationBuilder.DropTable(
                name: "NuGetPackageToLoads");
        }
    }
}
