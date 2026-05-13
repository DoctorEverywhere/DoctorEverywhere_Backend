using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services
{
    public interface IAppointmentService
    {
        public Task CreateAppointmentAsync(string userId, int doctorId, CreateAppointmentDto dto);

        //public Task<List<AppointmentDto>> GetAppointmentsById(string userId);

        public Task<AppointmentDto> GetAppointmentByIdAsync(string userId, int appointmentId);
    }
}
