namespace DoctorEverywhere.Enums;

public enum AppointmentStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2, //from patient
    Rejected = 3, //from doctor
    Rescheduled = 4
}