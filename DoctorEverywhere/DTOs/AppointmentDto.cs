using DoctorEverywhere.Domain;
using DoctorEverywhere.Enums;

namespace DoctorEverywhere.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime StartingAt { get; set; }
        public AppointmentStatus StatusId { get; set; } 
        public DateTime RequestedAt { get; set; }
        public string DoctorName { get; set; }  = string.Empty;
        public string PatientName { get; set; } = string.Empty;

    }
}
