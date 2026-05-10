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

        /* // Seed data for testing
         // 1. Define static IDs
         string doctorRoleId = "c85eee14-6340-4840-95c2-db12554843e6";
         string userId = "d95eee14-6340-4840-95c2-db12554843e5";
         int doctorId = 1;
         int officeId = 1;

         // 2. Seed the Doctor Role
         builder.Entity<IdentityRole>().HasData(new IdentityRole
         {
             Id = doctorRoleId,
             Name = "Doctor",
             NormalizedName = "DOCTOR"
         });

         // 3. Seed the Identity User
         var doctorUser = new ApplicationUser
         {
             Id = userId,
             UserName = "smith",
             NormalizedUserName = "DR.SMITH@EXAMPLE.COM",
             Email = "dr.smith@example.com",
             NormalizedEmail = "DR.SMITH@EXAMPLE.COM",
             EmailConfirmed = true,
             SecurityStamp = "3b08e2f0-f9cb-4c2d-8e68-04313b2ce24e",
             PasswordHash = "AQAAAAIAAYagAAAAEOc1wX1vFkK+9G9I/G+yR7Vv1sZ0xYk2f7m1vU8xR1y7E3b9a7Z2w3e4r5t6y7u8iQ=="
         };

      *//*   var passwordHasher = new PasswordHasher<ApplicationUser>();
         doctorUser.PasswordHash = passwordHasher.HashPassword(doctorUser, "SecureDoctor123!");*//*

         builder.Entity<ApplicationUser>().HasData(doctorUser);

         // 4. Assign the User to the Doctor Role
         builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
         {
             RoleId = doctorRoleId,
             UserId = userId
         });

         // 5. Seed the Doctor Entity (Link to Identity User via ApplicationUserId)
         builder.Entity<Doctor>().HasData(new Doctor
         {
             Id = doctorId,
             ApplicationUserId = userId, // Foreign Key linking to ApplicationUser
             FirstName = "John",
             LastName = "Smith",
             Specialty = (Specialty)1 // Replace with actual Enum value, e.g., Specialty.Pediatrics

             // CRITICAL: Do NOT initialize Lists or Navigation properties here. 
             // e.g., NO 'Office = new Office()' or 'WorkingHours = ...'
         });

         // 6. Seed the Office Entity (Link to Doctor via DoctorId)
         builder.Entity<Office>().HasData(new Office
         {
             Id = officeId,
             DoctorId = doctorId, // Foreign Key linking to the Doctor
             Name = "Smith Medical Associates",
             Address = "123 Healthway Blvd, Suite 100",
             City = "Seattle",
             PostalCode = "98101",
             Latitude = 47.6062,
             Longitude = -122.3321
         });
     */
    }
}