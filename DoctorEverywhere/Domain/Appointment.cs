using DoctorEverywhere.Enums;

namespace DoctorEverywhere.Domain;

public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; }

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    public DateTime StartingAt { get; set; }

    public AppointmentStatus StatusId { get; set; } = AppointmentStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}