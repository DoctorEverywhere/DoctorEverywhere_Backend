using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DoctorEverywhere.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private ApplicationDbContext _context;

        public AnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AnalyticsSummaryDto> GetAnalyticsSummary()
        {
            var statusCountTask = GetAppointmentStatusStats();
            var specialtyCountTask = GetAppointmentsBySpecialtyStats();

            await Task.WhenAll(statusCountTask, specialtyCountTask);

            return new AnalyticsSummaryDto
            {
                AppointmentsByStatusCount = await statusCountTask,
                DemandBySpecialtyCount = await specialtyCountTask
            };
        }
        private async Task<List<AppointmentStatusCountDto>> GetAppointmentStatusStats()
        {
            return await _context.Appointments
                .AsNoTracking() //no tracking returned entiries(faster),used for aggregation
                .GroupBy(a => a.StatusId)
                .Select(global => new AppointmentStatusCountDto
                {
                    Status = global.Key,
                    Count = global.Count()
                })
                .ToListAsync();
        }

        private async Task<List<SpecialtyDemandDto>> GetAppointmentsBySpecialtyStats()
        {
            return await _context.Appointments
                .AsNoTracking()
                .Join(
                _context.Doctors.IgnoreQueryFilters(), //Include appointments for deactivated doctors
                a => a.DoctorId,
                d => d.Id,
                (a,d) => new { a,d.Specialty }
                )
                .GroupBy(x => x.Specialty)
                .Select(g => new SpecialtyDemandDto
                {
                    Specialty = g.Key,
                    Count = g.Count()
                }) 
                .ToListAsync();
        }
    }
}
