using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class TryToEnableCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoreSampleImage_ImageInfo_ImageInfoId",
                table: "CoreSampleImage");

            migrationBuilder.DropIndex(
                name: "IX_CoreSampleImage_ImageInfoId",
                table: "CoreSampleImage");

            migrationBuilder.CreateIndex(
                name: "IX_CoreSampleImage_ImageInfoId",
                table: "CoreSampleImage",
                column: "ImageInfoId",
                unique: true,
                filter: "[ImageInfoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CoreSampleImage_ImageInfo_ImageInfoId",
                table: "CoreSampleImage",
                column: "ImageInfoId",
                principalTable: "ImageInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoreSampleImage_ImageInfo_ImageInfoId",
                table: "CoreSampleImage");

            migrationBuilder.DropIndex(
                name: "IX_CoreSampleImage_ImageInfoId",
                table: "CoreSampleImage");

            migrationBuilder.CreateIndex(
                name: "IX_CoreSampleImage_ImageInfoId",
                table: "CoreSampleImage",
                column: "ImageInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoreSampleImage_ImageInfo_ImageInfoId",
                table: "CoreSampleImage",
                column: "ImageInfoId",
                principalTable: "ImageInfo",
                principalColumn: "Id");
        }
    }
}
