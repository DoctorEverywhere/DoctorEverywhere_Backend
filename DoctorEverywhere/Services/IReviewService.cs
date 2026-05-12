using DoctorEverywhere.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DoctorEverywhere.Services
{
    public interface IReviewService
    {
        Task CreateReview(string userId, int doctorId, [FromBody] CreateReviewDto reviewDto);
        Task<List<ReviewDto>> GetReviewsByDoctorId(int doctorId);
    }
}
