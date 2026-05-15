using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services.Interfaces
{
    public interface IAppointmentService
    {
        public Task<Appointment> CreateAppointmentAsync(string userId, int doctorId, CreateAppointmentDto dto);

        public Task<IEnumerable<AppointmentDto>> GetUserAppointments(string userId);
        public Task<int?> GetDoctorIdForUser(string userId);

        public Task<AppointmentDto> GetAppointmentById(string userId, int appointmentId);
    }
}
