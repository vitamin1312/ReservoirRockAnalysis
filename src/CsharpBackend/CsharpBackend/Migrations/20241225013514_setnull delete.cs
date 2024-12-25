using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class setnulldelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageInfo_Field_FieldId",
                table: "ImageInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageInfo_Field_FieldId",
                table: "ImageInfo",
                column: "FieldId",
                principalTable: "Field",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageInfo_Field_FieldId",
                table: "ImageInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageInfo_Field_FieldId",
                table: "ImageInfo",
                column: "FieldId",
                principalTable: "Field",
                principalColumn: "Id");
        }
    }
}
