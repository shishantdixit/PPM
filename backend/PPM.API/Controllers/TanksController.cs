using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.Stock;
using PPM.Application.Interfaces;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class TanksController : ControllerBase
{
    private readonly ITankService _tankService;
    private readonly ILogger<TanksController> _logger;

    public TanksController(ITankService tankService, ILogger<TanksController> logger)
    {
        _tankService = tankService;
        _logger = logger;
    }

    /// <summary>
    /// Get all tanks for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var tanks = await _tankService.GetAllTanksAsync(tenantId.Value);
            return Ok(ApiResponse<List<TankDto>>.SuccessResponse(tanks));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tanks");
            return StatusCode(500, ApiResponse<List<TankDto>>.ErrorResponse("Failed to fetch tanks"));
        }
    }

    /// <summary>
    /// Get tank by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var tank = await _tankService.GetTankByIdAsync(tenantId.Value, id);
            if (tank == null)
                return NotFound(ApiResponse<TankDto>.ErrorResponse("Tank not found"));

            return Ok(ApiResponse<TankDto>.SuccessResponse(tank));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tank {TankId}", id);
            return StatusCode(500, ApiResponse<TankDto>.ErrorResponse("Failed to fetch tank"));
        }
    }

    /// <summary>
    /// Get tank summary for dashboard
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var summary = await _tankService.GetTankSummaryAsync(tenantId.Value);
            return Ok(ApiResponse<TankSummaryDto>.SuccessResponse(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tank summary");
            return StatusCode(500, ApiResponse<TankSummaryDto>.ErrorResponse("Failed to fetch tank summary"));
        }
    }

    /// <summary>
    /// Create a new tank (Owner/Manager only)
    /// </summary>
    [HttpPost]
    [RequireManager]
    public async Task<IActionResult> Create([FromBody] CreateTankDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var tank = await _tankService.CreateTankAsync(tenantId.Value, dto);
            return CreatedAtAction(nameof(GetById), new { id = tank.TankId }, ApiResponse<TankDto>.SuccessResponse(tank, "Tank created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TankDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tank");
            return StatusCode(500, ApiResponse<TankDto>.ErrorResponse("Failed to create tank"));
        }
    }

    /// <summary>
    /// Update a tank (Owner/Manager only)
    /// </summary>
    [HttpPut("{id}")]
    [RequireManager]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTankDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var tank = await _tankService.UpdateTankAsync(tenantId.Value, id, dto);
            if (tank == null)
                return NotFound(ApiResponse<TankDto>.ErrorResponse("Tank not found"));

            return Ok(ApiResponse<TankDto>.SuccessResponse(tank, "Tank updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TankDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tank {TankId}", id);
            return StatusCode(500, ApiResponse<TankDto>.ErrorResponse("Failed to update tank"));
        }
    }

    /// <summary>
    /// Delete a tank (Owner/Manager only)
    /// </summary>
    [HttpDelete("{id}")]
    [RequireManager]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var result = await _tankService.DeleteTankAsync(tenantId.Value, id);
            if (!result)
                return NotFound(ApiResponse<object>.ErrorResponse("Tank not found"));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Tank deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tank {TankId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete tank"));
        }
    }

    /// <summary>
    /// Get stock summary for a specific tank
    /// </summary>
    [HttpGet("{id}/stock-summary")]
    public async Task<IActionResult> GetStockSummary(Guid id, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var summary = await _tankService.GetTankStockSummaryAsync(tenantId.Value, id, fromDate, toDate);
            if (summary == null)
                return NotFound(ApiResponse<StockSummaryDto>.ErrorResponse("Tank not found"));

            return Ok(ApiResponse<StockSummaryDto>.SuccessResponse(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock summary for tank {TankId}", id);
            return StatusCode(500, ApiResponse<StockSummaryDto>.ErrorResponse("Failed to fetch stock summary"));
        }
    }

    /// <summary>
    /// Get stock movement report for a specific tank
    /// </summary>
    [HttpGet("{id}/movement-report")]
    [RequireManager]
    public async Task<IActionResult> GetMovementReport(Guid id, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var report = await _tankService.GetStockMovementReportAsync(tenantId.Value, id, fromDate, toDate);
            if (report == null)
                return NotFound(ApiResponse<StockMovementReportDto>.ErrorResponse("Tank not found"));

            return Ok(ApiResponse<StockMovementReportDto>.SuccessResponse(report));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching movement report for tank {TankId}", id);
            return StatusCode(500, ApiResponse<StockMovementReportDto>.ErrorResponse("Failed to fetch movement report"));
        }
    }
}
