namespace DoctorEverywhere.DTOs
{
    public class AnalyticsSummaryDto
    {
        public List<AppointmentStatusCountDto> AppointmentsByStatusCount { get; set; }

        public List<SpecialtyDemandDto> DemandBySpecialtyCount { get; set; } = new();
    }
}
