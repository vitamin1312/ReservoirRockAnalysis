using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddedFieldsInFieldTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Field",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FieldId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageInfo_Field_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Field",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CoreSampleImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PathToImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PathToMask = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InfoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreSampleImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreSampleImage_ImageInfo_InfoId",
                        column: x => x.InfoId,
                        principalTable: "ImageInfo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoreSampleImage_InfoId",
                table: "CoreSampleImage",
                column: "InfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageInfo_FieldId",
                table: "ImageInfo",
                column: "FieldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoreSampleImage");

            migrationBuilder.DropTable(
                name: "ImageInfo");

            migrationBuilder.DropTable(
                name: "Field");
        }
    }
}
