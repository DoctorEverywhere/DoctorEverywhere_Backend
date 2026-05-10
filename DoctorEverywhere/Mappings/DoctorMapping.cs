using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;

namespace DoctorEverywhere.Mappings
{
    public static class DoctorMapping
    {
        public static DoctorDto? ToDoctorDto(this Doctor doctor)
        {
            if (doctor == null) return null;
            return new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialty = doctor.Specialty,
                Office = doctor.Office.ToOfficeDto()
            };
        }

        public static OfficeDto? ToOfficeDto(this Office office)
        {
            if (office == null) return null;
            return new OfficeDto
            {
                Id = office.Id,
                Name = office.Name,
                Address = office.Address,
                City = office.City,
                PostalCode = office.PostalCode,
                Latitude = office.Latitude,
                Longitude = office.Longitude
            };
        }
    }
}
