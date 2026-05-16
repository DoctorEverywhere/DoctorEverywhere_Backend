using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Numerics;
using System.Security.Claims;

namespace DoctorEverywhere.Services
{
    public class PatientService : IPatientService
    {
        private ApplicationDbContext _context;

        public PatientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PatientDto>> GetAllPatients()
        {
            return await _context.Patients
                .Select(p => new PatientDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,

                }).ToListAsync();
        }

        public async Task<PatientDto> GetPatientById(int id)
        {
              Patient? patient = await _context.Patients
                .SingleOrDefaultAsync(p => p.Id == id);
               
              if(patient == null)
               {
                throw new EntityNotFoundException($"Patient with ID {id} not found.");
                }

               return new PatientDto
               {

                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,

               };

        }

        public async Task<PatientDto> GetPatientByUserId(string userId)
        {
            Patient? patient = await _context.Patients
              .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

            if (patient == null)
            {
                throw new EntityNotFoundException($"Patient with ID {userId} not found.");
            }

            return new PatientDto
            {

                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,

            };

        }

        public async Task DeletePatientAsync(string userId)
        {
            var patient = await _context.Patients
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.ApplicationUserId == userId);

            if (patient == null)
            {
                throw new EntityNotFoundException($"Patient with User ID {userId} not found.");
            }

            patient.FirstName = "Deleted";
            patient.LastName = "Patient";
            patient.IsActive = false;

            var randomGuid = Guid.NewGuid().ToString();
            patient.ApplicationUser.UserName = $"deleted_patient_{randomGuid}";
            patient.ApplicationUser.NormalizedUserName = $"DELETED_PATIENT_{randomGuid}".ToUpper();
            patient.ApplicationUser.PasswordHash = null;

            var futureAppointments = await _context.Appointments
            .Where(a => a.PatientId == patient.Id && a.StartingAt > DateTime.UtcNow)
            .ToListAsync();

            foreach (var appointment in futureAppointments)
            {
                appointment.StatusId = AppointmentStatus.Cancelled;
            }

            await _context.SaveChangesAsync();
        }
    }
}
