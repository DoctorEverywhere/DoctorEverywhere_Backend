using DoctorEverywhere.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DoctorEverywhere.Services.Interfaces
{
    public interface IAnalyticsService
    {
        public  Task<AnalyticsSummaryDto> GetAnalyticsSummary();
       // public Task<List<AppointmentStatusCountDto>> GetAppointmentStatusStats();

        //public Task<List<SpecialtyDemandDto>> GetDemandBySpecialty();
    }
}
