using Microsoft.AspNetCore.Identity;
using DoctorEverywhere.Domain;

public class ApplicationUser : IdentityUser
{
    public Doctor DoctorProfile { get; set; }
    public Patient PatientProfile { get; set; }
    public Manager ManagerProfile { get; set; }
}