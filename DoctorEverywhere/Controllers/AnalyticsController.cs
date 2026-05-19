using DoctorEverywhere.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorEverywhere.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController :ControllerBase
    {
        private IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("summary")]
        public async Task<IActionResult> GetAppointmentStatusStats()
        {
            try
            {
                var stats  = await _analyticsService.GetAnalyticsSummary();
                return StatusCode(StatusCodes.Status200OK, stats);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

       
    }
}
