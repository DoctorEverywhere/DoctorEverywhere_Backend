using System.ComponentModel.DataAnnotations;

namespace DoctorEverywhere.Domain;

public class Review
{
    public int Id { get; set; }

    [Required] public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    [Required] public int PatientId { get; set; }
    public Patient Patient { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string Comments { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}