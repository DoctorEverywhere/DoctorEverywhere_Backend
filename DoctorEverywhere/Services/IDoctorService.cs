using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services
{
    public interface IDoctorService
    {
        public Task<DoctorDto?> GetDoctorById(int id);
        public Task<List<DoctorDto?>> GetDoctorBySpecialty(int? specialty);
    }
}
