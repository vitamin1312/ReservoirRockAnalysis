using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class MorePorosityInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "pixelLengthRatio",
                table: "PoreInfo",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pixelLengthRatio",
                table: "PoreInfo");
        }
    }
}
