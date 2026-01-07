using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFuelSaleSaleNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SaleNumber",
                table: "FuelSales",
                type: "text",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaleNumber",
                table: "FuelSales");

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
        }
    }
}
