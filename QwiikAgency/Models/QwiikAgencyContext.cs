using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QwiikAgency.Models;

public partial class QwiikAgencyContext : DbContext
{
    public QwiikAgencyContext()
    {
    }

    public QwiikAgencyContext(DbContextOptions<QwiikAgencyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agency> Agencies { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Offday> Offdays { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agency>(entity =>
        {
            entity.HasKey(e => e.AgencyId).HasName("PK_Agency");

            entity.ToTable("AGENCY");

            entity.Property(e => e.AgencyId).HasColumnName("AGENCY_ID");
            entity.Property(e => e.AgencyAddress)
                .IsUnicode(false)
                .HasColumnName("AGENCY_ADDRESS");
            entity.Property(e => e.AgencyMaxAppointment).HasColumnName("AGENCY_MAX_APPOINTMENT");
            entity.Property(e => e.AgencyName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("AGENCY_NAME");
            entity.Property(e => e.AgencyPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AGENCY_PHONE");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK_Appointment");

            entity.ToTable("APPOINTMENT");

            entity.Property(e => e.AppointmentId).HasColumnName("APPOINTMENT_ID");
            entity.Property(e => e.AgencyId).HasColumnName("AGENCY_ID");
            entity.Property(e => e.AppointmentDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("APPOINTMENT_DATE");
            entity.Property(e => e.CustId).HasColumnName("CUST_ID");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustId);

            entity.ToTable("CUSTOMER");

            entity.Property(e => e.CustId).HasColumnName("CUST_ID");
            entity.Property(e => e.CustAddress)
                .IsUnicode(false)
                .HasColumnName("CUST_ADDRESS");
            entity.Property(e => e.CustName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CUST_NAME");
            entity.Property(e => e.CustPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_PHONE");
        });

        modelBuilder.Entity<Offday>(entity =>
        {
            entity.ToTable("OFFDAY");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AgencyId).HasColumnName("AGENCY_ID");
            entity.Property(e => e.OffDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("OFF_DATE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
