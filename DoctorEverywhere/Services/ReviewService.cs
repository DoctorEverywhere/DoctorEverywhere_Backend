using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorEverywhere.Services
{
    public class ReviewService : IReviewService
    {
        private ApplicationDbContext _context;
        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateReview(string userId, int doctorId, [FromBody] CreateReviewDto reviewDto)
        {
        
            var patientId = await _context.Patients
                .Where(p => p.ApplicationUserId == userId)
                .Select(p => p.Id) // Only fetch the Id column for performance
                .FirstOrDefaultAsync();

            if (patientId == 0)
            {
                throw new EntityNotFoundException($"Patient with user ID {userId} not found.");
            }

            var newReview = new Review
            {
                DoctorId = doctorId,
                PatientId = patientId,
                Rating = reviewDto.Rating,
                Comments = reviewDto.Comments
            };

            await _context.Reviews.AddAsync(newReview);
            await _context.SaveChangesAsync();

        }

        public async Task<List<ReviewDto>> GetReviewsByDoctorId(int doctorId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.DoctorId == doctorId)
                .IgnoreQueryFilters()
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    PatientFirstName = r.Patient.FirstName,
                    PatientLastName = r.Patient.LastName,
                    Rating = r.Rating,
                    Comments = r.Comments,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return reviews;
        }
    }
}
