using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Mappings;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoctorEverywhere.Services
{
    public class DoctorService : IDoctorService
    {
        private ApplicationDbContext _context;
        public DoctorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DoctorDto?> GetDoctorById(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Office)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
            {
                throw new EntityNotFoundException($"Doctor with ID {id} not found.");
            }

            return doctor.ToDoctorDto();
/*
            return new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialty = doctor.Specialty,
                Office = new OfficeDto
                {
                    Id = doctor.Office.Id,
                    Name = doctor.Office.Name,
                    Address = doctor.Office.Address,
                    City = doctor.Office.City,
                    PostalCode = doctor.Office.PostalCode,
                    Latitude = doctor.Office.Latitude,
                    Longitude = doctor.Office.Longitude
                }
            };*/
        }

        public async Task<List<DoctorDto?>> GetDoctorBySpecialty(int? specialty)
        {
            if (specialty == null)
            {
                return new List<DoctorDto?>();
            }

            var doctors = await _context.Doctors
                .Include(d => d.Office)
                .Where(d => d.Specialty == (Specialty)specialty)
                .ToListAsync();

            return doctors.Select(d => d.ToDoctorDto()).ToList();
        }

        public async Task<DoctorDto?> GetMyProfile(string userId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Office)
                .FirstOrDefaultAsync(d => d.ApplicationUserId == userId);

            if (doctor == null)
            {
                throw new EntityNotFoundException($"Doctor with User ID {userId} not found.");
            }

            return doctor.ToDoctorDto();
        }

        public async Task DeleteDoctorAsync(string userId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Office)
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.ApplicationUserId == userId);

            if (doctor == null)
            {
                throw new EntityNotFoundException($"Doctor with User ID {userId} not found.");
            }

            doctor.FirstName = "Deleted";
            doctor.LastName ="Doctor";
            doctor.Specialty = (Specialty)10;
            doctor.IsActive = false;

            var randomGuid = Guid.NewGuid().ToString();
            doctor.ApplicationUser.UserName = $"deleted_doctor_{randomGuid}";
            doctor.ApplicationUser.NormalizedUserName = $"DELETED_DOCTOR_{randomGuid}".ToUpper();
            doctor.ApplicationUser.PasswordHash = null;

            if(doctor.Office != null)
            {
                doctor.Office.Name = $"Deleted Office {randomGuid}";
                doctor.Office.Address = "Deleted Address";
                doctor.Office.City = "Deleted City";
                doctor.Office.PostalCode = "00000";
                doctor.Office.Latitude = 0;
                doctor.Office.Longitude = 0;
            }

            var futureAppointments = await _context.Appointments
            .Where(a => a.DoctorId == doctor.Id && a.StartingAt > DateTime.UtcNow)
            .ToListAsync();

            foreach (var appointment in futureAppointments)
            {
                appointment.StatusId = AppointmentStatus.Rejected; 
            }

            await _context.SaveChangesAsync();
        }
    }
}
