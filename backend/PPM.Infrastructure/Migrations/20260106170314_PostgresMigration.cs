using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PostgresMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 774, DateTimeKind.Utc).AddTicks(773), new DateTime(2026, 1, 6, 17, 3, 13, 774, DateTimeKind.Utc).AddTicks(977) });

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
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(3934), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(3935) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4819), new DateTime(2025, 7, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4199), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4819) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4861), new DateTime(2025, 10, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4859), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4862) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4875), new DateTime(2025, 11, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4874), new DateTime(2026, 1, 6, 17, 3, 13, 775, DateTimeKind.Utc).AddTicks(4876) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(965), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(9520), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(969) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(981), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(978), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(982) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(990), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(988), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(991) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7635), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7636) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7644), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7645) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7649), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7650) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 14, DateTimeKind.Utc).AddTicks(1627), new DateTime(2026, 1, 6, 16, 51, 32, 14, DateTimeKind.Utc).AddTicks(1759) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 15, DateTimeKind.Utc).AddTicks(8453), new DateTime(2027, 1, 6, 16, 51, 32, 15, DateTimeKind.Utc).AddTicks(6394), new DateTime(2026, 1, 6, 16, 51, 32, 15, DateTimeKind.Utc).AddTicks(6119), new DateTime(2026, 1, 6, 16, 51, 32, 15, DateTimeKind.Utc).AddTicks(8455) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(2584), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(2585) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4696), new DateTime(2025, 7, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(3149), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4698) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4799), new DateTime(2025, 10, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4795), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4801) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4830), new DateTime(2025, 11, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4827), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4830) });
        }
    }
}
