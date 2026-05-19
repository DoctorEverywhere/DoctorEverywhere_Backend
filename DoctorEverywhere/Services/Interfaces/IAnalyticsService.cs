using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Services.Interfaces
{
    public interface IAnalyticsService
    {
        public  Task<AnalyticsSummaryDto> GetAnalyticsSummary();
    }
}
