namespace DoctorEverywhere.Domain;

/// Manager minimal fields, insert will be made by us once
public class Manager
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
}