using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrialAndLicenseKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActiveLicenseKeyId",
                table: "Tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrial",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseActivatedAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialEndDate",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialStartDate",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LicenseKeys",
                columns: table => new
                {
                    LicenseKeyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SubscriptionPlan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DurationMonths = table.Column<int>(type: "integer", nullable: false),
                    MaxMachines = table.Column<int>(type: "integer", nullable: false),
                    MaxWorkers = table.Column<int>(type: "integer", nullable: false),
                    MaxMonthlyBills = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ActivatedByTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneratedBySystemUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseKeys", x => x.LicenseKeyId);
                    table.ForeignKey(
                        name: "FK_LicenseKeys_SystemUsers_GeneratedBySystemUserId",
                        column: x => x.GeneratedBySystemUserId,
                        principalTable: "SystemUsers",
                        principalColumn: "SystemUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LicenseKeys_Tenants_ActivatedByTenantId",
                        column: x => x.ActivatedByTenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(701), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(705) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(718), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(719) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(726), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(727) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(732), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(733) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(738), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(739) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(744), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(745) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(750), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(751) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(756), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(757) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(5407), new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(4239), new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(5408) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(5456), new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(5455), new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(5457) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(5463), new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(5462), new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(5464) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(1845), new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(1846) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(1854), new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(1855) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(1859), new DateTime(2026, 1, 21, 15, 47, 43, 172, DateTimeKind.Utc).AddTicks(1860) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 173, DateTimeKind.Utc).AddTicks(7752), new DateTime(2026, 1, 21, 15, 47, 43, 173, DateTimeKind.Utc).AddTicks(7755) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 173, DateTimeKind.Utc).AddTicks(7772), new DateTime(2026, 1, 21, 15, 47, 43, 173, DateTimeKind.Utc).AddTicks(7772) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 174, DateTimeKind.Utc).AddTicks(12), new DateTime(2026, 1, 21, 15, 47, 43, 174, DateTimeKind.Utc).AddTicks(13) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 174, DateTimeKind.Utc).AddTicks(18), new DateTime(2026, 1, 21, 15, 47, 43, 174, DateTimeKind.Utc).AddTicks(18) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 174, DateTimeKind.Utc).AddTicks(20), new DateTime(2026, 1, 21, 15, 47, 43, 174, DateTimeKind.Utc).AddTicks(21) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 174, DateTimeKind.Utc).AddTicks(3), new DateTime(2026, 1, 21, 15, 47, 43, 174, DateTimeKind.Utc).AddTicks(4) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4602), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4604) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4649), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4650) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4678), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4679) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4688), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4689) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4701), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4702) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4708), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4709) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4715), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4716) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4720), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(4721) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5206), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5208) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5214), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5215) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5220), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5221) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5226), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5227) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5232), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5233) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5238), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5239) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5244), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5245) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5250), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5251) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5360), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5362) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5367), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5367) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5373), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5374) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5379), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5380) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5385), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5386) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5391), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5392) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5397), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5398) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5402), new DateTime(2026, 1, 21, 15, 47, 43, 176, DateTimeKind.Utc).AddTicks(5403) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 42, 185, DateTimeKind.Utc).AddTicks(7069), "$2a$11$YAjIAPWsNAdUXG5h2weFge4Cl01SifTz6syI8.oOEBwZhRUcvjxDi", new DateTime(2026, 1, 21, 15, 47, 42, 185, DateTimeKind.Utc).AddTicks(7362) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "ActiveLicenseKeyId", "CreatedAt", "IsTrial", "LicenseActivatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "TrialEndDate", "TrialStartDate", "UpdatedAt" },
                values: new object[] { null, new DateTime(2026, 1, 21, 15, 47, 42, 187, DateTimeKind.Utc).AddTicks(8329), false, null, new DateTime(2027, 1, 21, 15, 47, 42, 187, DateTimeKind.Utc).AddTicks(1135), new DateTime(2026, 1, 21, 15, 47, 42, 187, DateTimeKind.Utc).AddTicks(979), null, null, new DateTime(2026, 1, 21, 15, 47, 42, 187, DateTimeKind.Utc).AddTicks(8334) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 42, 519, DateTimeKind.Utc).AddTicks(6301), "$2a$11$AakDGgOClvYuu/Wz1c9lkeUTKbZctd5Y.WI.fJSPg340.xoSgWWKC", new DateTime(2026, 1, 21, 15, 47, 42, 519, DateTimeKind.Utc).AddTicks(6312) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 42, 738, DateTimeKind.Utc).AddTicks(9459), new DateTime(2025, 7, 21, 15, 47, 42, 738, DateTimeKind.Utc).AddTicks(190), "$2a$11$fLS7gMPNG2J0SMO6W4NSI.vw8VeUpEzxgSsUd/yfTJv653LyMVmba", new DateTime(2026, 1, 21, 15, 47, 42, 738, DateTimeKind.Utc).AddTicks(9470) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 42, 942, DateTimeKind.Utc).AddTicks(7606), new DateTime(2025, 10, 21, 15, 47, 42, 942, DateTimeKind.Utc).AddTicks(7570), "$2a$11$X/1E7dtGcaDpPEpk.o3zjOmeRCAnmP.tbp.m2lErav2jM6.PLHsG2", new DateTime(2026, 1, 21, 15, 47, 42, 942, DateTimeKind.Utc).AddTicks(7609) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 47, 43, 171, DateTimeKind.Utc).AddTicks(5630), new DateTime(2025, 11, 21, 15, 47, 43, 171, DateTimeKind.Utc).AddTicks(5597), "$2a$11$bOcsRLG2sE/4pwXOt08yW.m2VotZlMPY8.Xj7LY2fKMkcz9YTxkGm", new DateTime(2026, 1, 21, 15, 47, 43, 171, DateTimeKind.Utc).AddTicks(5634) });

            migrationBuilder.CreateIndex(
                name: "IX_LicenseKeys_ActivatedByTenantId",
                table: "LicenseKeys",
                column: "ActivatedByTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseKeys_GeneratedBySystemUserId",
                table: "LicenseKeys",
                column: "GeneratedBySystemUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseKeys_Key",
                table: "LicenseKeys",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenseKeys_Status",
                table: "LicenseKeys",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicenseKeys");

            migrationBuilder.DropColumn(
                name: "ActiveLicenseKeyId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsTrial",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LicenseActivatedAt",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TrialEndDate",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TrialStartDate",
                table: "Tenants");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8044), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8047) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8055), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8055) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8059), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8060) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8063), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8064) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8067), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8068) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8071), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8072) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8093), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8094) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8097), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(8098) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9307), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(7926), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9309) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9327), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9324), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9328) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9334), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9333), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(9335) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1121), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1134) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1155), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1156) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1160), new DateTime(2026, 1, 9, 6, 10, 51, 186, DateTimeKind.Utc).AddTicks(1161) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(3848), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(3849) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(3865), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(3866) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6421), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6422) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6428), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6428) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6434), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6435) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6405), new DateTime(2026, 1, 9, 6, 10, 51, 187, DateTimeKind.Utc).AddTicks(6406) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9827), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9828) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9834), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9835) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9848), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9849) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9853), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9853) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9862), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9863) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9867), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9867) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9871), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9872) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9875), new DateTime(2026, 1, 9, 6, 10, 51, 189, DateTimeKind.Utc).AddTicks(9875) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(67), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(68) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(71), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(72) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(76), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(76) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(80), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(80) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(84), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(85) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(88), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(89) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(92), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(93) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(96), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(97) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(152), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(153) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(156), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(157) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(160), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(161) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(164), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(165) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(168), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(169) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(172), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(173) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(176), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(177) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(180), new DateTime(2026, 1, 9, 6, 10, 51, 190, DateTimeKind.Utc).AddTicks(181) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 50, 586, DateTimeKind.Utc).AddTicks(5110), "$2a$11$bV6OIM2Y8wUJST5L6yazSOQRjpUSFcjG/dvVDKiW76svIAbae033m", new DateTime(2026, 1, 9, 6, 10, 50, 586, DateTimeKind.Utc).AddTicks(5294) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 50, 587, DateTimeKind.Utc).AddTicks(8355), new DateTime(2027, 1, 9, 6, 10, 50, 587, DateTimeKind.Utc).AddTicks(5584), new DateTime(2026, 1, 9, 6, 10, 50, 587, DateTimeKind.Utc).AddTicks(5465), new DateTime(2026, 1, 9, 6, 10, 50, 587, DateTimeKind.Utc).AddTicks(8357) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 50, 770, DateTimeKind.Utc).AddTicks(7584), "$2a$11$jtnPnZLbP04J9085SWqQdurF2UA/7a9VCre34UUOg5bd5u3IlLavu", new DateTime(2026, 1, 9, 6, 10, 50, 770, DateTimeKind.Utc).AddTicks(7595) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 50, 935, DateTimeKind.Utc).AddTicks(3278), new DateTime(2025, 7, 9, 6, 10, 50, 935, DateTimeKind.Utc).AddTicks(2213), "$2a$11$lsXspL/8haOyyySgQ8.ZgeLRiVkswrG/V.JQQPuA.aneJVQvv8Kuu", new DateTime(2026, 1, 9, 6, 10, 50, 935, DateTimeKind.Utc).AddTicks(3281) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 61, DateTimeKind.Utc).AddTicks(3901), new DateTime(2025, 10, 9, 6, 10, 51, 61, DateTimeKind.Utc).AddTicks(3874), "$2a$11$tvtbMwtMwyuhaAbGbRgzQunCS7h.y1CGu0uVUSz5rfsQfyrXHc9hO", new DateTime(2026, 1, 9, 6, 10, 51, 61, DateTimeKind.Utc).AddTicks(3904) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 9, 6, 10, 51, 185, DateTimeKind.Utc).AddTicks(911), new DateTime(2025, 11, 9, 6, 10, 51, 185, DateTimeKind.Utc).AddTicks(885), "$2a$11$48bo9mIxLIwW6cptXE7DYukH1Y4gbFczWk3SwvTM2ZvQzuL6fCeZK", new DateTime(2026, 1, 9, 6, 10, 51, 185, DateTimeKind.Utc).AddTicks(914) });
        }
    }
}
