using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services
{
    public interface IAvailabilityService
    {
        public Task CreateorUpdateAvailability(string userId, List<AvailabilityDto> availabilityDtos);
    }
}
