using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemUsers",
                columns: table => new
                {
                    SystemUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "SuperAdmin"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUsers", x => x.SystemUserId);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "India"),
                    PinCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    SubscriptionPlan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SubscriptionStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SubscriptionStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubscriptionEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxMachines = table.Column<int>(type: "integer", nullable: false),
                    MaxWorkers = table.Column<int>(type: "integer", nullable: false),
                    MaxMonthlyBills = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "FuelTypes",
                columns: table => new
                {
                    FuelTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FuelName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FuelCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Liters"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelTypes", x => x.FuelTypeId);
                    table.ForeignKey(
                        name: "FK_FuelTypes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Salary = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FuelRates",
                columns: table => new
                {
                    FuelRateId = table.Column<Guid>(type: "uuid", nullable: false),
                    FuelTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelRates", x => x.FuelRateId);
                    table.ForeignKey(
                        name: "FK_FuelRates_FuelTypes_FuelTypeId",
                        column: x => x.FuelTypeId,
                        principalTable: "FuelTypes",
                        principalColumn: "FuelTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelRates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelRates_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "SystemUsers",
                columns: new[] { "SystemUserId", "CreatedAt", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 6, 16, 51, 32, 14, DateTimeKind.Utc).AddTicks(1627), "admin@ppmapp.com", "Super Administrator", true, null, "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", "SuperAdmin", new DateTime(2026, 1, 6, 16, 51, 32, 14, DateTimeKind.Utc).AddTicks(1759), "superadmin" });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "TenantId", "Address", "City", "CompanyName", "Country", "CreatedAt", "Email", "IsActive", "MaxMachines", "MaxMonthlyBills", "MaxWorkers", "OwnerName", "Phone", "PinCode", "State", "SubscriptionEndDate", "SubscriptionPlan", "SubscriptionStartDate", "SubscriptionStatus", "TenantCode", "UpdatedAt" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "123 Demo Street, Near City Center", "Mumbai", "Demo Petrol Pump", "India", new DateTime(2026, 1, 6, 16, 51, 32, 15, DateTimeKind.Utc).AddTicks(8453), "demo@petroldemo.com", true, 5, 10000, 20, "Rajesh Kumar", "9876543210", "400001", "Maharashtra", new DateTime(2027, 1, 6, 16, 51, 32, 15, DateTimeKind.Utc).AddTicks(6394), "Premium", new DateTime(2026, 1, 6, 16, 51, 32, 15, DateTimeKind.Utc).AddTicks(6119), "Active", "DEMO001", new DateTime(2026, 1, 6, 16, 51, 32, 15, DateTimeKind.Utc).AddTicks(8455) });

            migrationBuilder.InsertData(
                table: "FuelTypes",
                columns: new[] { "FuelTypeId", "CreatedAt", "FuelCode", "FuelName", "IsActive", "TenantId", "Unit", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7635), "PTR", "Petrol", true, new Guid("22222222-2222-2222-2222-222222222222"), "Liters", new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7636) },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7644), "DSL", "Diesel", true, new Guid("22222222-2222-2222-2222-222222222222"), "Liters", new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7645) },
                    { new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7649), "CNG", "CNG", true, new Guid("22222222-2222-2222-2222-222222222222"), "Kg", new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(7650) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "DateOfJoining", "Email", "EmployeeCode", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Phone", "Role", "Salary", "TenantId", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(2584), null, "owner@petroldemo.com", null, "Rajesh Kumar", true, null, "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", "9876543210", "Owner", null, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(2585), "owner" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4696), new DateTime(2025, 7, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(3149), "manager@petroldemo.com", "MGR001", "Suresh Patel", true, null, "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", "9876543211", "Manager", 30000m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4698), "manager" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4799), new DateTime(2025, 10, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4795), "ramesh@petroldemo.com", "EMP001", "Ramesh Kumar", true, null, "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", "9876543212", "Worker", 15000m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4801), "ramesh" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4830), new DateTime(2025, 11, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4827), "dinesh@petroldemo.com", "EMP002", "Dinesh Sharma", true, null, "$2a$11$YQ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Oe6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ6vQ", "9876543213", "Worker", 15000m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(4830), "dinesh" }
                });

            migrationBuilder.InsertData(
                table: "FuelRates",
                columns: new[] { "FuelRateId", "CreatedAt", "EffectiveFrom", "EffectiveTo", "FuelTypeId", "Rate", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(965), new DateTime(2026, 1, 6, 16, 51, 32, 16, DateTimeKind.Utc).AddTicks(9520), null, new Guid("77777777-7777-7777-7777-777777777777"), 102.50m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(969), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(981), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(978), null, new Guid("88888888-8888-8888-8888-888888888888"), 89.75m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(982), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(990), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(988), null, new Guid("99999999-9999-9999-9999-999999999999"), 75.00m, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 6, 16, 51, 32, 17, DateTimeKind.Utc).AddTicks(991), new Guid("33333333-3333-3333-3333-333333333333") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuelRates_FuelTypeId",
                table: "FuelRates",
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRates_FuelTypeId_EffectiveTo",
                table: "FuelRates",
                columns: new[] { "FuelTypeId", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_FuelRates_TenantId",
                table: "FuelRates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRates_UpdatedBy",
                table: "FuelRates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTypes_TenantId",
                table: "FuelTypes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTypes_TenantId_FuelCode",
                table: "FuelTypes",
                columns: new[] { "TenantId", "FuelCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemUsers_Email",
                table: "SystemUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemUsers_Username",
                table: "SystemUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Email",
                table: "Tenants",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_SubscriptionStatus_IsActive",
                table: "Tenants",
                columns: new[] { "SubscriptionStatus", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_TenantCode",
                table: "Tenants",
                column: "TenantCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_IsActive",
                table: "Users",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Phone",
                table: "Users",
                columns: new[] { "TenantId", "Phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Role",
                table: "Users",
                columns: new[] { "TenantId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Username",
                table: "Users",
                columns: new[] { "TenantId", "Username" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelRates");

            migrationBuilder.DropTable(
                name: "SystemUsers");

            migrationBuilder.DropTable(
                name: "FuelTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
