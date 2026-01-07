using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.Shift;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class ShiftsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ShiftsController> _logger;

    public ShiftsController(ApplicationDbContext dbContext, ILogger<ShiftsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all workers for shift assignment
    /// </summary>
    [HttpGet("workers")]
    public async Task<IActionResult> GetWorkers()
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var workers = await _dbContext.Users
                .Where(u => u.TenantId == tenantId.Value && u.Role == "Worker" && u.IsActive)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Username
                })
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(workers));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching workers");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch workers"));
        }
    }

    /// <summary>
    /// Get all machines for shift assignment
    /// </summary>
    [HttpGet("machines")]
    public async Task<IActionResult> GetMachines()
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var machines = await _dbContext.Machines
                .Where(m => m.TenantId == tenantId.Value && m.IsActive)
                .Select(m => new
                {
                    m.MachineId,
                    m.MachineName,
                    m.MachineCode,
                    m.Location,
                    NozzleCount = m.Nozzles.Count(n => n.IsActive)
                })
                .OrderBy(m => m.MachineName)
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(machines));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching machines");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch machines"));
        }
    }

    /// <summary>
    /// Get nozzles for a specific machine (for shift creation)
    /// </summary>
    [HttpGet("machines/{machineId}/nozzles")]
    public async Task<IActionResult> GetMachineNozzles(Guid machineId)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var nozzles = await _dbContext.Nozzles
                .Where(n => n.TenantId == tenantId.Value && n.MachineId == machineId && n.IsActive)
                .Include(n => n.FuelType)
                .Select(n => new
                {
                    n.NozzleId,
                    n.NozzleNumber,
                    n.NozzleName,
                    n.CurrentMeterReading,
                    FuelTypeId = n.FuelTypeId,
                    FuelName = n.FuelType!.FuelName,
                    FuelCode = n.FuelType.FuelCode
                })
                .OrderBy(n => n.NozzleNumber)
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(nozzles));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching machine nozzles");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch nozzles"));
        }
    }

    /// <summary>
    /// Get all shifts for the current tenant (with optional filters)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] ShiftStatus? status = null,
        [FromQuery] Guid? workerId = null)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            IQueryable<Core.Entities.Shift> query = _dbContext.Shifts;

            // Filter by tenant for regular users
            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(s => s.TenantId == tenantId.Value);
            }
            else if (!isSuperAdmin && !tenantId.HasValue)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));
            }

            // Apply filters
            if (fromDate.HasValue)
                query = query.Where(s => s.ShiftDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.ShiftDate <= toDate.Value);

            if (status.HasValue)
                query = query.Where(s => s.Status == status.Value);

            if (workerId.HasValue)
                query = query.Where(s => s.WorkerId == workerId.Value);

            var shifts = await query
                .Include(s => s.Worker)
                .Include(s => s.Machine)
                .OrderByDescending(s => s.ShiftDate)
                .ThenByDescending(s => s.StartTime)
                .Select(s => new ShiftDto
                {
                    ShiftId = s.ShiftId,
                    TenantId = s.TenantId,
                    WorkerId = s.WorkerId,
                    WorkerName = s.Worker!.FullName,
                    MachineId = s.MachineId,
                    MachineName = s.Machine!.MachineName,
                    ShiftDate = s.ShiftDate,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Status = s.Status,
                    TotalSales = s.TotalSales,
                    CashCollected = s.CashCollected,
                    CreditSales = s.CreditSales,
                    DigitalPayments = s.DigitalPayments,
                    Borrowing = s.Borrowing,
                    Variance = s.Variance,
                    Notes = s.Notes,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    NozzleReadings = new List<ShiftNozzleReadingDto>()
                })
                .ToListAsync();

            return Ok(ApiResponse<List<ShiftDto>>.SuccessResponse(shifts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching shifts");
            return StatusCode(500, ApiResponse<List<ShiftDto>>.ErrorResponse("Failed to fetch shifts"));
        }
    }

    /// <summary>
    /// Get shift details by ID with all nozzle readings
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            var shift = await _dbContext.Shifts
                .Where(s => s.ShiftId == id && (isSuperAdmin || s.TenantId == tenantId))
                .Include(s => s.Worker)
                .Include(s => s.Machine)
                .Include(s => s.NozzleReadings)
                    .ThenInclude(nr => nr.Nozzle)
                        .ThenInclude(n => n!.Machine)
                .Include(s => s.NozzleReadings)
                    .ThenInclude(nr => nr.Nozzle)
                        .ThenInclude(n => n!.FuelType)
                .FirstOrDefaultAsync();

            if (shift == null)
                return NotFound(ApiResponse<ShiftDto>.ErrorResponse("Shift not found"));

            var shiftDto = new ShiftDto
            {
                ShiftId = shift.ShiftId,
                TenantId = shift.TenantId,
                WorkerId = shift.WorkerId,
                WorkerName = shift.Worker!.FullName,
                MachineId = shift.MachineId,
                MachineName = shift.Machine!.MachineName,
                ShiftDate = shift.ShiftDate,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Status = shift.Status,
                TotalSales = shift.TotalSales,
                CashCollected = shift.CashCollected,
                CreditSales = shift.CreditSales,
                DigitalPayments = shift.DigitalPayments,
                Borrowing = shift.Borrowing,
                Variance = shift.Variance,
                Notes = shift.Notes,
                CreatedAt = shift.CreatedAt,
                UpdatedAt = shift.UpdatedAt,
                NozzleReadings = shift.NozzleReadings.Select(nr => new ShiftNozzleReadingDto
                {
                    ShiftNozzleReadingId = nr.ShiftNozzleReadingId,
                    ShiftId = nr.ShiftId,
                    NozzleId = nr.NozzleId,
                    NozzleNumber = nr.Nozzle!.NozzleNumber,
                    NozzleName = nr.Nozzle.NozzleName ?? "",
                    MachineName = nr.Nozzle.Machine!.MachineName,
                    FuelName = nr.Nozzle.FuelType!.FuelName,
                    FuelCode = nr.Nozzle.FuelType.FuelCode,
                    OpeningReading = nr.OpeningReading,
                    ClosingReading = nr.ClosingReading,
                    QuantitySold = nr.QuantitySold,
                    RateAtShift = nr.RateAtShift,
                    ExpectedAmount = nr.ExpectedAmount
                }).ToList()
            };

            return Ok(ApiResponse<ShiftDto>.SuccessResponse(shiftDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching shift {ShiftId}", id);
            return StatusCode(500, ApiResponse<ShiftDto>.ErrorResponse("Failed to fetch shift"));
        }
    }

    /// <summary>
    /// Get current user's active shift
    /// </summary>
    [HttpGet("my-active")]
    public async Task<IActionResult> GetMyActiveShift()
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var userId = HttpContext.GetUserId();

            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("User context not found"));

            var activeShift = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value && s.WorkerId == userId && s.Status == ShiftStatus.Active)
                .Include(s => s.Worker)
                .Include(s => s.Machine)
                .Include(s => s.NozzleReadings)
                    .ThenInclude(nr => nr.Nozzle)
                        .ThenInclude(n => n!.Machine)
                .Include(s => s.NozzleReadings)
                    .ThenInclude(nr => nr.Nozzle)
                        .ThenInclude(n => n!.FuelType)
                .FirstOrDefaultAsync();

            if (activeShift == null)
                return Ok(ApiResponse<ShiftDto?>.SuccessResponse(null, "No active shift found"));

            var shiftDto = new ShiftDto
            {
                ShiftId = activeShift.ShiftId,
                TenantId = activeShift.TenantId,
                WorkerId = activeShift.WorkerId,
                WorkerName = activeShift.Worker!.FullName,
                MachineId = activeShift.MachineId,
                MachineName = activeShift.Machine!.MachineName,
                ShiftDate = activeShift.ShiftDate,
                StartTime = activeShift.StartTime,
                EndTime = activeShift.EndTime,
                Status = activeShift.Status,
                TotalSales = activeShift.TotalSales,
                CashCollected = activeShift.CashCollected,
                CreditSales = activeShift.CreditSales,
                DigitalPayments = activeShift.DigitalPayments,
                Borrowing = activeShift.Borrowing,
                Variance = activeShift.Variance,
                Notes = activeShift.Notes,
                CreatedAt = activeShift.CreatedAt,
                UpdatedAt = activeShift.UpdatedAt,
                NozzleReadings = activeShift.NozzleReadings.Select(nr => new ShiftNozzleReadingDto
                {
                    ShiftNozzleReadingId = nr.ShiftNozzleReadingId,
                    ShiftId = nr.ShiftId,
                    NozzleId = nr.NozzleId,
                    NozzleNumber = nr.Nozzle!.NozzleNumber,
                    NozzleName = nr.Nozzle.NozzleName ?? "",
                    MachineName = nr.Nozzle.Machine!.MachineName,
                    FuelName = nr.Nozzle.FuelType!.FuelName,
                    FuelCode = nr.Nozzle.FuelType.FuelCode,
                    OpeningReading = nr.OpeningReading,
                    ClosingReading = nr.ClosingReading,
                    QuantitySold = nr.QuantitySold,
                    RateAtShift = nr.RateAtShift,
                    ExpectedAmount = nr.ExpectedAmount
                }).ToList()
            };

            return Ok(ApiResponse<ShiftDto>.SuccessResponse(shiftDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching active shift");
            return StatusCode(500, ApiResponse<ShiftDto>.ErrorResponse("Failed to fetch active shift"));
        }
    }

    /// <summary>
    /// Start a new shift
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> StartShift([FromBody] CreateShiftDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var userId = HttpContext.GetUserId();
            var userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("User context not found"));

            // Determine target worker ID
            var targetWorkerId = dto.WorkerId ?? userId;

            // Authorization: Workers can only start shifts for themselves
            if (userRole == "Worker" && targetWorkerId != userId)
                return Forbid();

            // Check if target worker already has an active shift
            var hasActiveShift = await _dbContext.Shifts
                .AnyAsync(s => s.TenantId == tenantId.Value && s.WorkerId == targetWorkerId && s.Status == ShiftStatus.Active);

            if (hasActiveShift)
            {
                var workerName = await _dbContext.Users
                    .Where(u => u.UserId == targetWorkerId)
                    .Select(u => u.FullName)
                    .FirstOrDefaultAsync();
                return BadRequest(ApiResponse<ShiftDto>.ErrorResponse($"{workerName ?? "This worker"} already has an active shift. Please close it first."));
            }

            // Validate machine exists and belongs to tenant
            var machine = await _dbContext.Machines
                .FirstOrDefaultAsync(m => m.MachineId == dto.MachineId && m.TenantId == tenantId.Value && m.IsActive);

            if (machine == null)
                return BadRequest(ApiResponse<ShiftDto>.ErrorResponse("Machine not found or is inactive"));

            // If no nozzles specified, get all active nozzles for the machine
            List<NozzleReadingInput> openingReadings;
            if (dto.OpeningReadings == null || !dto.OpeningReadings.Any())
            {
                // Auto-populate with all nozzles on the machine
                var machineNozzles = await _dbContext.Nozzles
                    .Where(n => n.TenantId == tenantId.Value && n.MachineId == dto.MachineId && n.IsActive)
                    .ToListAsync();

                if (!machineNozzles.Any())
                    return BadRequest(ApiResponse<ShiftDto>.ErrorResponse("No active nozzles found for the selected machine"));

                openingReadings = machineNozzles.Select(n => new NozzleReadingInput
                {
                    NozzleId = n.NozzleId,
                    Reading = n.CurrentMeterReading
                }).ToList();
            }
            else
            {
                // Validate that specified nozzles belong to the selected machine
                var specifiedNozzleIds = dto.OpeningReadings.Select(r => r.NozzleId).ToList();
                var validNozzles = await _dbContext.Nozzles
                    .Where(n => n.TenantId == tenantId.Value && n.MachineId == dto.MachineId && specifiedNozzleIds.Contains(n.NozzleId) && n.IsActive)
                    .Select(n => n.NozzleId)
                    .ToListAsync();

                var invalidNozzles = specifiedNozzleIds.Except(validNozzles).ToList();
                if (invalidNozzles.Any())
                    return BadRequest(ApiResponse<ShiftDto>.ErrorResponse("Some specified nozzles don't belong to the selected machine or are inactive"));

                openingReadings = dto.OpeningReadings;
            }

            // Check if any nozzles are already being used in other active shifts
            var nozzleIds = openingReadings.Select(r => r.NozzleId).ToList();
            var nozzlesInUse = await _dbContext.ShiftNozzleReadings
                .Where(snr => snr.TenantId == tenantId.Value
                    && nozzleIds.Contains(snr.NozzleId)
                    && snr.Shift!.Status == ShiftStatus.Active
                    && snr.Shift.WorkerId != targetWorkerId)  // Exclude if same worker (shouldn't happen but just in case)
                .Include(snr => snr.Nozzle)
                .Include(snr => snr.Shift)
                    .ThenInclude(s => s!.Worker)
                .Select(snr => new
                {
                    NozzleNumber = snr.Nozzle!.NozzleNumber,
                    WorkerName = snr.Shift!.Worker!.FullName
                })
                .ToListAsync();

            if (nozzlesInUse.Any())
            {
                var nozzleDetails = string.Join(", ", nozzlesInUse.Select(n => $"{n.NozzleNumber} (being used by {n.WorkerName})"));
                return BadRequest(ApiResponse<ShiftDto>.ErrorResponse($"Cannot start shift. The following nozzles are already in use: {nozzleDetails}. Please close those shifts first."));
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Create shift
                var shift = new Core.Entities.Shift
                {
                    ShiftId = Guid.NewGuid(),
                    TenantId = tenantId.Value,
                    WorkerId = targetWorkerId,
                    MachineId = dto.MachineId,
                    ShiftDate = dto.ShiftDate,
                    StartTime = dto.StartTime,
                    Status = ShiftStatus.Active
                };

                _dbContext.Shifts.Add(shift);

                // Add nozzle readings with current fuel rates
                foreach (var reading in openingReadings)
                {
                    // Get nozzle with fuel type
                    var nozzle = await _dbContext.Nozzles
                        .Include(n => n.FuelType)
                        .FirstOrDefaultAsync(n => n.NozzleId == reading.NozzleId && n.TenantId == tenantId.Value);

                    if (nozzle == null)
                        return BadRequest(ApiResponse<ShiftDto>.ErrorResponse($"Nozzle not found: {reading.NozzleId}"));

                    // Get current fuel rate
                    var currentRate = await _dbContext.FuelRates
                        .Where(fr => fr.TenantId == tenantId.Value && fr.FuelTypeId == nozzle.FuelTypeId && fr.EffectiveTo == null)
                        .FirstOrDefaultAsync();

                    if (currentRate == null)
                        return BadRequest(ApiResponse<ShiftDto>.ErrorResponse($"No current rate found for {nozzle.FuelType!.FuelName}"));

                    // Validate reading
                    if (reading.Reading < nozzle.CurrentMeterReading)
                        return BadRequest(ApiResponse<ShiftDto>.ErrorResponse($"Opening reading for {nozzle.FuelType.FuelName} nozzle {nozzle.NozzleNumber} cannot be less than current meter reading ({nozzle.CurrentMeterReading})"));

                    var shiftNozzleReading = new ShiftNozzleReading
                    {
                        ShiftNozzleReadingId = Guid.NewGuid(),
                        TenantId = tenantId.Value,
                        ShiftId = shift.ShiftId,
                        NozzleId = reading.NozzleId,
                        OpeningReading = reading.Reading,
                        RateAtShift = currentRate.Rate
                    };

                    _dbContext.ShiftNozzleReadings.Add(shiftNozzleReading);

                    // Update nozzle's current meter reading
                    nozzle.CurrentMeterReading = reading.Reading;
                    nozzle.UpdatedAt = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Started shift {ShiftId} for worker {WorkerId}", shift.ShiftId, targetWorkerId);

                // Reload with navigation properties
                var result = await _dbContext.Shifts
                    .Where(s => s.ShiftId == shift.ShiftId)
                    .Include(s => s.Worker)
                    .Include(s => s.Machine)
                    .Include(s => s.NozzleReadings)
                        .ThenInclude(nr => nr.Nozzle)
                            .ThenInclude(n => n!.Machine)
                    .Include(s => s.NozzleReadings)
                        .ThenInclude(nr => nr.Nozzle)
                            .ThenInclude(n => n!.FuelType)
                    .Select(s => new ShiftDto
                    {
                        ShiftId = s.ShiftId,
                        TenantId = s.TenantId,
                        WorkerId = s.WorkerId,
                        WorkerName = s.Worker!.FullName,
                        MachineId = s.MachineId,
                        MachineName = s.Machine!.MachineName,
                        ShiftDate = s.ShiftDate,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Status = s.Status,
                        TotalSales = s.TotalSales,
                        CashCollected = s.CashCollected,
                        CreditSales = s.CreditSales,
                        DigitalPayments = s.DigitalPayments,
                        Borrowing = s.Borrowing,
                        Variance = s.Variance,
                        Notes = s.Notes,
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.UpdatedAt,
                        NozzleReadings = s.NozzleReadings.Select(nr => new ShiftNozzleReadingDto
                        {
                            ShiftNozzleReadingId = nr.ShiftNozzleReadingId,
                            ShiftId = nr.ShiftId,
                            NozzleId = nr.NozzleId,
                            NozzleNumber = nr.Nozzle!.NozzleNumber,
                            NozzleName = nr.Nozzle.NozzleName ?? "",
                            MachineName = nr.Nozzle.Machine!.MachineName,
                            FuelName = nr.Nozzle.FuelType!.FuelName,
                            FuelCode = nr.Nozzle.FuelType.FuelCode,
                            OpeningReading = nr.OpeningReading,
                            ClosingReading = nr.ClosingReading,
                            QuantitySold = nr.QuantitySold,
                            RateAtShift = nr.RateAtShift,
                            ExpectedAmount = nr.ExpectedAmount
                        }).ToList()
                    })
                    .FirstAsync();

                return Ok(ApiResponse<ShiftDto>.SuccessResponse(result, "Shift started successfully"));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting shift");
            return StatusCode(500, ApiResponse<ShiftDto>.ErrorResponse("Failed to start shift"));
        }
    }

    /// <summary>
    /// Close shift with settlement
    /// </summary>
    [HttpPut("{id}/close")]
    public async Task<IActionResult> CloseShift(Guid id, [FromBody] CloseShiftDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var userId = HttpContext.GetUserId();

            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("User context not found"));

            var shift = await _dbContext.Shifts
                .Include(s => s.NozzleReadings)
                    .ThenInclude(nr => nr.Nozzle)
                .FirstOrDefaultAsync(s => s.ShiftId == id && s.TenantId == tenantId.Value);

            if (shift == null)
                return NotFound(ApiResponse<ShiftDto>.ErrorResponse("Shift not found"));

            if (shift.Status != ShiftStatus.Active)
                return BadRequest(ApiResponse<ShiftDto>.ErrorResponse("Only active shifts can be closed"));

            // Only worker who started the shift or manager/owner can close it
            var userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            if (shift.WorkerId != userId && userRole != "Manager" && userRole != "Owner")
                return Forbid();

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Update closing readings and calculate quantities
                foreach (var closingReading in dto.ClosingReadings)
                {
                    var nozzleReading = shift.NozzleReadings.FirstOrDefault(nr => nr.NozzleId == closingReading.NozzleId);
                    if (nozzleReading == null)
                        return BadRequest(ApiResponse<ShiftDto>.ErrorResponse($"Nozzle reading not found for nozzle: {closingReading.NozzleId}"));

                    // Validate closing reading
                    if (closingReading.Reading < nozzleReading.OpeningReading)
                        return BadRequest(ApiResponse<ShiftDto>.ErrorResponse("Closing reading cannot be less than opening reading"));

                    nozzleReading.ClosingReading = closingReading.Reading;
                    nozzleReading.QuantitySold = closingReading.Reading - nozzleReading.OpeningReading;
                    nozzleReading.ExpectedAmount = nozzleReading.QuantitySold * nozzleReading.RateAtShift;

                    // Update nozzle's current meter reading
                    nozzleReading.Nozzle!.CurrentMeterReading = closingReading.Reading;
                    nozzleReading.Nozzle.UpdatedAt = DateTime.UtcNow;
                }

                // Calculate totals
                shift.TotalSales = shift.NozzleReadings.Sum(nr => nr.ExpectedAmount);
                shift.CashCollected = dto.CashCollected;
                shift.CreditSales = dto.CreditSales;
                shift.DigitalPayments = dto.DigitalPayments;
                shift.Borrowing = dto.Borrowing;

                // Calculate variance: Expected Sales - (Cash + Credit + Digital)
                var totalCollected = dto.CashCollected + dto.CreditSales + dto.DigitalPayments;
                shift.Variance = shift.TotalSales - totalCollected;

                // Calculate payment breakdown from individual sales for reconciliation
                var salesSummary = await _dbContext.FuelSales
                    .Where(fs => fs.ShiftId == shift.ShiftId)
                    .GroupBy(fs => fs.PaymentMethod)
                    .Select(g => new { PaymentMethod = g.Key, TotalAmount = g.Sum(fs => fs.Amount) })
                    .ToListAsync();

                var salesCash = salesSummary.FirstOrDefault(s => s.PaymentMethod == PaymentMethod.Cash)?.TotalAmount ?? 0;
                var salesCredit = salesSummary.FirstOrDefault(s => s.PaymentMethod == PaymentMethod.Credit)?.TotalAmount ?? 0;
                var salesDigital = salesSummary.FirstOrDefault(s => s.PaymentMethod == PaymentMethod.Digital)?.TotalAmount ?? 0;
                var totalFromSales = salesCash + salesCredit + salesDigital;

                // Log reconciliation
                _logger.LogInformation(
                    "Shift {ShiftId} closure - Meter sales: {MeterSales}, Individual sales: {IndividualSales}",
                    id, shift.TotalSales, totalFromSales);

                // Warn if >5% variance between meter readings and individual sales
                if (shift.TotalSales > 0 && Math.Abs(shift.TotalSales - totalFromSales) > (shift.TotalSales * 0.05m))
                {
                    _logger.LogWarning(
                        "Shift {ShiftId} has significant variance between meter readings ({MeterSales}) and individual sales ({IndividualSales})",
                        id, shift.TotalSales, totalFromSales);
                }

                shift.EndTime = dto.EndTime;
                shift.Status = ShiftStatus.Closed;
                shift.Notes = dto.Notes;
                shift.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Closed shift {ShiftId} with variance {Variance}", id, shift.Variance);

                // Reload with navigation properties
                var result = await _dbContext.Shifts
                    .Where(s => s.ShiftId == id)
                    .Include(s => s.Worker)
                    .Include(s => s.Machine)
                    .Include(s => s.NozzleReadings)
                        .ThenInclude(nr => nr.Nozzle)
                            .ThenInclude(n => n!.Machine)
                    .Include(s => s.NozzleReadings)
                        .ThenInclude(nr => nr.Nozzle)
                            .ThenInclude(n => n!.FuelType)
                    .Include(s => s.FuelSales)
                        .ThenInclude(fs => fs.Nozzle)
                            .ThenInclude(n => n!.FuelType)
                    .Select(s => new ShiftDto
                    {
                        ShiftId = s.ShiftId,
                        TenantId = s.TenantId,
                        WorkerId = s.WorkerId,
                        WorkerName = s.Worker!.FullName,
                        MachineId = s.MachineId,
                        MachineName = s.Machine!.MachineName,
                        ShiftDate = s.ShiftDate,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Status = s.Status,
                        TotalSales = s.TotalSales,
                        CashCollected = s.CashCollected,
                        CreditSales = s.CreditSales,
                        DigitalPayments = s.DigitalPayments,
                        Borrowing = s.Borrowing,
                        Variance = s.Variance,
                        Notes = s.Notes,
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.UpdatedAt,
                        NozzleReadings = s.NozzleReadings.Select(nr => new ShiftNozzleReadingDto
                        {
                            ShiftNozzleReadingId = nr.ShiftNozzleReadingId,
                            ShiftId = nr.ShiftId,
                            NozzleId = nr.NozzleId,
                            NozzleNumber = nr.Nozzle!.NozzleNumber,
                            NozzleName = nr.Nozzle.NozzleName ?? "",
                            MachineName = nr.Nozzle.Machine!.MachineName,
                            FuelName = nr.Nozzle.FuelType!.FuelName,
                            FuelCode = nr.Nozzle.FuelType.FuelCode,
                            OpeningReading = nr.OpeningReading,
                            ClosingReading = nr.ClosingReading,
                            QuantitySold = nr.QuantitySold,
                            RateAtShift = nr.RateAtShift,
                            ExpectedAmount = nr.ExpectedAmount
                        }).ToList(),
                        FuelSales = s.FuelSales.Select(fs => new FuelSaleDto
                        {
                            FuelSaleId = fs.FuelSaleId,
                            ShiftId = fs.ShiftId,
                            NozzleId = fs.NozzleId,
                            SaleNumber = fs.SaleNumber,
                            NozzleNumber = fs.Nozzle!.NozzleNumber,
                            FuelName = fs.Nozzle.FuelType!.FuelName,
                            Quantity = fs.Quantity,
                            Rate = fs.Rate,
                            Amount = fs.Amount,
                            PaymentMethod = fs.PaymentMethod,
                            CustomerName = fs.CustomerName,
                            CustomerPhone = fs.CustomerPhone,
                            VehicleNumber = fs.VehicleNumber,
                            SaleTime = fs.SaleTime,
                            Notes = fs.Notes,
                            IsVoided = fs.IsVoided,
                            VoidedAt = fs.VoidedAt,
                            VoidedBy = fs.VoidedBy,
                            VoidReason = fs.VoidReason
                        }).ToList()
                    })
                    .FirstAsync();

                return Ok(ApiResponse<ShiftDto>.SuccessResponse(result, "Shift closed successfully"));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing shift {ShiftId}", id);
            return StatusCode(500, ApiResponse<ShiftDto>.ErrorResponse("Failed to close shift"));
        }
    }
}
