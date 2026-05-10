using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DoctorEverywhere.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private ApplicationDbContext _context;
        public AvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAvailability(string userId, List<AvailabilityDto> availabilityDtos)
        {
            // 1. Find the Doctor's internal integer Id
            var doctorId = await _context.Doctors
                .Where(d => d.ApplicationUserId == userId)
                .Select(d => d.Id) // Only fetch the Id column for performance
                .FirstOrDefaultAsync();

            if (doctorId == 0)
            {
                throw new EntityNotFoundException($"Doctor with user ID {userId} not found.");
            }

            // 2. Create the new schedule using the resolved DoctorId
            var newSchedules = availabilityDtos.Select(dto => new WorkingSchedule
            {
                DoctorId = doctorId,
                DayOfWeek = dto.DayOfWeek,
                ShiftStartTime = dto.ShiftStartTime,
                ShiftEndTime = dto.ShiftEndTime
            }).ToList();

            // 3. Add the entire list to the DbContext at once
            await _context.WorkingSchedules.AddRangeAsync(newSchedules);
            await _context.SaveChangesAsync();
        }
    }
}
