using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMachineToShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete any existing shifts and related data since they don't have MachineId
            migrationBuilder.Sql("DELETE FROM \"ShiftNozzleReadings\"");
            migrationBuilder.Sql("DELETE FROM \"FuelSales\"");
            migrationBuilder.Sql("DELETE FROM \"Shifts\"");

            migrationBuilder.AddColumn<Guid>(
                name: "MachineId",
                table: "Shifts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9870), new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9873) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9886), new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9888) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9894), new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9895) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9900), new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9951) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9957), new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9958) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9963), new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9964) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9971), new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9972) });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: new Guid("f0000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9977), new DateTime(2026, 1, 7, 16, 13, 32, 656, DateTimeKind.Utc).AddTicks(9978) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(2128), new DateTime(2026, 1, 7, 16, 13, 32, 651, DateTimeKind.Utc).AddTicks(9851), new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(2131) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(2157), new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(2154), new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(2158) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(2163), new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(2162), new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(2163) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 651, DateTimeKind.Utc).AddTicks(4558), new DateTime(2026, 1, 7, 16, 13, 32, 651, DateTimeKind.Utc).AddTicks(4563) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 651, DateTimeKind.Utc).AddTicks(4676), new DateTime(2026, 1, 7, 16, 13, 32, 651, DateTimeKind.Utc).AddTicks(4677) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 651, DateTimeKind.Utc).AddTicks(4935), new DateTime(2026, 1, 7, 16, 13, 32, 651, DateTimeKind.Utc).AddTicks(4936) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(9779), new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(9783) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(9802), new DateTime(2026, 1, 7, 16, 13, 32, 652, DateTimeKind.Utc).AddTicks(9803) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 653, DateTimeKind.Utc).AddTicks(2824), new DateTime(2026, 1, 7, 16, 13, 32, 653, DateTimeKind.Utc).AddTicks(2825) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 653, DateTimeKind.Utc).AddTicks(2831), new DateTime(2026, 1, 7, 16, 13, 32, 653, DateTimeKind.Utc).AddTicks(2831) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 653, DateTimeKind.Utc).AddTicks(2837), new DateTime(2026, 1, 7, 16, 13, 32, 653, DateTimeKind.Utc).AddTicks(2837) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 653, DateTimeKind.Utc).AddTicks(2811), new DateTime(2026, 1, 7, 16, 13, 32, 653, DateTimeKind.Utc).AddTicks(2814) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3801), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3804) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3818), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3819) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3844), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3845) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3853), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3854) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3889), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3890) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3895), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3896) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3901), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3902) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ba000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3907), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(3908) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4477), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4479) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4486), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4487) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4492), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4493) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4499), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4500) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4505), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4506) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4511), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4512) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4518), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4519) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("ca000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4524), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4525) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000001-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4727), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4729) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000002-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4735), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4736) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000003-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4744), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4745) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000004-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4750), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4751) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000005-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4759), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4760) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000006-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4765), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4766) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000007-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4772), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4773) });

            migrationBuilder.UpdateData(
                table: "PlanFeatures",
                keyColumn: "PlanFeatureId",
                keyValue: new Guid("da000008-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4777), new DateTime(2026, 1, 7, 16, 13, 32, 657, DateTimeKind.Utc).AddTicks(4778) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 31, 620, DateTimeKind.Utc).AddTicks(6392), "$2a$11$1xPyPcZIPMjDwjstGcQpHuDeimgVnVpJeIXLnFs3GbLlLjttyOMoK", new DateTime(2026, 1, 7, 16, 13, 31, 620, DateTimeKind.Utc).AddTicks(6669) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 31, 622, DateTimeKind.Utc).AddTicks(6092), new DateTime(2027, 1, 7, 16, 13, 31, 621, DateTimeKind.Utc).AddTicks(9671), new DateTime(2026, 1, 7, 16, 13, 31, 621, DateTimeKind.Utc).AddTicks(9462), new DateTime(2026, 1, 7, 16, 13, 31, 622, DateTimeKind.Utc).AddTicks(6095) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 31, 879, DateTimeKind.Utc).AddTicks(4206), "$2a$11$hfbtYoaIydrnVX6WJtozvuhQZSDeOdYTA9XbSKPuxlIFTcZ2ivcnq", new DateTime(2026, 1, 7, 16, 13, 31, 879, DateTimeKind.Utc).AddTicks(4217) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 126, DateTimeKind.Utc).AddTicks(8096), new DateTime(2025, 7, 7, 16, 13, 32, 126, DateTimeKind.Utc).AddTicks(6955), "$2a$11$nqinP/zqa5JKQzg52WEn8uwpb2Oq67OPXLVgpXoax9HjblSdjTzau", new DateTime(2026, 1, 7, 16, 13, 32, 126, DateTimeKind.Utc).AddTicks(8103) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 386, DateTimeKind.Utc).AddTicks(1581), new DateTime(2025, 10, 7, 16, 13, 32, 386, DateTimeKind.Utc).AddTicks(1532), "$2a$11$TMSabOUR42sVEw6n8.pTTOmnrPk8u75RkgYpZqwq4KG3CNkxN/2Py", new DateTime(2026, 1, 7, 16, 13, 32, 386, DateTimeKind.Utc).AddTicks(1585) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 16, 13, 32, 650, DateTimeKind.Utc).AddTicks(4732), new DateTime(2025, 11, 7, 16, 13, 32, 650, DateTimeKind.Utc).AddTicks(4652), "$2a$11$VI0ZYIqMlofsW30v3AJNEuiDgGlc3bY6uSsSBLLvL8XR18662a3C2", new DateTime(2026, 1, 7, 16, 13, 32, 650, DateTimeKind.Utc).AddTicks(4738) });

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_MachineId",
                table: "Shifts",
                column: "MachineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Machines_MachineId",
                table: "Shifts",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "MachineId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Machines_MachineId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_MachineId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "MachineId",
                table: "Shifts");

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
        }
    }
}
