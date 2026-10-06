using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbase.Migrations
{
    /// <inheritdoc />
    public partial class AddTuner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cars_Brand_Model_Year",
                schema: "carbase",
                table: "Cars");

            migrationBuilder.AddColumn<string>(
                name: "Tuner",
                schema: "carbase",
                table: "Cars",
                type: "citext",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Brand_Model_Year_Tuner",
                schema: "carbase",
                table: "Cars",
                columns: new[] { "Brand", "Model", "Year", "Tuner" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cars_Brand_Model_Year_Tuner",
                schema: "carbase",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Tuner",
                schema: "carbase",
                table: "Cars");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Brand_Model_Year",
                schema: "carbase",
                table: "Cars",
                columns: new[] { "Brand", "Model", "Year" },
                unique: true);
        }
    }
}
