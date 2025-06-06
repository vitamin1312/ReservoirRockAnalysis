﻿// <auto-generated />
using System;
using CsharpBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CsharpBackend.Migrations
{
    [DbContext(typeof(CsharpBackendContext))]
    [Migration("20250526234427_new-image-info")]
    partial class newimageinfo
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CsharpBackend.Models.CoreSampleImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ImageInfoId")
                        .HasColumnType("int");

                    b.Property<string>("PathToImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PathToMask")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ImageInfoId");

                    b.ToTable("CoreSampleImage");
                });

            modelBuilder.Entity("CsharpBackend.Models.Field", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Field");
                });

            modelBuilder.Entity("CsharpBackend.Models.ImageInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly?>("CreationDate")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("FieldId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("pixelLengthRatio")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("FieldId");

                    b.ToTable("ImageInfo");
                });

            modelBuilder.Entity("CsharpBackend.Models.PoreInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double?>("Area")
                        .HasColumnType("float");

                    b.Property<double?>("AspectRatio")
                        .HasColumnType("float");

                    b.Property<double>("CentroidX")
                        .HasColumnType("float");

                    b.Property<double>("CentroidY")
                        .HasColumnType("float");

                    b.Property<double?>("Circularity")
                        .HasColumnType("float");

                    b.Property<double?>("ConvexArea")
                        .HasColumnType("float");

                    b.Property<int?>("CoreSampleImageId")
                        .HasColumnType("int");

                    b.Property<double?>("FeretDiameter")
                        .HasColumnType("float");

                    b.Property<int?>("Index")
                        .HasColumnType("int");

                    b.Property<double?>("Orientation")
                        .HasColumnType("float");

                    b.Property<double?>("Perimeter")
                        .HasColumnType("float");

                    b.Property<string>("PorosityName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("pixelLengthRatio")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CoreSampleImageId");

                    b.ToTable("PoreInfo");
                });

            modelBuilder.Entity("CsharpBackend.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("CsharpBackend.Models.CoreSampleImage", b =>
                {
                    b.HasOne("CsharpBackend.Models.ImageInfo", "ImageInfo")
                        .WithMany()
                        .HasForeignKey("ImageInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ImageInfo");
                });

            modelBuilder.Entity("CsharpBackend.Models.ImageInfo", b =>
                {
                    b.HasOne("CsharpBackend.Models.Field", "Field")
                        .WithMany("FieldImages")
                        .HasForeignKey("FieldId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Field");
                });

            modelBuilder.Entity("CsharpBackend.Models.PoreInfo", b =>
                {
                    b.HasOne("CsharpBackend.Models.CoreSampleImage", "coreSampleImage")
                        .WithMany("PoresInfo")
                        .HasForeignKey("CoreSampleImageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("coreSampleImage");
                });

            modelBuilder.Entity("CsharpBackend.Models.CoreSampleImage", b =>
                {
                    b.Navigation("PoresInfo");
                });

            modelBuilder.Entity("CsharpBackend.Models.Field", b =>
                {
                    b.Navigation("FieldImages");
                });
#pragma warning restore 612, 618
        }
    }
}
