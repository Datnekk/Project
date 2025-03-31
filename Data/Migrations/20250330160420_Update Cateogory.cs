using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCateogory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CategoryId",
                table: "Rooms",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Categories_CategoryId",
                table: "Rooms",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Categories_CategoryId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_CategoryId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Rooms");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
