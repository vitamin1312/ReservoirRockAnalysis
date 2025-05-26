using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoreInfo_CoreSampleImage_CoreSampleImageId",
                table: "PoreInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_PoreInfo_CoreSampleImage_CoreSampleImageId",
                table: "PoreInfo",
                column: "CoreSampleImageId",
                principalTable: "CoreSampleImage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoreInfo_CoreSampleImage_CoreSampleImageId",
                table: "PoreInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_PoreInfo_CoreSampleImage_CoreSampleImageId",
                table: "PoreInfo",
                column: "CoreSampleImageId",
                principalTable: "CoreSampleImage",
                principalColumn: "Id");
        }
    }
}
