using Bogus;
using DoctorEverywhere.Domain;
using DoctorEverywhere.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DoctorEverywhere
{
    public static class FakeDataSeeder
    {
        private class RealClinicLocation
        {
            public string Address { get; set; }
            public string PostalCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Check if database is already seeded
            if (context.Set<Doctor>().Any()) return;

            string doctorPassword = "Doctor1!";
            string patientPassword = "Patient1!";
            var doctors = new List<Doctor>();
            var patients = new List<Patient>();

            // ==========================================
            // 1. GENERATE DOCTORS & OFFICES
            // ==========================================
            var athensClinics = new List<RealClinicLocation>
            {
               new RealClinicLocation { Address = "Louizis Riankour 64",      PostalCode = "11523", Latitude = 37.993532, Longitude = 23.766987 }, // Right at Panormou Metro
               new RealClinicLocation { Address = "Panormou 20",              PostalCode = "11524", Latitude = 37.991667, Longitude = 23.764560 }, // Lower Panormou Street
               new RealClinicLocation { Address = "Karystou 10",              PostalCode = "11523", Latitude = 37.994812, Longitude = 23.765412 }, // Northern Panormou residential
               new RealClinicLocation { Address = "Trikalon 5",               PostalCode = "11524", Latitude = 37.992211, Longitude = 23.768520 }, // East of Panormou
               new RealClinicLocation { Address = "Doukissis Plakentias 45",  PostalCode = "11523", Latitude = 37.995921, Longitude = 23.763456 }, // Near Agios Dimitrios park
               new RealClinicLocation { Address = "Eslin 15",                 PostalCode = "11523", Latitude = 37.989451, Longitude = 23.762512 }, // Near President Hotel
               new RealClinicLocation { Address = "Vatopediou 22",            PostalCode = "11523", Latitude = 37.996512, Longitude = 23.761214 }, // Upper Ambelokipi
               new RealClinicLocation { Address = "Achaias 30",               PostalCode = "11524", Latitude = 37.991124, Longitude = 23.769821 }, // Towards Erythros Stavros
               new RealClinicLocation { Address = "Trifylias 12",             PostalCode = "11524", Latitude = 37.994112, Longitude = 23.770123 }, // Upper east side of Panormou
               new RealClinicLocation { Address = "Leoforos Kifisias 118",    PostalCode = "11526", Latitude = 37.989823, Longitude = 23.760124 }  // Kifisias/Panormou Intersection
            };

            for (int i = 1; i <= 10; i++) // Generating 10 Doctors
            {
                var user = new ApplicationUser
                {
                    UserName = $"doctor{i}"
                };

                await userManager.CreateAsync(user, doctorPassword);
                await userManager.AddToRoleAsync(user, "Doctor");

                var faker = new Faker();

                var realLocation = athensClinics[i - 1];
                var doctor = new Doctor
                {
                    ApplicationUserId = user.Id,
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    Specialty = (Specialty)faker.Random.Int(0, 3),
                    IsActive = true,
                    Office = new Office
                    {
                        Name = $"{faker.Name.LastName()} Clinic",
                        Address = realLocation.Address,
                        City = "Athens", // Forced Athens location
                        PostalCode = realLocation.PostalCode,
                        // Approximate coordinates for Athens
                        Latitude = realLocation.Latitude,
                        Longitude = realLocation.Longitude
                    },
                    WorkingHours = new List<WorkingSchedule>
                {
                    // Basic Mon-Fri 09:00 to 17:00 schedule
                    new WorkingSchedule { DayOfWeek = DayOfWeekOption.Monday, ShiftStartTime = new TimeSpan(9, 0, 0), ShiftEndTime = new TimeSpan(17, 0, 0) },
                    new WorkingSchedule { DayOfWeek = DayOfWeekOption.Tuesday, ShiftStartTime = new TimeSpan(9, 0, 0), ShiftEndTime = new TimeSpan(17, 0, 0) },
                    new WorkingSchedule { DayOfWeek = DayOfWeekOption.Wednesday, ShiftStartTime = new TimeSpan(9, 0, 0), ShiftEndTime = new TimeSpan(17, 0, 0) },
                    new WorkingSchedule { DayOfWeek = DayOfWeekOption.Thursday, ShiftStartTime = new TimeSpan(9, 0, 0), ShiftEndTime = new TimeSpan(17, 0, 0) },
                    new WorkingSchedule { DayOfWeek = DayOfWeekOption.Friday, ShiftStartTime = new TimeSpan(9, 0, 0), ShiftEndTime = new TimeSpan(17, 0, 0) }
                }
                };

                context.Set<Doctor>().Add(doctor);
                doctors.Add(doctor);
            }
            await context.SaveChangesAsync();

            // ==========================================
            // 2. GENERATE PATIENTS
            // ==========================================
            for (int i = 1; i <= 10; i++) // Generating 10 Patients
            {
                var user = new ApplicationUser
                {
                    UserName = $"patient{i}"
                };

                await userManager.CreateAsync(user, patientPassword);
                await userManager.AddToRoleAsync(user, "Patient");

                var faker = new Faker();
                var patient = new Patient
                {
                    ApplicationUserId = user.Id,
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    IsActive = true
                };

                context.Set<Patient>().Add(patient);
                patients.Add(patient);
            }
            await context.SaveChangesAsync();

            // ==========================================
            // 3. GENERATE APPOINTMENTS & REVIEWS
            // ==========================================
            var random = new Random();

            // Generate a few appointments per patient
            foreach (var patient in patients)
            {
                var randomDoctor = doctors[random.Next(doctors.Count)];

                // Random day in the next 14 days
                var appointmentDate = DateTime.UtcNow.Date.AddDays(random.Next(1, 15));

                // Random hour between 09:00 and 16:00 (to finish by 17:00)
                var hour = random.Next(9, 16);
                var startingAt = appointmentDate.AddHours(hour);

                var appointment = new Appointment
                {
                    PatientId = patient.Id,
                    DoctorId = randomDoctor.Id,
                    StartingAt = startingAt,
                    StatusId = (AppointmentStatus)random.Next(0, 4),
                    RequestedAt = DateTime.UtcNow.AddDays(-2)
                };
                context.Set<Appointment>().Add(appointment);

                var realisticReviews = new[]
                {
                    "Excellent doctor, very attentive and polite.",
                    "Great experience, explained everything to me in detail.",
                    "The clinic was spotless, though we had to wait a few extra minutes.",
                    "Very reassuring and professional. Highly recommended.",
                    "Great scientist, helped me immediately with my problem.",
                    "Friendly environment and excellent medical knowledge.",
                    "Listened carefully to all my symptoms. I felt very safe."
                };
                // Add ONE review for this doctor/patient combo to respect the unique index
                var review = new Review
                {
                    PatientId = patient.Id,
                    DoctorId = randomDoctor.Id,
                    Rating = random.Next(3, 6), // Ratings 3-5
                    Comments = new Faker().PickRandom(realisticReviews),
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                };
                context.Set<Review>().Add(review);
            }

            await context.SaveChangesAsync();
        }
    }
}
