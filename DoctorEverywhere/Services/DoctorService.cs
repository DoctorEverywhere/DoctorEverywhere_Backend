using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Mappings;
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
    }
}
