using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class MorePorosityInfoNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "pixelLengthRatio",
                table: "PoreInfo",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "pixelLengthRatio",
                table: "PoreInfo",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
