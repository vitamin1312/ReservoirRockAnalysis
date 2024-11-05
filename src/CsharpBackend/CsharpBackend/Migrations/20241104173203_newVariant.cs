﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsharpBackend.Migrations
{
    /// <inheritdoc />
    public partial class newVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoreSampleImage_ImageInfo_InfoId",
                table: "CoreSampleImage");

            migrationBuilder.DropIndex(
                name: "IX_CoreSampleImage_InfoId",
                table: "CoreSampleImage");

            migrationBuilder.DropColumn(
                name: "InfoId",
                table: "CoreSampleImage");

            migrationBuilder.AddColumn<int>(
                name: "FieldId",
                table: "Field",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImageInfoId",
                table: "CoreSampleImage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CoreSampleImage_ImageInfoId",
                table: "CoreSampleImage",
                column: "ImageInfoId");

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

            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "ImageInfoId",
                table: "CoreSampleImage");

            migrationBuilder.AddColumn<int>(
                name: "InfoId",
                table: "CoreSampleImage",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoreSampleImage_InfoId",
                table: "CoreSampleImage",
                column: "InfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoreSampleImage_ImageInfo_InfoId",
                table: "CoreSampleImage",
                column: "InfoId",
                principalTable: "ImageInfo",
                principalColumn: "Id");
        }
    }
}
