using DoctorEverywhere.Enums;

namespace DoctorEverywhere.Domain;

/// An appointment can be made by a patient to a specific doctor and will have a StartingAt and EndingAt date
/// EndingAt date could be removed if we make the one hour appointment rule a hard requirement
public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; }

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    public DateTime StartingAt { get; set; }
    public DateTime EndingAt { get; set; }

    public AppointmentStatus StatusId { get; set; } = AppointmentStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}