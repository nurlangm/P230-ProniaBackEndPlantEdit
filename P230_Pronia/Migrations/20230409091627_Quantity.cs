using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P230_Pronia.Migrations
{
    public partial class Quantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantities",
                table: "Plants",
                newName: "Quantity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Plants",
                newName: "Quantities");
        }
    }
}
