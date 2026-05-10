using DoctorEverywhere.Domain;
using DoctorEverywhere.Enums;

namespace DoctorEverywhere.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Specialty Specialty { get; set; }
        public OfficeDto? Office { get; set; }
    }
}
