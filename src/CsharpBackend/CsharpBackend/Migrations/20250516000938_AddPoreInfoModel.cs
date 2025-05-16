using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPoreInfoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoreInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PorosityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: true),
                    Area = table.Column<long>(type: "bigint", nullable: true),
                    Perimeter = table.Column<long>(type: "bigint", nullable: true),
                    Circularity = table.Column<double>(type: "float", nullable: true),
                    AspectRatio = table.Column<double>(type: "float", nullable: true),
                    FeretDiameter = table.Column<double>(type: "float", nullable: true),
                    Orientation = table.Column<double>(type: "float", nullable: true),
                    CentroidX = table.Column<int>(type: "int", nullable: false),
                    CentroidY = table.Column<int>(type: "int", nullable: false),
                    ConvexArea = table.Column<double>(type: "float", nullable: true),
                    CoreSampleImageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoreInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoreInfo_CoreSampleImage_CoreSampleImageId",
                        column: x => x.CoreSampleImageId,
                        principalTable: "CoreSampleImage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoreInfo_CoreSampleImageId",
                table: "PoreInfo",
                column: "CoreSampleImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoreInfo");
        }
    }
}
