using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBook.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToTraditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Traditions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Traditions");
        }
    }
}