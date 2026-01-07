using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockAndTankManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5137), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5138) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5142), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5143) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5145), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5145) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5147), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5157) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5159), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5160) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5162), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5162) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5164), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5164) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5166), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(5166) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(1968), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(1400), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(1969) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(1984), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(1984), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(1985) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(1989), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(1988), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(1989) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(53), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(53) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(58), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(59) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(114), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(115) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(4044), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(4045) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(4052), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(4052) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(5215), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(5215) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(5218), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(5218) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(5222), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(5222) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(5210), new DateTime(2026, 1, 7, 13, 25, 3, 581, DateTimeKind.Utc).AddTicks(5211) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6011), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6012) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6015), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6015) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6025), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6025) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6028), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6028) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6031) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6033), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6033) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6038), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6038) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6040), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6040) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6219), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6219) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6222), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6222) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6224), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6224) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6226), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6226) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6228), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6229) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6231), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6231) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6233), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6233) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6235), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6235) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6316), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6317) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6319), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6319) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6321), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6321) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6323), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6323) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6325), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6326) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6328), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6328) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6330), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6330) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6332), new DateTime(2026, 1, 7, 13, 25, 3, 582, DateTimeKind.Utc).AddTicks(6332) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 2, 977, DateTimeKind.Utc).AddTicks(5621), "$2a$11$n9m3VPRU39IIx0zi.lE8pef1HCaQNam90oKoB.n4SICq1J91naxpy", new DateTime(2026, 1, 7, 13, 25, 2, 977, DateTimeKind.Utc).AddTicks(5816) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 2, 978, DateTimeKind.Utc).AddTicks(5069), new DateTime(2027, 1, 7, 13, 25, 2, 978, DateTimeKind.Utc).AddTicks(4356), new DateTime(2026, 1, 7, 13, 25, 2, 978, DateTimeKind.Utc).AddTicks(4234), new DateTime(2026, 1, 7, 13, 25, 2, 978, DateTimeKind.Utc).AddTicks(5069) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 162, DateTimeKind.Utc).AddTicks(8633), "$2a$11$staUaX7i32owuN1lj5mAiOknH7dEggWPGbyH6nysIedzUIMFoa/ci", new DateTime(2026, 1, 7, 13, 25, 3, 162, DateTimeKind.Utc).AddTicks(8640) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 342, DateTimeKind.Utc).AddTicks(1375), new DateTime(2025, 7, 7, 13, 25, 3, 342, DateTimeKind.Utc).AddTicks(289), "$2a$11$xcTsf5MnkpcePT.WrwgAkO./SZOmpmlK34Y4lwdT77tlOvWqFWtCO", new DateTime(2026, 1, 7, 13, 25, 3, 342, DateTimeKind.Utc).AddTicks(1378) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 461, DateTimeKind.Utc).AddTicks(3850), new DateTime(2025, 10, 7, 13, 25, 3, 461, DateTimeKind.Utc).AddTicks(3832), "$2a$11$InO1cm/9RDLRY3sfjwQzHutK9Vc/h.9tqheDSsBvZAc4mOC5HHb8G", new DateTime(2026, 1, 7, 13, 25, 3, 461, DateTimeKind.Utc).AddTicks(3852) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 13, 25, 3, 580, DateTimeKind.Utc).AddTicks(7039), new DateTime(2025, 11, 7, 13, 25, 3, 580, DateTimeKind.Utc).AddTicks(7015), "$2a$11$F1FiGiBPx0h6Sfu8gRs5o.VRPUyKQDI7SZZvXjrbnYhuWss1Ultmy", new DateTime(2026, 1, 7, 13, 25, 3, 580, DateTimeKind.Utc).AddTicks(7042) });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockEntries");

            migrationBuilder.DropTable(
                name: "Tanks");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(235), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(236) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(251), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(252) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(254), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(255) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(257), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(257) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(259), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(259) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(261), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(262) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(263), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(264) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(266), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(266) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(7249), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(6684), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(7251) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(7280), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(7278), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(7280) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(7283), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(7283), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(7284) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(5404), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(5405) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(5415), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(5415) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(5418), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(5418) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(9433), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(9435) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(9443), new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(9443) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 973, DateTimeKind.Utc).AddTicks(722), new DateTime(2026, 1, 7, 12, 10, 10, 973, DateTimeKind.Utc).AddTicks(723) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 973, DateTimeKind.Utc).AddTicks(728), new DateTime(2026, 1, 7, 12, 10, 10, 973, DateTimeKind.Utc).AddTicks(728) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 973, DateTimeKind.Utc).AddTicks(731), new DateTime(2026, 1, 7, 12, 10, 10, 973, DateTimeKind.Utc).AddTicks(731) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 973, DateTimeKind.Utc).AddTicks(717), new DateTime(2026, 1, 7, 12, 10, 10, 973, DateTimeKind.Utc).AddTicks(717) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1162), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1162) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1166), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1167) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1169), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1169) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1171), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1171) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1191), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1192) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1194), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1194) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1208), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1208) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1210), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1210) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1455), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1456) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1458), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1458) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1460), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1461) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1462), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1463) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1465), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1465) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1467), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1467) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1469), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1470) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1471), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1472) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1511), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1511) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1513), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1513) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1515), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1516) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1518), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1518) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1520), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1520) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1522), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1522) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1541), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1541) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1543), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1543) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 453, DateTimeKind.Utc).AddTicks(5184), "$2a$11$bmlAvunMhIeYqri23AGxF.6Ou3AjRP05KIjxIz6Nh98C9e5AofGaO", new DateTime(2026, 1, 7, 12, 10, 10, 453, DateTimeKind.Utc).AddTicks(5437) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 454, DateTimeKind.Utc).AddTicks(6914), new DateTime(2027, 1, 7, 12, 10, 10, 454, DateTimeKind.Utc).AddTicks(6120), new DateTime(2026, 1, 7, 12, 10, 10, 454, DateTimeKind.Utc).AddTicks(6001), new DateTime(2026, 1, 7, 12, 10, 10, 454, DateTimeKind.Utc).AddTicks(6916) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 593, DateTimeKind.Utc).AddTicks(7535), "$2a$11$XXZj1Dlwf4IUnJGNh0u63ezI7DM/lNSm6MKXZz4jddyDp5QoXdceC", new DateTime(2026, 1, 7, 12, 10, 10, 593, DateTimeKind.Utc).AddTicks(7546) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 736, DateTimeKind.Utc).AddTicks(7186), new DateTime(2025, 7, 7, 12, 10, 10, 736, DateTimeKind.Utc).AddTicks(6078), "$2a$11$wVJPg/U/mk/GY72WStdzVOiHkoS1ebw.bXtvhasltHoYigl6wMr4O", new DateTime(2026, 1, 7, 12, 10, 10, 736, DateTimeKind.Utc).AddTicks(7188) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 853, DateTimeKind.Utc).AddTicks(5495), new DateTime(2025, 10, 7, 12, 10, 10, 853, DateTimeKind.Utc).AddTicks(5474), "$2a$11$15z8ExSsOdGx9gSbJMH9cubngJTs/RvxgAp.1xNJ2ssKCk09rNJ2m", new DateTime(2026, 1, 7, 12, 10, 10, 853, DateTimeKind.Utc).AddTicks(5497) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(247), new DateTime(2025, 11, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(225), "$2a$11$.tqSBRA29nq23c5XnT0xneTpHMV7T1/Zi4UNVrkT.hBablvAJ9wRW", new DateTime(2026, 1, 7, 12, 10, 10, 972, DateTimeKind.Utc).AddTicks(250) });
        }
    }
}
