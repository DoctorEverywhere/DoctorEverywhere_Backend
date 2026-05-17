using System.Threading;
using System.Threading.Tasks;
using DoctorEverywhere.Domain;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DoctorEverywhere.Services
{
    public class AnalyticsService : IAnalyticsService
    {
 
        private ApplicationDbContext _context;

        public AnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }


        /*
         * //We're using a DbContextFactory to create new instances of DbContext for each method.
        //Otherwise, we ran to the issue of having one query finish before the other and attempting to reuse the same context instance,
        //which is not thread-safe.
       private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

       public AnalyticsService(IDbContextFactory<ApplicationDbContext> contextFactory)
       {
           _contextFactory = contextFactory;
       }
       */
        public async Task<AnalyticsSummaryDto> GetAnalyticsSummary()
        {
            /*
            var statusCountTask = GetAppointmentStatusStats();
            var specialtyCountTask = GetAppointmentsBySpecialtyStats();
            var reviewCountTask = GetReviewSummary();

            await Task.WhenAll(statusCountTask, specialtyCountTask,reviewCountTask);

            return new AnalyticsSummaryDto
            {
                AppointmentsByStatusCount = await statusCountTask,
                DemandBySpecialtyCount = await specialtyCountTask,
                ReviewsByRatingCount = await reviewCountTask
            };
            */
             var statusCountTask = await GetAppointmentStatusStats();
            var specialtyCountTask = await GetAppointmentsBySpecialtyStats();
            var reviewCountTask = await GetReviewSummary();
            return new AnalyticsSummaryDto
            {
                AppointmentsByStatusCount = statusCountTask,
                DemandBySpecialtyCount = specialtyCountTask,
                ReviewsByRatingCount = reviewCountTask
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
            // await using var context = await _contextFactory.CreateDbContextAsync();

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

        private async Task<List<DoctorReviewSummaryDto>> GetReviewSummary()
        {

            return await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Doctor)
                .ThenInclude(d => d.Office) 
                .GroupBy(r => new
                 {
                    r.DoctorId,
                    r.Doctor.FirstName,
                    r.Doctor.LastName,
                    r.Doctor.Specialty
                 })
                .Select(g => new DoctorReviewSummaryDto
                {
                    DoctorId = g.Key.DoctorId,
                    DoctorName = $"{g.Key.FirstName} {g.Key.LastName}",
                    Specialty = g.Key.Specialty,
                    ReviewCount = g.Count(),
                    AverageRating = g.Average(r => r.Rating)
                })
                .OrderByDescending(d => d.AverageRating)
                .ToListAsync();
        }
    }
}
