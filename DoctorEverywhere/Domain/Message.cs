namespace DoctorEverywhere.Domain
{
    public class Message
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public string SenderUserId { get; set; } //applicationUserId of the sender
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
