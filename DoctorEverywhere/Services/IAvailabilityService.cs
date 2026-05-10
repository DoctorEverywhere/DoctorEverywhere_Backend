using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services
{
    public interface IAvailabilityService
    {
        public Task CreateAvailability(string userId, List<AvailabilityDto> availabilityDtos);
    }
}
