using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    FeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FeatureName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.FeatureId);
                });

            migrationBuilder.CreateTable(
                name: "SystemUsers",
                columns: table => new
                {
                    SystemUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "SuperAdmin"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUsers", x => x.SystemUserId);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "India"),
                    PinCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    SubscriptionPlan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SubscriptionStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SubscriptionStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubscriptionEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxMachines = table.Column<int>(type: "integer", nullable: false),
                    MaxWorkers = table.Column<int>(type: "integer", nullable: false),
                    MaxMonthlyBills = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "PlanFeatures",
                columns: table => new
                {
                    PlanFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPlan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanFeatures", x => x.PlanFeatureId);
                    table.ForeignKey(
                        name: "FK_PlanFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "FeatureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCustomers",
                columns: table => new
                {
                    CreditCustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentTermDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    BlockReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VehicleNumbers = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCustomers", x => x.CreditCustomerId);
                    table.ForeignKey(
                        name: "FK_CreditCustomers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FuelTypes",
                columns: table => new
                {
                    FuelTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FuelName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FuelCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Liters"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelTypes", x => x.FuelTypeId);
                    table.ForeignKey(
                        name: "FK_FuelTypes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    MachineId = table.Column<Guid>(type: "uuid", nullable: false),
                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MachineCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InstallationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.MachineId);
                    table.ForeignKey(
                        name: "FK_Machines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantFeatures",
                columns: table => new
                {
                    TenantFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsOverridden = table.Column<bool>(type: "boolean", nullable: false),
                    OverriddenBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OverriddenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantFeatures", x => x.TenantFeatureId);
                    table.ForeignKey(
                        name: "FK_TenantFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "FeatureId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantFeatures_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Salary = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tanks",
                columns: table => new
                {
                    TankId = table.Column<Guid>(type: "uuid", nullable: false),
                    TankName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TankCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FuelTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Capacity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    CurrentStock = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    MinimumLevel = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    InstallationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LastCalibrationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tanks", x => x.TankId);
                    table.ForeignKey(
                        name: "FK_Tanks_FuelTypes_FuelTypeId",
                        column: x => x.FuelTypeId,
                        principalTable: "FuelTypes",
                        principalColumn: "FuelTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tanks_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nozzles",
                columns: table => new
                {
                    NozzleId = table.Column<Guid>(type: "uuid", nullable: false),
                    MachineId = table.Column<Guid>(type: "uuid", nullable: false),
                    FuelTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    NozzleNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NozzleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrentMeterReading = table.Column<decimal>(type: "numeric(12,3)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nozzles", x => x.NozzleId);
                    table.ForeignKey(
                        name: "FK_Nozzles_FuelTypes_FuelTypeId",
                        column: x => x.FuelTypeId,
                        principalTable: "FuelTypes",
                        principalColumn: "FuelTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nozzles_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "MachineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Nozzles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpenseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentMode = table.Column<int>(type: "integer", nullable: false),
                    Reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Vendor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RecordedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expenses_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_RecordedById",
                        column: x => x.RecordedById,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FuelRates",
                columns: table => new
                {
                    FuelRateId = table.Column<Guid>(type: "uuid", nullable: false),
                    FuelTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelRates", x => x.FuelRateId);
                    table.ForeignKey(
                        name: "FK_FuelRates_FuelTypes_FuelTypeId",
                        column: x => x.FuelTypeId,
                        principalTable: "FuelTypes",
                        principalColumn: "FuelTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelRates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelRates_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
                    MachineId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TotalSales = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CashCollected = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreditSales = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DigitalPayments = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Borrowing = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Variance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.ShiftId);
                    table.ForeignKey(
                        name: "FK_Shifts_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "MachineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shifts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shifts_Users_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FuelSales",
                columns: table => new
                {
                    FuelSaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    NozzleId = table.Column<Guid>(type: "uuid", nullable: false),
                    SaleNumber = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CustomerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VehicleNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SaleTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsVoided = table.Column<bool>(type: "boolean", nullable: false),
                    VoidedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VoidedBy = table.Column<string>(type: "text", nullable: true),
                    VoidReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelSales", x => x.FuelSaleId);
                    table.ForeignKey(
                        name: "FK_FuelSales_Nozzles_NozzleId",
                        column: x => x.NozzleId,
                        principalTable: "Nozzles",
                        principalColumn: "NozzleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FuelSales_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelSales_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftNozzleReadings",
                columns: table => new
                {
                    ShiftNozzleReadingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    NozzleId = table.Column<Guid>(type: "uuid", nullable: false),
                    OpeningReading = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    ClosingReading = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    QuantitySold = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    RateAtShift = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpectedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftNozzleReadings", x => x.ShiftNozzleReadingId);
                    table.ForeignKey(
                        name: "FK_ShiftNozzleReadings_Nozzles_NozzleId",
                        column: x => x.NozzleId,
                        principalTable: "Nozzles",
                        principalColumn: "NozzleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftNozzleReadings_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftNozzleReadings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditTransactions",
                columns: table => new
                {
                    CreditTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FuelSaleId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentMode = table.Column<int>(type: "integer", nullable: true),
                    PaymentReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTransactions", x => x.CreditTransactionId);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_CreditCustomers_CreditCustomerId",
                        column: x => x.CreditCustomerId,
                        principalTable: "CreditCustomers",
                        principalColumn: "CreditCustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_FuelSales_FuelSaleId",
                        column: x => x.FuelSaleId,
                        principalTable: "FuelSales",
                        principalColumn: "FuelSaleId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockEntries",
                columns: table => new
                {
                    StockEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TankId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntryType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    StockBefore = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    StockAfter = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Vendor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: true),
                    FuelSaleId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockEntries", x => x.StockEntryId);
                    table.ForeignKey(
                        name: "FK_StockEntries_FuelSales_FuelSaleId",
                        column: x => x.FuelSaleId,
                        principalTable: "FuelSales",
                        principalColumn: "FuelSaleId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StockEntries_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StockEntries_Tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "Tanks",
                        principalColumn: "TankId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockEntries_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockEntries_Users_RecordedById",
                        column: x => x.RecordedById,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "FeatureId", "CreatedAt", "Description", "DisplayOrder", "FeatureCode", "FeatureName", "Icon", "IsActive", "Module", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("f0000001-0000-0000-0000-000000000001"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8044), "Access to sales reports, dashboards, and analytics", 1, "REPORTS", "Reports & Analytics", "chart-bar", true, "Analytics", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8047) },
                    { new Guid("f0000002-0000-0000-0000-000000000002"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8055), "Manage credit customers and their balances", 2, "CREDIT_CUSTOMERS", "Credit Customer Management", "credit-card", true, "Sales", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8055) },
                    { new Guid("f0000003-0000-0000-0000-000000000003"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8059), "Track and manage business expenses", 3, "EXPENSES", "Expense Tracking", "receipt", true, "Finance", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8060) },
                    { new Guid("f0000004-0000-0000-0000-000000000004"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8063), "Support for multiple shifts per day", 4, "MULTI_SHIFT", "Multiple Shifts", "clock", true, "Operations", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8064) },
                    { new Guid("f0000005-0000-0000-0000-000000000005"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8067), "Export data to Excel and PDF", 5, "EXPORT", "Data Export", "download", true, "Utilities", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8068) },
                    { new Guid("f0000006-0000-0000-0000-000000000006"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8071), "Programmatic API access for integrations", 6, "API_ACCESS", "API Access", "code", true, "Integration", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8072) },
                    { new Guid("f0000007-0000-0000-0000-000000000007"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8093), "Advanced analytics and custom reports", 7, "ADVANCED_REPORTS", "Advanced Reports", "chart-pie", true, "Analytics", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8094) },
                    { new Guid("f0000008-0000-0000-0000-000000000008"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8097), "Bulk import/export and batch operations", 8, "BULK_OPERATIONS", "Bulk Operations", "database", true, "Utilities", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8098) }
                });

            migrationBuilder.InsertData(
                table: "SystemUsers",
                columns: new[] { "SystemUserId", "CreatedAt", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 9, 6, 10, 50, 586, DateTimeKind.Utc).AddTicks(5110), "admin@ppmapp.com", "Super Administrator", true, null, "$2a$11$bV6OIM2Y8wUJST5L6yazSOQRjpUSFcjG/dvVDKiW76svIAbae033m", "SuperAdmin", new DateTime(2026, 1, 9, 6, 10, 50, 586, DateTimeKind.Utc).AddTicks(5294), "superadmin" });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "TenantId", "Address", "City", "CompanyName", "Country", "CreatedAt", "Email", "IsActive", "MaxMachines", "MaxMonthlyBills", "MaxWorkers", "OwnerName", "Phone", "PinCode", "State", "SubscriptionEndDate", "SubscriptionPlan", "SubscriptionStartDate", "SubscriptionStatus", "TenantCode", "UpdatedAt" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "123 Demo Street, Near City Center", "Mumbai", "Demo Petrol Pump", "India", new DateTime(2026, 1, 9, 6, 10, 50, 587, DateTimeKind.Utc).AddTicks(8355), "demo@petroldemo.com", true, 5, 10000, 20, "Rajesh Kumar", "9876543210", "400001", "Maharashtra", new DateTime(2027, 1, 9, 6, 10, 50, 587, DateTimeKind.Utc).AddTicks(5584), "Premium", new DateTime(2026, 1, 9, 6, 10, 50, 587, DateTimeKind.Utc).AddTicks(5465), "Active", "DEMO001", new DateTime(2026, 1, 9, 6, 10, 50, 587, DateTimeKind.Utc).AddTicks(8357) });

            migrationBuilder.InsertData(
                table: "FuelTypes",
                columns: new[] { "FuelTypeId", "CreatedAt", "FuelCode", "FuelName", "IsActive", "TenantId", "Unit", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1121), "PTR", "Petrol", true, new Guid("22222222-2222-2222-2222-222222222222"), "Liters", new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1134) },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1155), "DSL", "Diesel", true, new Guid("22222222-2222-2222-2222-222222222222"), "Liters", new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1156) },
                    { new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1160), "CNG", "CNG", true, new Guid("22222222-2222-2222-2222-222222222222"), "Kg", new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1161) }
                });

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "MachineId", "CreatedAt", "InstallationDate", "IsActive", "Location", "MachineCode", "MachineName", "Manufacturer", "Model", "SerialNumber", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(3848), new DateOnly(2023, 1, 15), true, "Left Side", "M001", "Machine 1", "Tokheim", "Premier B", "SRL-M1-2023", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(3849) },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(3865), new DateOnly(2023, 3, 20), true, "Right Side", "M002", "Machine 2", "Wayne", "Helix 6000", "SRL-M2-2023", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(3866) }
                });

            migrationBuilder.InsertData(
                table: "PlanFeatures",
                columns: new[] { "PlanFeatureId", "CreatedAt", "FeatureId", "IsEnabled", "SubscriptionPlan", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("ba000001-0000-0000-0000-000000000001"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9827), new Guid("f0000001-0000-0000-0000-000000000001"), false, "Basic", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9828) },
                    { new Guid("ba000002-0000-0000-0000-000000000002"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9834), new Guid("f0000002-0000-0000-0000-000000000002"), false, "Basic", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9835) },
                    { new Guid("ba000003-0000-0000-0000-000000000003"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9848), new Guid("f0000003-0000-0000-0000-000000000003"), false, "Basic", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9849) },
                    { new Guid("ba000004-0000-0000-0000-000000000004"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9853), new Guid("f0000004-0000-0000-0000-000000000004"), false, "Basic", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9853) },
                    { new Guid("ba000005-0000-0000-0000-000000000005"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9862), new Guid("f0000005-0000-0000-0000-000000000005"), false, "Basic", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9863) },
                    { new Guid("ba000006-0000-0000-0000-000000000006"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9867), new Guid("f0000006-0000-0000-0000-000000000006"), false, "Basic", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9867) },
                    { new Guid("ba000007-0000-0000-0000-000000000007"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9871), new Guid("f0000007-0000-0000-0000-000000000007"), false, "Basic", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9872) },
                    { new Guid("ba000008-0000-0000-0000-000000000008"), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9875), new Guid("f0000008-0000-0000-0000-000000000008"), false, "Basic", new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9875) },
                    { new Guid("ca000001-0000-0000-0000-000000000001"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(67), new Guid("f0000001-0000-0000-0000-000000000001"), true, "Premium", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(68) },
                    { new Guid("ca000002-0000-0000-0000-000000000002"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(71), new Guid("f0000002-0000-0000-0000-000000000002"), true, "Premium", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(72) },
                    { new Guid("ca000003-0000-0000-0000-000000000003"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(76), new Guid("f0000003-0000-0000-0000-000000000003"), true, "Premium", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(76) },
                    { new Guid("ca000004-0000-0000-0000-000000000004"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(80), new Guid("f0000004-0000-0000-0000-000000000004"), true, "Premium", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(80) },
                    { new Guid("ca000005-0000-0000-0000-000000000005"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(84), new Guid("f0000005-0000-0000-0000-000000000005"), true, "Premium", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(85) },
                    { new Guid("ca000006-0000-0000-0000-000000000006"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(88), new Guid("f0000006-0000-0000-0000-000000000006"), false, "Premium", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(89) },
                    { new Guid("ca000007-0000-0000-0000-000000000007"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(92), new Guid("f0000007-0000-0000-0000-000000000007"), false, "Premium", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(93) },
                    { new Guid("ca000008-0000-0000-0000-000000000008"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(96), new Guid("f0000008-0000-0000-0000-000000000008"), false, "Premium", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(97) },
                    { new Guid("da000001-0000-0000-0000-000000000001"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(152), new Guid("f0000001-0000-0000-0000-000000000001"), true, "Enterprise", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(153) },
                    { new Guid("da000002-0000-0000-0000-000000000002"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(156), new Guid("f0000002-0000-0000-0000-000000000002"), true, "Enterprise", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(157) },
                    { new Guid("da000003-0000-0000-0000-000000000003"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(160), new Guid("f0000003-0000-0000-0000-000000000003"), true, "Enterprise", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(161) },
                    { new Guid("da000004-0000-0000-0000-000000000004"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(164), new Guid("f0000004-0000-0000-0000-000000000004"), true, "Enterprise", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(165) },
                    { new Guid("da000005-0000-0000-0000-000000000005"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(168), new Guid("f0000005-0000-0000-0000-000000000005"), true, "Enterprise", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(169) },
                    { new Guid("da000006-0000-0000-0000-000000000006"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(172), new Guid("f0000006-0000-0000-0000-000000000006"), true, "Enterprise", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(173) },
                    { new Guid("da000007-0000-0000-0000-000000000007"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(176), new Guid("f0000007-0000-0000-0000-000000000007"), true, "Enterprise", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(177) },
                    { new Guid("da000008-0000-0000-0000-000000000008"), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(180), new Guid("f0000008-0000-0000-0000-000000000008"), true, "Enterprise", new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(181) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "DateOfJoining", "Email", "EmployeeCode", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Phone", "Role", "Salary", "TenantId", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 9, 6, 10, 50, 770, DateTimeKind.Utc).AddTicks(7584), null, "owner@petroldemo.com", null, "Rajesh Kumar", true, null, "$2a$11$jtnPnZLbP04J9085SWqQdurF2UA/7a9VCre34UUOg5bd5u3IlLavu", "9876543210", "Owner", null, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 50, 770, DateTimeKind.Utc).AddTicks(7595), "owner" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 1, 9, 6, 10, 50, 935, DateTimeKind.Utc).AddTicks(3278), new DateTime(2025, 7, 9, 6, 10, 50, 935, DateTimeKind.Utc).AddTicks(2213), "manager@petroldemo.com", "MGR001", "Suresh Patel", true, null, "$2a$11$lsXspL/8haOyyySgQ8.ZgeLRiVkswrG/V.JQQPuA.aneJVQvv8Kuu", "9876543211", "Manager", 30000m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 50, 935, DateTimeKind.Utc).AddTicks(3281), "manager" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 1, 9, 6, 10, 51, 61, DateTimeKind.Utc).AddTicks(3901), new DateTime(2025, 10, 9, 6, 10, 51, 61, DateTimeKind.Utc).AddTicks(3874), "ramesh@petroldemo.com", "EMP001", "Ramesh Kumar", true, null, "$2a$11$tvtbMwtMwyuhaAbGbRgzQunCS7h.y1CGu0uVUSz5rfsQfyrXHc9hO", "9876543212", "Worker", 15000m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 61, DateTimeKind.Utc).AddTicks(3904), "ramesh" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2026, 1, 9, 6, 10, 51, 185, DateTimeKind.Utc).AddTicks(911), new DateTime(2025, 11, 9, 6, 10, 51, 185, DateTimeKind.Utc).AddTicks(885), "dinesh@petroldemo.com", "EMP002", "Dinesh Sharma", true, null, "$2a$11$48bo9mIxLIwW6cptXE7DYukH1Y4gbFczWk3SwvTM2ZvQzuL6fCeZK", "9876543213", "Worker", 15000m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 185, DateTimeKind.Utc).AddTicks(914), "dinesh" }
                });

            migrationBuilder.InsertData(
                table: "FuelRates",
                columns: new[] { "FuelRateId", "CreatedAt", "EffectiveFrom", "EffectiveTo", "FuelTypeId", "Rate", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9307), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(7926), null, new Guid("77777777-7777-7777-7777-777777777777"), 102.50m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9309), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9327), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9324), null, new Guid("88888888-8888-8888-8888-888888888888"), 89.75m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9328), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9334), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9333), null, new Guid("99999999-9999-9999-9999-999999999999"), 75.00m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9335), new Guid("33333333-3333-3333-3333-333333333333") }
                });

            migrationBuilder.InsertData(
                table: "Nozzles",
                columns: new[] { "NozzleId", "CreatedAt", "CurrentMeterReading", "FuelTypeId", "IsActive", "MachineId", "NozzleName", "NozzleNumber", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6421), 23456.789m, new Guid("88888888-8888-8888-8888-888888888888"), true, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Diesel Nozzle 1", "N2", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6422) },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6428), 18765.432m, new Guid("77777777-7777-7777-7777-777777777777"), true, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Petrol Nozzle 2", "N1", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6428) },
                    { new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6434), 5678.123m, new Guid("99999999-9999-9999-9999-999999999999"), true, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "CNG Nozzle 1", "N2", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6435) },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6405), 15234.567m, new Guid("77777777-7777-7777-7777-777777777777"), true, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Petrol Nozzle 1", "N1", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6406) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId",
                table: "CreditCustomers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId_CustomerCode",
                table: "CreditCustomers",
                columns: new[] { "TenantId", "CustomerCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId_IsActive",
                table: "CreditCustomers",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId_IsBlocked",
                table: "CreditCustomers",
                columns: new[] { "TenantId", "IsBlocked" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId_Phone",
                table: "CreditCustomers",
                columns: new[] { "TenantId", "Phone" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_CreditCustomerId",
                table: "CreditTransactions",
                column: "CreditCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_FuelSaleId",
                table: "CreditTransactions",
                column: "FuelSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_TenantId",
                table: "CreditTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_TransactionDate",
                table: "CreditTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_TransactionType",
                table: "CreditTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Category",
                table: "Expenses",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseDate",
                table: "Expenses",
                column: "ExpenseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_RecordedById",
                table: "Expenses",
                column: "RecordedById");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId",
                table: "Expenses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId_Category",
                table: "Expenses",
                columns: new[] { "TenantId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId_ExpenseDate",
                table: "Expenses",
                columns: new[] { "TenantId", "ExpenseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Features_FeatureCode",
                table: "Features",
                column: "FeatureCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelRates_FuelTypeId",
                table: "FuelRates",
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRates_FuelTypeId_EffectiveTo",
                table: "FuelRates",
                columns: new[] { "FuelTypeId", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_FuelRates_TenantId",
                table: "FuelRates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRates_UpdatedBy",
                table: "FuelRates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FuelSales_NozzleId",
                table: "FuelSales",
                column: "NozzleId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelSales_PaymentMethod",
                table: "FuelSales",
                column: "PaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_FuelSales_SaleTime",
                table: "FuelSales",
                column: "SaleTime");

            migrationBuilder.CreateIndex(
                name: "IX_FuelSales_ShiftId",
                table: "FuelSales",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelSales_TenantId",
                table: "FuelSales",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTypes_TenantId",
                table: "FuelTypes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTypes_TenantId_FuelCode",
                table: "FuelTypes",
                columns: new[] { "TenantId", "FuelCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Machines_TenantId",
                table: "Machines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_TenantId_IsActive",
                table: "Machines",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Machines_TenantId_MachineCode",
                table: "Machines",
                columns: new[] { "TenantId", "MachineCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nozzles_FuelTypeId",
                table: "Nozzles",
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Nozzles_MachineId",
                table: "Nozzles",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Nozzles_TenantId",
                table: "Nozzles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Nozzles_TenantId_MachineId_NozzleNumber",
                table: "Nozzles",
                columns: new[] { "TenantId", "MachineId", "NozzleNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanFeatures_FeatureId",
                table: "PlanFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanFeatures_SubscriptionPlan_FeatureId",
                table: "PlanFeatures",
                columns: new[] { "SubscriptionPlan", "FeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftNozzleReadings_NozzleId",
                table: "ShiftNozzleReadings",
                column: "NozzleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftNozzleReadings_ShiftId",
                table: "ShiftNozzleReadings",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftNozzleReadings_ShiftId_NozzleId",
                table: "ShiftNozzleReadings",
                columns: new[] { "ShiftId", "NozzleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftNozzleReadings_TenantId",
                table: "ShiftNozzleReadings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_MachineId",
                table: "Shifts",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_ShiftDate",
                table: "Shifts",
                column: "ShiftDate");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_Status",
                table: "Shifts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_TenantId",
                table: "Shifts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_WorkerId",
                table: "Shifts",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_EntryDate",
                table: "StockEntries",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_EntryType",
                table: "StockEntries",
                column: "EntryType");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_FuelSaleId",
                table: "StockEntries",
                column: "FuelSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_RecordedById",
                table: "StockEntries",
                column: "RecordedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_ShiftId",
                table: "StockEntries",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_TankId",
                table: "StockEntries",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_TenantId",
                table: "StockEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_TenantId_ShiftId",
                table: "StockEntries",
                columns: new[] { "TenantId", "ShiftId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_TenantId_TankId_EntryDate",
                table: "StockEntries",
                columns: new[] { "TenantId", "TankId", "EntryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemUsers_Email",
                table: "SystemUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemUsers_Username",
                table: "SystemUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tanks_FuelTypeId",
                table: "Tanks",
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tanks_TenantId",
                table: "Tanks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tanks_TenantId_FuelTypeId",
                table: "Tanks",
                columns: new[] { "TenantId", "FuelTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tanks_TenantId_IsActive",
                table: "Tanks",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Tanks_TenantId_TankCode",
                table: "Tanks",
                columns: new[] { "TenantId", "TankCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantFeatures_FeatureId",
                table: "TenantFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantFeatures_TenantId_FeatureId",
                table: "TenantFeatures",
                columns: new[] { "TenantId", "FeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Email",
                table: "Tenants",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_SubscriptionStatus_IsActive",
                table: "Tenants",
                columns: new[] { "SubscriptionStatus", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_TenantCode",
                table: "Tenants",
                column: "TenantCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_IsActive",
                table: "Users",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Phone",
                table: "Users",
                columns: new[] { "TenantId", "Phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Role",
                table: "Users",
                columns: new[] { "TenantId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Username",
                table: "Users",
                columns: new[] { "TenantId", "Username" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditTransactions");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "FuelRates");

            migrationBuilder.DropTable(
                name: "PlanFeatures");

            migrationBuilder.DropTable(
                name: "ShiftNozzleReadings");

            migrationBuilder.DropTable(
                name: "StockEntries");

            migrationBuilder.DropTable(
                name: "SystemUsers");

            migrationBuilder.DropTable(
                name: "TenantFeatures");

            migrationBuilder.DropTable(
                name: "CreditCustomers");

            migrationBuilder.DropTable(
                name: "FuelSales");

            migrationBuilder.DropTable(
                name: "Tanks");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "Nozzles");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "FuelTypes");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
