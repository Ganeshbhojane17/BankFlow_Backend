using CustomerService.Application.Features.Dashboard.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _dashboardService.GetSummaryAsync();

            return Ok(new
            {
                success = true,
                message = "Dashboard summary retrieved successfully.",
                data = result,
                errors = Array.Empty<string>()
            });
        }

        [HttpGet("recent-customers")]
        public async Task<IActionResult> GetRecentCustomers([FromQuery] int count = 5)
        {
            if (count < 1 || count > 20)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Count must be between 1 and 20.",
                    errors = Array.Empty<string>()
                });
            }

            var result = await _dashboardService.GetRecentCustomersAsync(count);

            return Ok(new
            {
                success = true,
                message = "Recent customers retrieved successfully.",
                data = result,
                errors = Array.Empty<string>()
            });
        }
    }
}
