using Microsoft.AspNetCore.Mvc;
using PPM.Application.Common;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext dbContext, ILogger<HealthController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(ApiResponse<object>.SuccessResponse(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        }));
    }

    [HttpGet("db")]
    public async Task<IActionResult> CheckDatabase()
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();

            if (canConnect)
            {
                var tenantCount = _dbContext.Tenants.Count();
                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    database = "connected",
                    provider = _dbContext.Database.ProviderName,
                    tenantCount = tenantCount
                }));
            }

            return StatusCode(500, ApiResponse<object>.ErrorResponse("Database connection failed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return StatusCode(500, ApiResponse<object>.ErrorResponse($"Database error: {ex.Message}"));
        }
    }
}
