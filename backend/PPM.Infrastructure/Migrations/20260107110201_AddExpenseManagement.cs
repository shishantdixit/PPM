using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(857), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(180), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(858) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(866), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(865), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(866) });

            migrationBuilder.UpdateData(
                table: "FuelRates",
                keyColumn: "FuelRateId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "EffectiveFrom", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(871), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(870), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(872) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 369, DateTimeKind.Utc).AddTicks(8061), new DateTime(2026, 1, 7, 10, 28, 32, 369, DateTimeKind.Utc).AddTicks(8067) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 369, DateTimeKind.Utc).AddTicks(8085), new DateTime(2026, 1, 7, 10, 28, 32, 369, DateTimeKind.Utc).AddTicks(8085) });

            migrationBuilder.UpdateData(
                table: "FuelTypes",
                keyColumn: "FuelTypeId",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 369, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 1, 7, 10, 28, 32, 369, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(3380), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(3381) });

            migrationBuilder.UpdateData(
                table: "Machines",
                keyColumn: "MachineId",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(3389), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(3389) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(4923), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(4923) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(4930), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(4931) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(4933), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(4934) });

            migrationBuilder.UpdateData(
                table: "Nozzles",
                keyColumn: "NozzleId",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(4915), new DateTime(2026, 1, 7, 10, 28, 32, 370, DateTimeKind.Utc).AddTicks(4915) });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "SystemUserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 31, 631, DateTimeKind.Utc).AddTicks(5883), "$2a$11$5ExNDR1.imDNK0JU4E9b.eJ39/dyZ1bAahj6g48tbwYcoHOUbepka", new DateTime(2026, 1, 7, 10, 28, 31, 631, DateTimeKind.Utc).AddTicks(6139) });

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "SubscriptionEndDate", "SubscriptionStartDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 31, 632, DateTimeKind.Utc).AddTicks(7415), new DateTime(2027, 1, 7, 10, 28, 31, 632, DateTimeKind.Utc).AddTicks(6584), new DateTime(2026, 1, 7, 10, 28, 31, 632, DateTimeKind.Utc).AddTicks(6429), new DateTime(2026, 1, 7, 10, 28, 31, 632, DateTimeKind.Utc).AddTicks(7416) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 31, 879, DateTimeKind.Utc).AddTicks(1238), "$2a$11$N0/RFjY98NmdJ9gENZ2Aqu.0jOQK5c5jXGJDGWouTmyqILqI1/Npy", new DateTime(2026, 1, 7, 10, 28, 31, 879, DateTimeKind.Utc).AddTicks(1245) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 113, DateTimeKind.Utc).AddTicks(2509), new DateTime(2025, 7, 7, 10, 28, 32, 113, DateTimeKind.Utc).AddTicks(1614), "$2a$11$gu0RDxuNJfvK9w4uMZWxhOeojhKJxg5zqZ0UIfNSX5wLPuD9lyIX2", new DateTime(2026, 1, 7, 10, 28, 32, 113, DateTimeKind.Utc).AddTicks(2514) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 241, DateTimeKind.Utc).AddTicks(1896), new DateTime(2025, 10, 7, 10, 28, 32, 241, DateTimeKind.Utc).AddTicks(1873), "$2a$11$MXNYsW28gthtF5nXPMSnd.ETr1jQRjfI1Bodm480M9KM8EUrZ5cSm", new DateTime(2026, 1, 7, 10, 28, 32, 241, DateTimeKind.Utc).AddTicks(1899) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "DateOfJoining", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 7, 10, 28, 32, 369, DateTimeKind.Utc).AddTicks(409), new DateTime(2025, 11, 7, 10, 28, 32, 369, DateTimeKind.Utc).AddTicks(380), "$2a$11$Bqrk8ysp1MgECF5iJLXwjeCA7JtwWLlqkJwLfNXPuwZNiPEDUYIpG", new DateTime(2026, 1, 7, 10, 28, 32, 369, DateTimeKind.Utc).AddTicks(413) });
        }
    }
}
