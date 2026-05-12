using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services
{
    public interface IAvailabilityService
    {
        public Task CreateorUpdateAvailability(string userId, List<AvailabilityDto> availabilityDtos);
        public Task<List<AvailabilityDto>> GetAvailability(string userId);
        public Task<List<string>> GetDoctorAvailability(int doctorId, DateTime date);
    }
}
