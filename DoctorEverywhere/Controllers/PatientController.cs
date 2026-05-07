using DoctorEverywhere.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctorEverywhere.Controllers
{
    [Route("api/[controller]")] //api/patient
    [ApiController]
    public class PatientController : ControllerBase
    {
        private IPatientService patientService;

        public PatientController(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        //[HttpGet]

        //public async Task<List<PatientDto>>
    }
}
