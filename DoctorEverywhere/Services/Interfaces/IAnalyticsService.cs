using DoctorEverywhere.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DoctorEverywhere.Services.Interfaces
{
    public interface IAnalyticsService
    {
        public Task<List<AppointmentStatusCountDto>> GetAppointmentStatusStats();
    }
}
