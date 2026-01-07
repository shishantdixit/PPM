using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.Stock;
using PPM.Application.Interfaces;
using PPM.Core.Entities;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class StockEntriesController : ControllerBase
{
    private readonly ITankService _tankService;
    private readonly ILogger<StockEntriesController> _logger;

    public StockEntriesController(ITankService tankService, ILogger<StockEntriesController> logger)
    {
        _tankService = tankService;
        _logger = logger;
    }

    /// <summary>
    /// Get stock history with filters and pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetHistory([FromQuery] StockHistoryFilterDto filter)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var result = await _tankService.GetStockHistoryAsync(tenantId.Value, filter);
            return Ok(ApiResponse<PagedResult<StockEntryDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock history");
            return StatusCode(500, ApiResponse<PagedResult<StockEntryDto>>.ErrorResponse("Failed to fetch stock history"));
        }
    }

    /// <summary>
    /// Record stock in (fuel received from supplier) - Owner/Manager only
    /// </summary>
    [HttpPost("stock-in")]
    [RequireManager]
    public async Task<IActionResult> RecordStockIn([FromBody] CreateStockInDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var userId = HttpContext.GetUserId();
            if (userId == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorResponse("User context not found"));

            if (dto.Quantity <= 0)
                return BadRequest(ApiResponse<StockEntryDto>.ErrorResponse("Quantity must be greater than zero"));

            var entry = await _tankService.RecordStockInAsync(tenantId.Value, userId, dto);
            return CreatedAtAction(nameof(GetHistory), ApiResponse<StockEntryDto>.SuccessResponse(entry, "Stock in recorded successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockEntryDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording stock in");
            return StatusCode(500, ApiResponse<StockEntryDto>.ErrorResponse("Failed to record stock in"));
        }
    }

    /// <summary>
    /// Record stock adjustment (manual correction) - Owner/Manager only
    /// </summary>
    [HttpPost("adjustment")]
    [RequireManager]
    public async Task<IActionResult> RecordAdjustment([FromBody] CreateStockAdjustmentDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var userId = HttpContext.GetUserId();
            if (userId == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorResponse("User context not found"));

            if (dto.Quantity == 0)
                return BadRequest(ApiResponse<StockEntryDto>.ErrorResponse("Quantity cannot be zero"));

            var entry = await _tankService.RecordStockAdjustmentAsync(tenantId.Value, userId, dto);
            return CreatedAtAction(nameof(GetHistory), ApiResponse<StockEntryDto>.SuccessResponse(entry, "Stock adjustment recorded successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockEntryDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording stock adjustment");
            return StatusCode(500, ApiResponse<StockEntryDto>.ErrorResponse("Failed to record stock adjustment"));
        }
    }

    /// <summary>
    /// Get stock entries by tank
    /// </summary>
    [HttpGet("by-tank/{tankId}")]
    public async Task<IActionResult> GetByTank(Guid tankId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var filter = new StockHistoryFilterDto
            {
                TankId = tankId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _tankService.GetStockHistoryAsync(tenantId.Value, filter);
            return Ok(ApiResponse<PagedResult<StockEntryDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock entries for tank {TankId}", tankId);
            return StatusCode(500, ApiResponse<PagedResult<StockEntryDto>>.ErrorResponse("Failed to fetch stock entries"));
        }
    }

    /// <summary>
    /// Get stock entries by type
    /// </summary>
    [HttpGet("by-type/{entryType}")]
    public async Task<IActionResult> GetByType(StockEntryType entryType, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var filter = new StockHistoryFilterDto
            {
                EntryType = entryType,
                Page = page,
                PageSize = pageSize
            };

            var result = await _tankService.GetStockHistoryAsync(tenantId.Value, filter);
            return Ok(ApiResponse<PagedResult<StockEntryDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock entries by type {EntryType}", entryType);
            return StatusCode(500, ApiResponse<PagedResult<StockEntryDto>>.ErrorResponse("Failed to fetch stock entries"));
        }
    }
}
