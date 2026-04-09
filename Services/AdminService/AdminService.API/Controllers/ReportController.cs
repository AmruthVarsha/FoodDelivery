using AdminService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest(new { message = "From date must be before To date" });

            var report = await _reportService.GetSalesReportAsync(from, to);
            return Ok(report);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUserReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest(new { message = "From date must be before To date" });

            var report = await _reportService.GetUserReportAsync(from, to);
            return Ok(report);
        }

        [HttpGet("restaurants")]
        public async Task<IActionResult> GetRestaurantReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest(new { message = "From date must be before To date" });

            var report = await _reportService.GetRestaurantReportAsync(from, to);
            return Ok(report);
        }
    }
}
