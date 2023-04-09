using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P230_Pronia.Migrations
{
    public partial class Nebilim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantSizeColors_Quantity_quantityId",
                table: "PlantSizeColors");

            migrationBuilder.DropTable(
                name: "Quantity");

            migrationBuilder.DropIndex(
                name: "IX_PlantSizeColors_quantityId",
                table: "PlantSizeColors");

            migrationBuilder.DropColumn(
                name: "quantityId",
                table: "PlantSizeColors");

            migrationBuilder.AddColumn<int>(
                name: "Quantities",
                table: "Plants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantities",
                table: "Plants");

            migrationBuilder.AddColumn<int>(
                name: "quantityId",
                table: "PlantSizeColors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Quantity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quantity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantSizeColors_quantityId",
                table: "PlantSizeColors",
                column: "quantityId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantSizeColors_Quantity_quantityId",
                table: "PlantSizeColors",
                column: "quantityId",
                principalTable: "Quantity",
                principalColumn: "Id");
        }
    }
}
