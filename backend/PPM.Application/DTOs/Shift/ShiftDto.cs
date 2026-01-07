using PPM.Core.Entities;

namespace PPM.Application.DTOs.Shift;

public class ShiftDto
{
    public Guid ShiftId { get; set; }
    public Guid TenantId { get; set; }
    public Guid WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public DateOnly ShiftDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public ShiftStatus Status { get; set; }
    public decimal TotalSales { get; set; }
    public decimal CashCollected { get; set; }
    public decimal CreditSales { get; set; }
    public decimal DigitalPayments { get; set; }
    public decimal Borrowing { get; set; }
    public decimal Variance { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<ShiftNozzleReadingDto> NozzleReadings { get; set; } = new();
    public List<FuelSaleDto> FuelSales { get; set; } = new();
}

public class ShiftNozzleReadingDto
{
    public Guid ShiftNozzleReadingId { get; set; }
    public Guid ShiftId { get; set; }
    public Guid NozzleId { get; set; }
    public string NozzleNumber { get; set; } = string.Empty;
    public string NozzleName { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string FuelName { get; set; } = string.Empty;
    public string FuelCode { get; set; } = string.Empty;
    public decimal OpeningReading { get; set; }
    public decimal? ClosingReading { get; set; }
    public decimal QuantitySold { get; set; }
    public decimal RateAtShift { get; set; }
    public decimal ExpectedAmount { get; set; }
}

public class CreateShiftDto
{
    public Guid? WorkerId { get; set; }  // Optional - defaults to logged-in user if not specified
    public DateOnly ShiftDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public List<NozzleReadingInput> OpeningReadings { get; set; } = new();
}

public class NozzleReadingInput
{
    public Guid NozzleId { get; set; }
    public decimal Reading { get; set; }
}

public class CloseShiftDto
{
    public TimeOnly EndTime { get; set; }
    public List<NozzleReadingInput> ClosingReadings { get; set; } = new();
    public decimal CashCollected { get; set; }
    public decimal CreditSales { get; set; }
    public decimal DigitalPayments { get; set; }
    public decimal Borrowing { get; set; }
    public string? Notes { get; set; }
}

public class FuelSaleDto
{
    public Guid FuelSaleId { get; set; }
    public Guid ShiftId { get; set; }
    public Guid NozzleId { get; set; }
    public string NozzleNumber { get; set; } = string.Empty;
    public string FuelName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? VehicleNumber { get; set; }
    public DateTime SaleTime { get; set; }
    public string? Notes { get; set; }
}

public class CreateFuelSaleDto
{
    public Guid NozzleId { get; set; }
    public decimal Quantity { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? VehicleNumber { get; set; }
    public string? Notes { get; set; }
}
