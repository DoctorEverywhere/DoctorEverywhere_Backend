using System.Security.Claims;
using Azure.Core;
using DoctorEverywhere.DTOs;
using DoctorEverywhere.Enums;
using DoctorEverywhere.Exceptions;
using DoctorEverywhere.Messaging.DTOs;
using DoctorEverywhere.Messaging.Interfaces;
using DoctorEverywhere.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        [HttpGet("/summary")]
        public async Task<IActionResult> GetAppointmentStatusStats()
        {
            try
            {
                var stats = await _analyticsService.GetAppointmentStatusStats();
                return StatusCode(StatusCodes.Status200OK, stats);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
