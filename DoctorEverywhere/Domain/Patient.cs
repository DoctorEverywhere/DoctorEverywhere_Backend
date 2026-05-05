namespace DoctorEverywhere.Domain;

/// One Patient can book multiple appointments and write multiple reviews

public class Patient
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}