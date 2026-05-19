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
            var doctorId = await _context.Doctors
                .Where(d => d.ApplicationUserId == userId)
                .Select(d => d.Id) 
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

            //create the new schedule using the resolved DoctorId
            var newSchedules = availabilityDtos.Select(dto => new WorkingSchedule
            {
                DoctorId = doctorId,
                DayOfWeek = dto.DayOfWeek,
                ShiftStartTime = dto.ShiftStartTime,
                ShiftEndTime = dto.ShiftEndTime
            }).ToList();

            await _context.WorkingSchedules.AddRangeAsync(newSchedules);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AvailabilityDto>> GetAvailability(string userId)
        {
            var doctorId = await _context.Doctors
                .Where(d => d.ApplicationUserId == userId)
                .Select(d => d.Id)
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
            int sysDay = (int)date.DayOfWeek;
            int customDay = sysDay == 0 ? 6 : sysDay - 1; 
            var dayOfWeek = (DayOfWeekOption)customDay;

            var schedules = await _context.WorkingSchedules
                .Where(w => w.DoctorId == doctorId && w.DayOfWeek == dayOfWeek)
                .ToListAsync();

            if (schedules == null || !schedules.Any())
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
            
            TimeSpan oneHour = TimeSpan.FromHours(1);
            foreach (var schedule in schedules)
            {
                TimeSpan currentSlot = schedule.ShiftStartTime;

                while (currentSlot.Add(oneHour) <= schedule.ShiftEndTime)
                {
                    if (!bookedTimeSlots.Contains(currentSlot))
                    {
                        availableSlots.Add(currentSlot.ToString(@"hh\:mm"));
                    }
                    currentSlot = currentSlot.Add(oneHour);
                }
            }

            return availableSlots;
        }
    }
}