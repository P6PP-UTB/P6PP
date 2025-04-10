﻿// <auto-generated />
using System;
using BookingService.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookingService.API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20250324212952_Refactor")]
    partial class Refactor
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("BookingService.API.Entities.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("CheckInDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("CheckOutDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Bookings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BookingDate = new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1264),
                            CheckInDate = new DateTime(2025, 3, 25, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1268),
                            CheckOutDate = new DateTime(2025, 3, 26, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1272),
                            Price = 150,
                            ServiceId = 1,
                            Status = 0,
                            UserId = 1
                        },
                        new
                        {
                            Id = 2,
                            BookingDate = new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1276),
                            CheckInDate = new DateTime(2025, 3, 27, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1278),
                            CheckOutDate = new DateTime(2025, 3, 28, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1280),
                            Price = 250,
                            ServiceId = 2,
                            Status = 1,
                            UserId = 2
                        });
                });

            modelBuilder.Entity("BookingService.API.Entities.Discount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsValid")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Percentage")
                        .HasColumnType("int");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ValidTo")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Discount");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsValid = true,
                            Percentage = 10,
                            ValidFrom = new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(723),
                            ValidTo = new DateTime(2025, 4, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(776)
                        },
                        new
                        {
                            Id = 2,
                            IsValid = true,
                            Percentage = 20,
                            ValidFrom = new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(783),
                            ValidTo = new DateTime(2025, 4, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(785)
                        });
                });

            modelBuilder.Entity("BookingService.API.Entities.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Room");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Capacity = 20,
                            Name = "Room A",
                            Status = 0
                        },
                        new
                        {
                            Id = 2,
                            Capacity = 15,
                            Name = "Room B",
                            Status = 1
                        },
                        new
                        {
                            Id = 3,
                            Capacity = 10,
                            Name = "Room C",
                            Status = 2
                        },
                        new
                        {
                            Id = 4,
                            Capacity = 25,
                            Name = "Room D",
                            Status = 3
                        });
                });

            modelBuilder.Entity("BookingService.API.Entities.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsCancelled")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<string>("ServiceName")
                        .HasColumnType("longtext");

                    b.Property<int>("TrainerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Service");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsCancelled = false,
                            Price = 100,
                            ServiceName = "Service A",
                            TrainerId = 1
                        },
                        new
                        {
                            Id = 2,
                            IsCancelled = false,
                            Price = 200,
                            ServiceName = "Service B",
                            TrainerId = 2
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
