using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services
{
    public interface IPatientService
    {
        public Task<List<PatientDto>> GetAllPatients();

        public Task<PatientDto> GetPatientById(int id);

    }
}
