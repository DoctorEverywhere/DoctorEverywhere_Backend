using DoctorEverywhere.Enums;

namespace DoctorEverywhere.Domain;

public class WorkingSchedule
{
    public int Id { get; set; }

    public DayOfWeekOption DayOfWeek { get; set; }

    public TimeSpan ShiftStartTime { get; set; }
    public TimeSpan ShiftEndTime { get; set; }

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
}