using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace DoctorEverywhere.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private ApplicationDbContext _context;

        public AnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AppointmentStatusCountDto>> GetAppointmentStatusStats()
        {
            return await _context.Appointments
                .AsNoTracking()
                .GroupBy(a => a.StatusId)
                .Select(global => new AppointmentStatusCountDto
                {
                    Status = global.Key,
                    Count = global.Count()
                })
                .ToListAsync();
        }
    }
}
