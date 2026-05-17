using DoctorEverywhere.Enums;

namespace DoctorEverywhere.DTOs
{
    public class DoctorReviewSummaryDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }  
        public Specialty Specialty { get; set; }
        public int ReviewCount { get; set; }
        public double? AverageRating { get; set; }
    }
}
