namespace DoctorEverywhere.Domain;

/// Office - doctor connection 1-1

public class Office
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }

    // Geo location
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
}