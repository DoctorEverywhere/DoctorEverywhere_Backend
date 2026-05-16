using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services.Interfaces
{
    public interface IDoctorService
    {
        public Task<DoctorDto?> GetDoctorById(int id);
        public Task<List<DoctorDto?>> GetDoctorBySpecialty(int? specialty);
        public Task<DoctorDto?> GetMyProfile(string userId);
    }
}
