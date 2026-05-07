using Microsoft.AspNetCore.Mvc.ViewEngines;
using DoctorEverywhere.Enums;

namespace DoctorEverywhere.Domain;

/// Can work in one office (one doctor in one office 1-1)
public class Doctor
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Specialty Specialty { get; set; }
    public Office Office { get; set; }

    public ICollection<WorkingSchedule> WorkingHours { get; set; } = new List<WorkingSchedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}