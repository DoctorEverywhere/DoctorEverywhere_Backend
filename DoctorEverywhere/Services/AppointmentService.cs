using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoctorEverywhere.Services
{
    public class AppointmentService : IAppointmentService
    {
        private ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAppointmentAsync(string userId,int doctorId,CreateAppointmentDto dto)
        {
            var patientId = await _context.Patients
                .Where(p => p.ApplicationUserId == userId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            if (patientId == 0)
            {
                throw new EntityNotFoundException("Patient not found");
            }

            var newAppointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                StartingAt = dto.StartingAt
            };

            await _context.Appointments.AddAsync(newAppointment);
            await _context.SaveChangesAsync();
        }

        //no safeguard for appointment Id,kapoios mporei na balei appointment id 1 kai bgazei OK
        
        public async Task<IEnumerable<AppointmentDto>> GetUserAppointments(string userId)
        {
            var appointments = await _context.Appointments
                .Include(a=> a.Doctor)
                .Include(a=> a.Patient)
                .Where(a => 
                (a.Patient.ApplicationUserId == userId) || (a.Doctor.ApplicationUserId == userId))
                .OrderByDescending(a => a.StartingAt)
                .Select(appointment => new AppointmentDto
                {
                    Id = appointment.Id,
                    DoctorId = appointment.DoctorId,
                    PatientId = appointment.PatientId,
                    StartingAt = appointment.StartingAt,
                    StatusId = appointment.StatusId,
                    RequestedAt = appointment.RequestedAt,
                    DoctorName = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
                    PatientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}"
                })
                .ToListAsync();
            return appointments;
        }
        

        public async Task<AppointmentDto> GetAppointmentById(string userId, int appointmentId)
        {
            var appointment = await _context.Appointments
                .Where(a => a.Id == appointmentId &&
                            ((a.Patient.ApplicationUserId == userId) || (a.Doctor.ApplicationUserId == userId)))
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    PatientId = a.PatientId,
                    StartingAt = a.StartingAt,
                    StatusId = a.StatusId,
                    RequestedAt = a.RequestedAt,
                    DoctorName = $"{a.Doctor.FirstName} {a.Doctor.LastName}",
                    PatientName = $"{a.Patient.FirstName} {a.Patient.LastName}"
                })
                .FirstOrDefaultAsync();

            if (appointment is null)
            {
                throw new EntityNotFoundException("Appointment not found");
            }

            return appointment;
        }
    }
}
