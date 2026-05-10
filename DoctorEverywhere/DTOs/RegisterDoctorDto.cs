using DoctorEverywhere.Enums;

namespace DoctorEverywhere.DTOs
{
    public class RegisterDoctorDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Specialty Specialty { get; set; }
        public string OfficeName { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficeCity { get; set; }
        public string OfficePostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
