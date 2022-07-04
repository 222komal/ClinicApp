using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ClinicApp.Models
{
    public partial class HospitalManagementSystemContext : DbContext
    {
        public HospitalManagementSystemContext()
        {
        }

        public HospitalManagementSystemContext(DbContextOptions<HospitalManagementSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AllUsers> AllUsers { get; set; }
        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<ClinicAdmin> ClinicAdmin { get; set; }
        public virtual DbSet<Doctor> Doctor { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-PTJSHAP;Initial Catalog=HospitalManagementSystem;Integrated Security=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllUsers>(entity =>
            {
                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Apid);

                entity.Property(e => e.Appointmentreason).IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Duration)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Duration2).HasColumnName("duration2");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.StartTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ClinicAdmin>(entity =>
            {
                entity.HasKey(e => e.AdminId);

                entity.Property(e => e.AdFname)
                    .HasColumnName("Ad-fname")
                    .HasMaxLength(50);

                entity.Property(e => e.AdLname)
                    .HasColumnName("Ad-lname")
                    .HasMaxLength(50);

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.PhNumber)
                    .HasColumnName("ph-number")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.ClinicAvailbility)
                    .HasColumnName("Clinic-Availbility")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.DtFname)
                    .HasColumnName("Dt-fname")
                    .HasMaxLength(50);

                entity.Property(e => e.DtLname)
                    .HasColumnName("Dt-lname")
                    .HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.PhNumber)
                    .HasColumnName("ph-number")
                    .HasMaxLength(50);

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Gender).HasMaxLength(20);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(20)
                    .IsFixedLength();

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
