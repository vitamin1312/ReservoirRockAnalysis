using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedFieldId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageInfo_Field_FieldId",
                table: "ImageInfo");

            migrationBuilder.AlterColumn<int>(
                name: "FieldId",
                table: "ImageInfo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageInfo_Field_FieldId",
                table: "ImageInfo",
                column: "FieldId",
                principalTable: "Field",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageInfo_Field_FieldId",
                table: "ImageInfo");

            migrationBuilder.AlterColumn<int>(
                name: "FieldId",
                table: "ImageInfo",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageInfo_Field_FieldId",
                table: "ImageInfo",
                column: "FieldId",
                principalTable: "Field",
                principalColumn: "Id");
        }
    }
}
