using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InccApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyVariationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MonthlyVariation",
                table: "incc_entries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyVariation",
                table: "incc_entries");
        }
    }
}
