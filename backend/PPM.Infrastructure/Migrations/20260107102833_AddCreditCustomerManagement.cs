using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCustomerManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditCustomers",
                columns: table => new
                {
                    CreditCustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentTermDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    BlockReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VehicleNumbers = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCustomers", x => x.CreditCustomerId);
                    table.ForeignKey(
                        name: "FK_CreditCustomers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditTransactions",
                columns: table => new
                {
                    CreditTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FuelSaleId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentMode = table.Column<int>(type: "integer", nullable: true),
                    PaymentReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTransactions", x => x.CreditTransactionId);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_CreditCustomers_CreditCustomerId",
                        column: x => x.CreditCustomerId,
                        principalTable: "CreditCustomers",
                        principalColumn: "CreditCustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_FuelSales_FuelSaleId",
                        column: x => x.FuelSaleId,
                        principalTable: "FuelSales",
                        principalColumn: "FuelSaleId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_Tenants_TenantId",
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

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId",
                table: "CreditCustomers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId_CustomerCode",
                table: "CreditCustomers",
                columns: new[] { "TenantId", "CustomerCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId_IsActive",
                table: "CreditCustomers",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId_IsBlocked",
                table: "CreditCustomers",
                columns: new[] { "TenantId", "IsBlocked" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCustomers_TenantId_Phone",
                table: "CreditCustomers",
                columns: new[] { "TenantId", "Phone" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_CreditCustomerId",
                table: "CreditTransactions",
                column: "CreditCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_FuelSaleId",
                table: "CreditTransactions",
                column: "FuelSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_TenantId",
                table: "CreditTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_TransactionDate",
                table: "CreditTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_TransactionType",
                table: "CreditTransactions",
                column: "TransactionType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditTransactions");

            migrationBuilder.DropTable(
                name: "CreditCustomers");

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
    }
}
