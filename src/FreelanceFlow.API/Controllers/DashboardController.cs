using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FreelanceFlow.Application.Services.Interfaces;

namespace FreelanceFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await _dashboardService.GetDashboardStatsAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpGet("recent-activities")]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int count = 10)
    {
        var result = await _dashboardService.GetRecentActivitiesAsync(count);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpGet("monthly-revenue")]
    public async Task<IActionResult> GetMonthlyRevenue()
    {
        var result = await _dashboardService.GetMonthlyRevenueAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpGet("project-status-stats")]
    public async Task<IActionResult> GetProjectStatusStats()
    {
        var result = await _dashboardService.GetProjectStatusStatsAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }
}