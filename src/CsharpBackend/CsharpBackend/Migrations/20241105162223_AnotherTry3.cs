using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class AnotherTry3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoreSampleImage_ImageInfo_ImageInfoId",
                table: "CoreSampleImage");

            migrationBuilder.AlterColumn<int>(
                name: "ImageInfoId",
                table: "CoreSampleImage",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<int>(
                name: "ImageInfoId",
                table: "CoreSampleImage",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CoreSampleImage_ImageInfo_ImageInfoId",
                table: "CoreSampleImage",
                column: "ImageInfoId",
                principalTable: "ImageInfo",
                principalColumn: "Id");
        }
    }
}
