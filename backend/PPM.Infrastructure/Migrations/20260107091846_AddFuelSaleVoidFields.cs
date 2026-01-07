using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFuelSaleVoidFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "FuelSales",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VoidReason",
                table: "FuelSales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VoidedAt",
                table: "FuelSales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoidedBy",
                table: "FuelSales",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(3001), new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(1686), new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(3002) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(3014), new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(3012), new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(3015) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(3021), new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(3020), new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(3022) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 103, DateTimeKind.Utc).AddTicks(9112), new DateTime(2026, 1, 7, 9, 18, 45, 103, DateTimeKind.Utc).AddTicks(9117) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 103, DateTimeKind.Utc).AddTicks(9138), new DateTime(2026, 1, 7, 9, 18, 45, 103, DateTimeKind.Utc).AddTicks(9139) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 103, DateTimeKind.Utc).AddTicks(9144), new DateTime(2026, 1, 7, 9, 18, 45, 103, DateTimeKind.Utc).AddTicks(9145) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(7891), new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(7894) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(7910), new DateTime(2026, 1, 7, 9, 18, 45, 104, DateTimeKind.Utc).AddTicks(7910) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 105, DateTimeKind.Utc).AddTicks(742), new DateTime(2026, 1, 7, 9, 18, 45, 105, DateTimeKind.Utc).AddTicks(743) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 105, DateTimeKind.Utc).AddTicks(748), new DateTime(2026, 1, 7, 9, 18, 45, 105, DateTimeKind.Utc).AddTicks(749) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 105, DateTimeKind.Utc).AddTicks(754), new DateTime(2026, 1, 7, 9, 18, 45, 105, DateTimeKind.Utc).AddTicks(755) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 105, DateTimeKind.Utc).AddTicks(726), new DateTime(2026, 1, 7, 9, 18, 45, 105, DateTimeKind.Utc).AddTicks(727) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 44, 129, DateTimeKind.Utc).AddTicks(990), "$2a$11$CSfDEAon9YSY4lqrykNnuOVSaMG.sBE2Oz1BDwOlj8U7Gj4Yn50Ua", new DateTime(2026, 1, 7, 9, 18, 44, 129, DateTimeKind.Utc).AddTicks(1383) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 44, 131, DateTimeKind.Utc).AddTicks(2233), new DateTime(2027, 1, 7, 9, 18, 44, 130, DateTimeKind.Utc).AddTicks(5448), new DateTime(2026, 1, 7, 9, 18, 44, 130, DateTimeKind.Utc).AddTicks(5247), new DateTime(2026, 1, 7, 9, 18, 44, 131, DateTimeKind.Utc).AddTicks(2238) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 44, 492, DateTimeKind.Utc).AddTicks(6727), "$2a$11$.ER1XS/YHC5CnYYuvidWkesOf01iRKKkFQYhv/W43MGPFnULGjDOa", new DateTime(2026, 1, 7, 9, 18, 44, 492, DateTimeKind.Utc).AddTicks(6741) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 44, 702, DateTimeKind.Utc).AddTicks(1945), new DateTime(2025, 7, 7, 9, 18, 44, 701, DateTimeKind.Utc).AddTicks(9913), "$2a$11$GJHSUJXzY4RDTSsoBC7zZO8lJXrFjuliB3uCbwc8yjxiDDQWyiwMG", new DateTime(2026, 1, 7, 9, 18, 44, 702, DateTimeKind.Utc).AddTicks(1950) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 44, 916, DateTimeKind.Utc).AddTicks(6242), new DateTime(2025, 10, 7, 9, 18, 44, 916, DateTimeKind.Utc).AddTicks(6127), "$2a$11$TZ5OyOrIs1ww.ggz2HGpzuCw0X33zOx70aJMHUUqeMs0Sxe5psPPa", new DateTime(2026, 1, 7, 9, 18, 44, 916, DateTimeKind.Utc).AddTicks(6248) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 18, 45, 102, DateTimeKind.Utc).AddTicks(9531), new DateTime(2025, 11, 7, 9, 18, 45, 102, DateTimeKind.Utc).AddTicks(9502), "$2a$11$SgEpQSqMfUNmfPIkLqUdFODnFI0i8puTKTeRaqUwalUKH29N1NQ6.", new DateTime(2026, 1, 7, 9, 18, 45, 102, DateTimeKind.Utc).AddTicks(9536) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "FuelSales");

            migrationBuilder.DropColumn(
                name: "VoidReason",
                table: "FuelSales");

            migrationBuilder.DropColumn(
                name: "VoidedAt",
                table: "FuelSales");

            migrationBuilder.DropColumn(
                name: "VoidedBy",
                table: "FuelSales");

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(7464), new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(6595), new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(7465) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(7475), new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(7473), new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(7475) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(7481), new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(7480), new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(7482) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(5029), new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(5032) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(5040), new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(5041) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(5044), new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(5045) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(886), new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(888) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(900), new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(901) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(3008), new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(3009) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(3013), new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(3014) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(3021), new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(3022) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(2999), new DateTime(2026, 1, 7, 9, 12, 3, 874, DateTimeKind.Utc).AddTicks(2999) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 32, DateTimeKind.Utc).AddTicks(136), "$2a$11$A0adTksJaO52lofIhLmyLe4pLf9zp7GFN83MpdORveBJrohzSzuZ2", new DateTime(2026, 1, 7, 9, 12, 3, 32, DateTimeKind.Utc).AddTicks(393) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 33, DateTimeKind.Utc).AddTicks(7620), new DateTime(2027, 1, 7, 9, 12, 3, 33, DateTimeKind.Utc).AddTicks(2491), new DateTime(2026, 1, 7, 9, 12, 3, 33, DateTimeKind.Utc).AddTicks(2292), new DateTime(2026, 1, 7, 9, 12, 3, 33, DateTimeKind.Utc).AddTicks(7623) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 362, DateTimeKind.Utc).AddTicks(115), "$2a$11$i01tmisyQTOXGDlFmnM4KeMSUKmwzhuNVE7LJUdEmwsmZc/BopJN2", new DateTime(2026, 1, 7, 9, 12, 3, 362, DateTimeKind.Utc).AddTicks(124) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 540, DateTimeKind.Utc).AddTicks(4827), new DateTime(2025, 7, 7, 9, 12, 3, 540, DateTimeKind.Utc).AddTicks(3559), "$2a$11$A1JADSlqDTIG5FkXJBZC1.mtViK.QDRMGVcD0A9gDlAvH9oBa3rGu", new DateTime(2026, 1, 7, 9, 12, 3, 540, DateTimeKind.Utc).AddTicks(4831) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 714, DateTimeKind.Utc).AddTicks(5950), new DateTime(2025, 10, 7, 9, 12, 3, 714, DateTimeKind.Utc).AddTicks(5927), "$2a$11$K1TxYPaZxg6s6lCJA9tgVuhkADAAZ9wHx0I9jmNILmQTwE7js3RIq", new DateTime(2026, 1, 7, 9, 12, 3, 714, DateTimeKind.Utc).AddTicks(5954) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(488), new DateTime(2025, 11, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(459), "$2a$11$g..9u77q9c4Px3buB.6RcuDZxaGECL6RntC/D7SPYf4zAZ/rPBTM6", new DateTime(2026, 1, 7, 9, 12, 3, 873, DateTimeKind.Utc).AddTicks(491) });
        }
    }
}
