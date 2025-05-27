using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class newimageinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "pixelLengthRatio",
                table: "ImageInfo",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pixelLengthRatio",
                table: "ImageInfo");
        }
    }
}
