using System.ComponentModel.DataAnnotations;

namespace DoctorEverywhere.DTOs
{
    public class ReviewDto
    {
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime CreatedAt { get; set; }
    }
}
