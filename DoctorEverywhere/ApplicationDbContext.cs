using DoctorEverywhere.Domain;
using DoctorEverywhere.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoctorEverywhere;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Office> Offices { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<WorkingSchedule> WorkingSchedules { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        string patientRoleId = "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d";
        string doctorRoleId = "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e";
        string managerRoleId = "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f";

        // 2. Seed the Roles
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = patientRoleId,
                Name = "Patient",
                NormalizedName = "PATIENT", 
                ConcurrencyStamp = "1" 
            },
            new IdentityRole
            {
                Id = doctorRoleId,
                Name = "Doctor",
                NormalizedName = "DOCTOR",
                ConcurrencyStamp = "2" 
            },
            new IdentityRole
            {
                Id = managerRoleId,
                Name = "Manager",
                NormalizedName = "MANAGER",
                ConcurrencyStamp = "3" 
            }
        );

        builder.Entity<Doctor>()
            .HasOne(d => d.ApplicationUser)
            .WithOne(u => u.DoctorProfile)
            .HasForeignKey<Doctor>(d => d.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Patient>()
            .HasOne(p => p.ApplicationUser)
            .WithOne(u => u.PatientProfile)
            .HasForeignKey<Patient>(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Manager>()
            .HasOne(m => m.ApplicationUser)
            .WithOne(u => u.ManagerProfile)
            .HasForeignKey<Manager>(m => m.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Doctor>()
            .HasOne(d => d.Office)
            .WithOne(o => o.Doctor)
            .HasForeignKey<Office>(o => o.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<WorkingSchedule>()
            .HasOne(ws => ws.Doctor)
            .WithMany(d => d.WorkingHours)
            .HasForeignKey(ws => ws.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasOne(r => r.Patient)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasOne(r => r.Doctor)
            .WithMany(d => d.Reviews)
            .HasForeignKey(r => r.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasIndex(r => new { r.PatientId, r.DoctorId })
            .IsUnique();

        builder.Entity<Message>()
            .HasOne(m => m.Appointment)
            .WithMany(a => a.Messages)
            .HasForeignKey(m => m.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
            .Property(m => m.SenderUserId)
            .IsRequired();

        builder.Entity<Message>()
            .Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Entity<Appointment>()
            .Property(a => a.StatusId)
            .HasConversion<int>();

        builder.Entity<Doctor>()
            .Property(d => d.Specialty)
            .HasConversion<int>();

        builder.Entity<WorkingSchedule>()
            .Property(ws => ws.DayOfWeek)
            .HasConversion<int>();

        builder.Entity<Review>()
            .Property(r => r.Comments)
            .HasMaxLength(1000);

        builder.Entity<Patient>().HasQueryFilter(p => p.IsActive);
        builder.Entity<Doctor>().HasQueryFilter(d => d.IsActive);
    }
}