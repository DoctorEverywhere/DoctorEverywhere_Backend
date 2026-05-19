namespace DoctorEverywhere.Messaging.DTOs
{
    public class AppointmentMessageDto
    {
        public Guid MessageId { get; set; } 

        public DateTime CreatedAt { get; set; } 

        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public DateTime StartingAt { get; set; }

    }
}
