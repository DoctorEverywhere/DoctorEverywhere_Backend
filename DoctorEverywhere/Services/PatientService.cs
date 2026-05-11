using System.Diagnostics.Eventing.Reader;
using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Exceptions;
using Microsoft.EntityFrameworkCore;

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

        //public async Task<bool> CreatePatient()
       // {
        //    Patient patient = new Patient()
        //    {
              
        //    };

             
       // }
            

    }
}
