using DoctorEverywhere.Domain;
using DoctorEverywhere.Enums;

namespace DoctorEverywhere.DTOs
{
    public class AvailabilityDto
    {
       // public int Id { get; set; }
        public DayOfWeekOption DayOfWeek { get; set; }
        public TimeSpan ShiftStartTime { get; set; }
        public TimeSpan ShiftEndTime { get; set; }

    }
}
