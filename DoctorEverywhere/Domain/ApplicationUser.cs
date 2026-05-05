using Microsoft.AspNetCore.Identity;
using DoctorEverywhere.Domain;

/// Identity User is a build in class for .net applications to create Identity table for roles
public class ApplicationUser : IdentityUser
{
    public Doctor DoctorProfile { get; set; }
    public Patient PatientProfile { get; set; }
    public Manager ManagerProfile { get; set; }
}