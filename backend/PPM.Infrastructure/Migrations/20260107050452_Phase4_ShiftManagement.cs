using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase4_ShiftManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CustomerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VehicleNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SaleTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(9108), new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(8313), new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(9109) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(9117), new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(9115), new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(9117) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(9121), new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(9120), new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(9122) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(6771), new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(6772) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(6778), new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(6778) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(6781), new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(6782) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(1573), new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(1578) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(1614), new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(1615) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(4899), new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(4900) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(4907), new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(4908) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(4913), new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(4914) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(4888), new DateTime(2026, 1, 7, 5, 4, 49, 265, DateTimeKind.Utc).AddTicks(4890) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 48, 655, DateTimeKind.Utc).AddTicks(6212), "$2a$11$kKpH0t2rSKtJCnXC1Ztf.e1vnGiHjEHax8VH1BqoTZ6AzqTkdwuR2", new DateTime(2026, 1, 7, 5, 4, 48, 655, DateTimeKind.Utc).AddTicks(6408) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 48, 657, DateTimeKind.Utc).AddTicks(783), new DateTime(2027, 1, 7, 5, 4, 48, 656, DateTimeKind.Utc).AddTicks(6272), new DateTime(2026, 1, 7, 5, 4, 48, 656, DateTimeKind.Utc).AddTicks(6143), new DateTime(2026, 1, 7, 5, 4, 48, 657, DateTimeKind.Utc).AddTicks(786) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 48, 844, DateTimeKind.Utc).AddTicks(7940), "$2a$11$los/Rp2srhknJ0T/f8pe4Ost5MnxY2knCpOyh5kCKPLyNAYBXpd3e", new DateTime(2026, 1, 7, 5, 4, 48, 844, DateTimeKind.Utc).AddTicks(7946) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 20, DateTimeKind.Utc).AddTicks(3944), new DateTime(2025, 7, 7, 5, 4, 49, 20, DateTimeKind.Utc).AddTicks(2981), "$2a$11$ooYuZsfUX9n53nphE2iJeuoOL7GqYZzcamjKjy3S/4orQWghMZXYu", new DateTime(2026, 1, 7, 5, 4, 49, 20, DateTimeKind.Utc).AddTicks(3947) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 136, DateTimeKind.Utc).AddTicks(9266), new DateTime(2025, 10, 7, 5, 4, 49, 136, DateTimeKind.Utc).AddTicks(9248), "$2a$11$E3kFNeLup.pz/PRqUj8pm.wRzDHhVBAKfJ8v9lSA04AJexsl4q4M6", new DateTime(2026, 1, 7, 5, 4, 49, 136, DateTimeKind.Utc).AddTicks(9268) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(3206), new DateTime(2025, 11, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(3187), "$2a$11$J1FVmXbMwus3QoGnW9bueO1flIrnC/8CZ9gsgNR2RxHxA1GDp0./i", new DateTime(2026, 1, 7, 5, 4, 49, 257, DateTimeKind.Utc).AddTicks(3208) });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelSales");

            migrationBuilder.DropTable(
                name: "ShiftNozzleReadings");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(7859), new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(7149), new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(7860) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(7866), new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(7865), new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(7867) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(7871), new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(7870), new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(7871) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(5826), new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(5828) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(5835), new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(5835) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(5837), new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(5838) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(600), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(601) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(610), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(610) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2026), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2026) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2029), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2029) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2032), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2033) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2019), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2019) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 118, DateTimeKind.Utc).AddTicks(7596), "$2a$11$IAZW0nQU4pHQcssG0pdgxuXNhsRTRaHFe7hVC9lJQwwKC.XbZ1KqW", new DateTime(2026, 1, 6, 17, 56, 47, 118, DateTimeKind.Utc).AddTicks(7847) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 119, DateTimeKind.Utc).AddTicks(9982), new DateTime(2027, 1, 6, 17, 56, 47, 119, DateTimeKind.Utc).AddTicks(9105), new DateTime(2026, 1, 6, 17, 56, 47, 119, DateTimeKind.Utc).AddTicks(8947), new DateTime(2026, 1, 6, 17, 56, 47, 119, DateTimeKind.Utc).AddTicks(9983) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 374, DateTimeKind.Utc).AddTicks(6210), "$2a$11$X/9IBFn0Tmn6SqVVqmsSl.LPU3CCwXRgacwz696HToBhi1SA4wckW", new DateTime(2026, 1, 6, 17, 56, 47, 374, DateTimeKind.Utc).AddTicks(6219) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 616, DateTimeKind.Utc).AddTicks(6813), new DateTime(2025, 7, 6, 17, 56, 47, 616, DateTimeKind.Utc).AddTicks(5643), "$2a$11$erBYpjJblLycx54Poslw5ukpuTAVuVMXwUq3K2PaUx8exyLm5cPvy", new DateTime(2026, 1, 6, 17, 56, 47, 616, DateTimeKind.Utc).AddTicks(6817) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 754, DateTimeKind.Utc).AddTicks(8492), new DateTime(2025, 10, 6, 17, 56, 47, 754, DateTimeKind.Utc).AddTicks(8471), "$2a$11$gp0JbMF9CJccBTCTXd84AutJsRg/f5Kp7pE1PRHjZQOxAXS8zx95i", new DateTime(2026, 1, 6, 17, 56, 47, 754, DateTimeKind.Utc).AddTicks(8496) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(2188), new DateTime(2025, 11, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(2053), "$2a$11$16p/gZ1czTJyC208WL/QtOGMdr19vxPqr6B.0m3S3fpQb/1K8Q4LS", new DateTime(2026, 1, 6, 17, 56, 47, 895, DateTimeKind.Utc).AddTicks(2193) });
        }
    }
}
