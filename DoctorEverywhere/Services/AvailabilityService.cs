using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services.Interfaces;
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

        public async Task CreateorUpdateAvailability(string userId, List<AvailabilityDto> availabilityDtos)
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

            //delete by doctorId
            var existingSchedules = await _context.WorkingSchedules
                .Where(ws => ws.DoctorId == doctorId)
                .ToListAsync();

            _context.WorkingSchedules.RemoveRange(existingSchedules);

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

        public async Task<List<AvailabilityDto>> GetAvailability(string userId)
        {
            var doctorId = await _context.Doctors
                .Where(d => d.ApplicationUserId == userId)
                .Select(d => d.Id) // Only fetch the Id column for performance
                .FirstOrDefaultAsync();
            if (doctorId == 0)
            {
                throw new EntityNotFoundException($"Doctor with user ID {userId} not found.");
            }
            var schedules = await _context.WorkingSchedules
                .Where(ws => ws.DoctorId == doctorId)
                .ToListAsync();
            return schedules.Select(ws => new AvailabilityDto
            {
                DayOfWeek = ws.DayOfWeek,
                ShiftStartTime = ws.ShiftStartTime,
                ShiftEndTime = ws.ShiftEndTime
            }).ToList();
        }

        public async Task<List<string>> GetDoctorAvailability(int doctorId, DateTime date)
        {
            var dayOfWeek = (DayOfWeekOption)date.DayOfWeek;

            var schedule = await _context.WorkingSchedules
                .FirstOrDefaultAsync(w => w.DoctorId == doctorId && w.DayOfWeek == dayOfWeek);

            if (schedule == null)
            {
                return new List<string>();
            }

            var bookedTimeSlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                         && a.StartingAt.Date == date.Date
                         && (a.StatusId == AppointmentStatus.Pending || a.StatusId == AppointmentStatus.Confirmed))
                .Select(a => a.StartingAt.TimeOfDay)
                .ToListAsync();

            var availableSlots = new List<string>();
            TimeSpan currentSlot = schedule.ShiftStartTime;
            TimeSpan oneHour = TimeSpan.FromHours(1);

            while (currentSlot.Add(oneHour) <= schedule.ShiftEndTime)
            {
                if (!bookedTimeSlots.Contains(currentSlot))
                {
                    availableSlots.Add(currentSlot.ToString(@"hh\:mm"));
                }
                currentSlot = currentSlot.Add(oneHour);
            }

            return availableSlots;
        }
    }
}
