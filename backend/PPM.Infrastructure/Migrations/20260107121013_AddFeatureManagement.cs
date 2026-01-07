using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureManagement : Migration
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

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "FeatureId", "CreatedAt", "Description", "DisplayOrder", "FeatureCode", "FeatureName", "Icon", "IsActive", "Module", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("f0000001-0000-0000-0000-000000000001"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(235), "Access to sales reports, dashboards, and analytics", 1, "REPORTS", "Reports & Analytics", "chart-bar", true, "Analytics", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(236) },
                    { new Guid("f0000002-0000-0000-0000-000000000002"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(251), "Manage credit customers and their balances", 2, "CREDIT_CUSTOMERS", "Credit Customer Management", "credit-card", true, "Sales", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(252) },
                    { new Guid("f0000003-0000-0000-0000-000000000003"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(254), "Track and manage business expenses", 3, "EXPENSES", "Expense Tracking", "receipt", true, "Finance", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(255) },
                    { new Guid("f0000004-0000-0000-0000-000000000004"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(257), "Support for multiple shifts per day", 4, "MULTI_SHIFT", "Multiple Shifts", "clock", true, "Operations", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(257) },
                    { new Guid("f0000005-0000-0000-0000-000000000005"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(259), "Export data to Excel and PDF", 5, "EXPORT", "Data Export", "download", true, "Utilities", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(259) },
                    { new Guid("f0000006-0000-0000-0000-000000000006"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(261), "Programmatic API access for integrations", 6, "API_ACCESS", "API Access", "code", true, "Integration", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(262) },
                    { new Guid("f0000007-0000-0000-0000-000000000007"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(263), "Advanced analytics and custom reports", 7, "ADVANCED_REPORTS", "Advanced Reports", "chart-pie", true, "Analytics", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(264) },
                    { new Guid("f0000008-0000-0000-0000-000000000008"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(266), "Bulk import/export and batch operations", 8, "BULK_OPERATIONS", "Bulk Operations", "database", true, "Utilities", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(266) }
                });

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

            migrationBuilder.InsertData(
                table: "PlanFeatures",
                columns: new[] { "PlanFeatureId", "CreatedAt", "FeatureId", "IsEnabled", "SubscriptionPlan", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("ba000001-0000-0000-0000-000000000001"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1162), new Guid("f0000001-0000-0000-0000-000000000001"), false, "Basic", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1162) },
                    { new Guid("ba000002-0000-0000-0000-000000000002"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1166), new Guid("f0000002-0000-0000-0000-000000000002"), false, "Basic", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1167) },
                    { new Guid("ba000003-0000-0000-0000-000000000003"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1169), new Guid("f0000003-0000-0000-0000-000000000003"), false, "Basic", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1169) },
                    { new Guid("ba000004-0000-0000-0000-000000000004"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1171), new Guid("f0000004-0000-0000-0000-000000000004"), false, "Basic", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1171) },
                    { new Guid("ba000005-0000-0000-0000-000000000005"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1191), new Guid("f0000005-0000-0000-0000-000000000005"), false, "Basic", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1192) },
                    { new Guid("ba000006-0000-0000-0000-000000000006"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1194), new Guid("f0000006-0000-0000-0000-000000000006"), false, "Basic", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1194) },
                    { new Guid("ba000007-0000-0000-0000-000000000007"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1208), new Guid("f0000007-0000-0000-0000-000000000007"), false, "Basic", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1208) },
                    { new Guid("ba000008-0000-0000-0000-000000000008"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1210), new Guid("f0000008-0000-0000-0000-000000000008"), false, "Basic", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1210) },
                    { new Guid("ca000001-0000-0000-0000-000000000001"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1455), new Guid("f0000001-0000-0000-0000-000000000001"), true, "Premium", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1456) },
                    { new Guid("ca000002-0000-0000-0000-000000000002"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1458), new Guid("f0000002-0000-0000-0000-000000000002"), true, "Premium", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1458) },
                    { new Guid("ca000003-0000-0000-0000-000000000003"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1460), new Guid("f0000003-0000-0000-0000-000000000003"), true, "Premium", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1461) },
                    { new Guid("ca000004-0000-0000-0000-000000000004"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1462), new Guid("f0000004-0000-0000-0000-000000000004"), true, "Premium", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1463) },
                    { new Guid("ca000005-0000-0000-0000-000000000005"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1465), new Guid("f0000005-0000-0000-0000-000000000005"), true, "Premium", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1465) },
                    { new Guid("ca000006-0000-0000-0000-000000000006"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1467), new Guid("f0000006-0000-0000-0000-000000000006"), false, "Premium", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1467) },
                    { new Guid("ca000007-0000-0000-0000-000000000007"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1469), new Guid("f0000007-0000-0000-0000-000000000007"), false, "Premium", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1470) },
                    { new Guid("ca000008-0000-0000-0000-000000000008"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1471), new Guid("f0000008-0000-0000-0000-000000000008"), false, "Premium", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1472) },
                    { new Guid("da000001-0000-0000-0000-000000000001"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1511), new Guid("f0000001-0000-0000-0000-000000000001"), true, "Enterprise", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1511) },
                    { new Guid("da000002-0000-0000-0000-000000000002"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1513), new Guid("f0000002-0000-0000-0000-000000000002"), true, "Enterprise", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1513) },
                    { new Guid("da000003-0000-0000-0000-000000000003"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1515), new Guid("f0000003-0000-0000-0000-000000000003"), true, "Enterprise", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1516) },
                    { new Guid("da000004-0000-0000-0000-000000000004"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1518), new Guid("f0000004-0000-0000-0000-000000000004"), true, "Enterprise", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1518) },
                    { new Guid("da000005-0000-0000-0000-000000000005"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1520), new Guid("f0000005-0000-0000-0000-000000000005"), true, "Enterprise", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1520) },
                    { new Guid("da000006-0000-0000-0000-000000000006"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1522), new Guid("f0000006-0000-0000-0000-000000000006"), true, "Enterprise", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1522) },
                    { new Guid("da000007-0000-0000-0000-000000000007"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1541), new Guid("f0000007-0000-0000-0000-000000000007"), true, "Enterprise", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1541) },
                    { new Guid("da000008-0000-0000-0000-000000000008"), new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1543), new Guid("f0000008-0000-0000-0000-000000000008"), true, "Enterprise", new DateTime(2026, 1, 7, 12, 10, 10, 974, DateTimeKind.Utc).AddTicks(1543) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Features_FeatureCode",
                table: "Features",
                column: "FeatureCode",
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
                name: "IX_TenantFeatures_FeatureId",
                table: "TenantFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantFeatures_TenantId_FeatureId",
                table: "TenantFeatures",
                columns: new[] { "TenantId", "FeatureId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanFeatures");

            migrationBuilder.DropTable(
                name: "TenantFeatures");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(4939), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(4321), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(4940) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(4956), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(4955), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(4957) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(4960), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(4959), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(3052), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(3053) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(3059), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(3059) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(3061), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(3062) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(7213), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(7213) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(7222), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(7222) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(8577), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(8578) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(8583), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(8583) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(8586), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(8586) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(8568), new DateTime(2026, 1, 7, 11, 2, 0, 960, DateTimeKind.Utc).AddTicks(8569) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 204, DateTimeKind.Utc).AddTicks(424), "$2a$11$NYENG6POfhBTnsD2xom5ZupuJKj4DkOzt0fjUMLtKnC4810Xh8hD6", new DateTime(2026, 1, 7, 11, 2, 0, 204, DateTimeKind.Utc).AddTicks(635) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 205, DateTimeKind.Utc).AddTicks(1130), new DateTime(2027, 1, 7, 11, 2, 0, 205, DateTimeKind.Utc).AddTicks(387), new DateTime(2026, 1, 7, 11, 2, 0, 205, DateTimeKind.Utc).AddTicks(234), new DateTime(2026, 1, 7, 11, 2, 0, 205, DateTimeKind.Utc).AddTicks(1131) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 447, DateTimeKind.Utc).AddTicks(8975), "$2a$11$RN2oJCqQBI.LNiM22kYxU.Rx2bniyf3X6O2aJHfljOnoIER/GzAuC", new DateTime(2026, 1, 7, 11, 2, 0, 447, DateTimeKind.Utc).AddTicks(8982) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 689, DateTimeKind.Utc).AddTicks(36), new DateTime(2025, 7, 7, 11, 2, 0, 688, DateTimeKind.Utc).AddTicks(8829), "$2a$11$oYJZ1QHYArvOOLf/wVFXTepuFpdzbzxgdfchOW7bXh4TjbARQ2oiG", new DateTime(2026, 1, 7, 11, 2, 0, 689, DateTimeKind.Utc).AddTicks(38) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 824, DateTimeKind.Utc).AddTicks(7026), new DateTime(2025, 10, 7, 11, 2, 0, 824, DateTimeKind.Utc).AddTicks(7006), "$2a$11$b3ofLz/S6heZW6x.vJlWhO98aZXhht65iPcKgJyuasjbFlOCTw3Fu", new DateTime(2026, 1, 7, 11, 2, 0, 824, DateTimeKind.Utc).AddTicks(7028) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 11, 2, 0, 959, DateTimeKind.Utc).AddTicks(9224), new DateTime(2025, 11, 7, 11, 2, 0, 959, DateTimeKind.Utc).AddTicks(8882), "$2a$11$8PQ25iaqgOXwd0laXeP3yO5dYbBJ3TmshLVcG3f7BATM9wH0vuMPy", new DateTime(2026, 1, 7, 11, 2, 0, 959, DateTimeKind.Utc).AddTicks(9231) });
        }
    }
}
