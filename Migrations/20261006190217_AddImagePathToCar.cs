using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbase.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                schema: "carbase",
                table: "Cars",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                schema: "carbase",
                table: "Cars");
        }
    }
}
