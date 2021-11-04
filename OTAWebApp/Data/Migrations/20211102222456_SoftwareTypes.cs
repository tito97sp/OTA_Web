using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OTAWebApp.Data.Migrations
{
    public partial class SoftwareTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoftwareType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoftwareVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoftwareTypeId = table.Column<int>(type: "int", nullable: false),
                    Major = table.Column<long>(type: "bigint", nullable: false),
                    Minor = table.Column<long>(type: "bigint", nullable: false),
                    Patch = table.Column<long>(type: "bigint", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirmwarePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoftwareVersion_SoftwareType_SoftwareTypeId",
                        column: x => x.SoftwareTypeId,
                        principalTable: "SoftwareType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoftwareVersion_SoftwareTypeId",
                table: "SoftwareVersion",
                column: "SoftwareTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoftwareVersion");

            migrationBuilder.DropTable(
                name: "SoftwareType");
        }
    }
}
