using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services
{
    public interface IAuthService
    {
        public Task RegisterPatient(RegisterPatientDto registerPatientDto);
        public Task RegisterDoctor(RegisterDoctorDto registerDoctorDto);
        public Task<string> Login(LoginDto loginDto);
    }
}
