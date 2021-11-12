using Microsoft.EntityFrameworkCore.Migrations;

namespace OTAWebApp.Migrations
{
    public partial class modifysoftwareversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoftwareVersionId",
                table: "HardwareType",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HardwareType_SoftwareVersionId",
                table: "HardwareType",
                column: "SoftwareVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_HardwareType_SoftwareVersion_SoftwareVersionId",
                table: "HardwareType",
                column: "SoftwareVersionId",
                principalTable: "SoftwareVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HardwareType_SoftwareVersion_SoftwareVersionId",
                table: "HardwareType");

            migrationBuilder.DropIndex(
                name: "IX_HardwareType_SoftwareVersionId",
                table: "HardwareType");

            migrationBuilder.DropColumn(
                name: "SoftwareVersionId",
                table: "HardwareType");
        }
    }
}
