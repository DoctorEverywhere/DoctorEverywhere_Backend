namespace DoctorEverywhere.DTOs
{
    public class AnalyticsSummaryDto
    {
        public List<AppointmentStatusCountDto> AppointmentsByStatusCount { get; set; } = new();

        public List<SpecialtyDemandDto> DemandBySpecialtyCount { get; set; } = new();

        public List<DoctorReviewSummaryDto> ReviewsByRatingCount { get; set; } = new();
    }
}
