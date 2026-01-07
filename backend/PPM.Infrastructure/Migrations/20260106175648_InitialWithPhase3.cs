using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialWithPhase3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "MachineId", "CreatedAt", "InstallationDate", "IsActive", "Location", "MachineCode", "MachineName", "Manufacturer", "Model", "SerialNumber", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(600), new DateOnly(2023, 1, 15), true, "Left Side", "M001", "Machine 1", "Tokheim", "Premier B", "SRL-M1-2023", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(601) },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(610), new DateOnly(2023, 3, 20), true, "Right Side", "M002", "Machine 2", "Wayne", "Helix 6000", "SRL-M2-2023", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(610) }
                });

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

            migrationBuilder.InsertData(
                table: "Nozzles",
                columns: new[] { "NozzleId", "CreatedAt", "CurrentMeterReading", "FuelTypeId", "IsActive", "MachineId", "NozzleName", "NozzleNumber", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2026), 23456.789m, new Guid("88888888-8888-8888-8888-888888888888"), true, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Diesel Nozzle 1", "N2", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2026) },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2029), 18765.432m, new Guid("77777777-7777-7777-7777-777777777777"), true, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Petrol Nozzle 2", "N1", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2029) },
                    { new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2032), 5678.123m, new Guid("99999999-9999-9999-9999-999999999999"), true, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "CNG Nozzle 1", "N2", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2033) },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2019), 15234.567m, new Guid("77777777-7777-7777-7777-777777777777"), true, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Petrol Nozzle 1", "N1", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 17, 56, 47, 896, DateTimeKind.Utc).AddTicks(2019) }
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nozzles");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(7519), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(6974), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(7520) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(7549), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(7548), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(7550) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(7553), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(7552), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(7553) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(6200), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(6201) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(6204), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(6205) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(6207), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(6207) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 774, DateTimeKind.Utc).AddTicks(773), "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", new DateTime(2026, 1, 6, 17, 3, 13, 774, DateTimeKind.Utc).AddTicks(977) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(2165), new DateTime(2027, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(1359), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(1223), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(2166) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(3934), "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(3935) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4819), new DateTime(2025, 7, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4199), "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4819) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4861), new DateTime(2025, 10, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4859), "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4862) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4875), new DateTime(2025, 11, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4874), "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4876) });
        }
    }
}
