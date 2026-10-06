using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Carbase.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "carbase");

            migrationBuilder.CreateTable(
                name: "Cars",
                schema: "carbase",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    WeightKg = table.Column<int>(type: "integer", nullable: true),
                    HorsePower = table.Column<int>(type: "integer", nullable: true),
                    TorqueNm = table.Column<int>(type: "integer", nullable: true),
                    TopSpeed = table.Column<int>(type: "integer", nullable: true),
                    Time0To100 = table.Column<decimal>(type: "numeric", nullable: true),
                    Time100To200 = table.Column<decimal>(type: "numeric", nullable: true),
                    Time200To250 = table.Column<decimal>(type: "numeric", nullable: true),
                    TimeQuarterMile = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cars",
                schema: "carbase");
        }
    }
}
