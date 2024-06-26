﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QwiikAgency.Models;

#nullable disable

namespace QwiikAgency.Migrations
{
    [DbContext(typeof(QwiikAgencyContext))]
    partial class QwiikAgencyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QwiikAgency.Models.Agency", b =>
                {
                    b.Property<int>("AgencyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("AGENCY_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AgencyId"));

                    b.Property<string>("AgencyAddress")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("AGENCY_ADDRESS");

                    b.Property<int>("AgencyMaxAppointment")
                        .HasColumnType("int")
                        .HasColumnName("AGENCY_MAX_APPOINTMENT");

                    b.Property<string>("AgencyName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("AGENCY_NAME");

                    b.Property<string>("AgencyPhone")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("AGENCY_PHONE");

                    b.HasKey("AgencyId")
                        .HasName("PK_Agency");

                    b.ToTable("AGENCY", (string)null);
                });

            modelBuilder.Entity("QwiikAgency.Models.Appointment", b =>
                {
                    b.Property<int>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("APPOINTMENT_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointmentId"));

                    b.Property<int>("AgencyId")
                        .HasColumnType("int")
                        .HasColumnName("AGENCY_ID");

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("smalldatetime")
                        .HasColumnName("APPOINTMENT_DATE");

                    b.Property<int>("CustId")
                        .HasColumnType("int")
                        .HasColumnName("CUST_ID");

                    b.HasKey("AppointmentId")
                        .HasName("PK_Appointment");

                    b.ToTable("APPOINTMENT", (string)null);
                });

            modelBuilder.Entity("QwiikAgency.Models.Customer", b =>
                {
                    b.Property<int>("CustId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CUST_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustId"));

                    b.Property<string>("CustAddress")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("CUST_ADDRESS");

                    b.Property<string>("CustName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("CUST_NAME");

                    b.Property<string>("CustPhone")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("CUST_PHONE");

                    b.HasKey("CustId");

                    b.ToTable("CUSTOMER", (string)null);
                });

            modelBuilder.Entity("QwiikAgency.Models.Offday", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<int>("AgencyId")
                        .HasColumnType("int")
                        .HasColumnName("AGENCY_ID");

                    b.Property<DateTime>("OffDate")
                        .HasColumnType("smalldatetime")
                        .HasColumnName("OFF_DATE");

                    b.HasKey("Id");

                    b.ToTable("OFFDAY", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
