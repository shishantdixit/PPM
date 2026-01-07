using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.Tests.Helpers;

public static class TestDbContextFactory
{
    public static ApplicationDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new ApplicationDbContext(options);
        // Don't use EnsureCreated() as it triggers HasData seeding
        // The InMemory provider creates tables on-the-fly when entities are added
        return context;
    }

    public static void SeedTestData(ApplicationDbContext context)
    {
        // Create test tenant
        var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var tenant = new Tenant
        {
            TenantId = tenantId,
            TenantCode = "TEST001",
            CompanyName = "Test Petrol Pump",
            OwnerName = "Test Owner",
            Email = "test@test.com",
            Phone = "1234567890",
            SubscriptionPlan = "Premium",
            SubscriptionStatus = "Active",
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionEndDate = DateTime.UtcNow.AddYears(1),
            MaxMachines = 10,
            MaxWorkers = 50,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Tenants.Add(tenant);

        // Create second tenant for isolation tests
        var tenant2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var tenant2 = new Tenant
        {
            TenantId = tenant2Id,
            TenantCode = "TEST002",
            CompanyName = "Second Petrol Pump",
            OwnerName = "Second Owner",
            Email = "test2@test.com",
            Phone = "9876543210",
            SubscriptionPlan = "Basic",
            SubscriptionStatus = "Active",
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionEndDate = DateTime.UtcNow.AddYears(1),
            MaxMachines = 5,
            MaxWorkers = 10,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Tenants.Add(tenant2);

        // Create test users
        var ownerId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var owner = new User
        {
            UserId = ownerId,
            TenantId = tenantId,
            Username = "testowner",
            Email = "owner@test.com",
            Phone = "1111111111",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            FullName = "Test Owner",
            Role = "Owner",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Users.Add(owner);

        var managerId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var manager = new User
        {
            UserId = managerId,
            TenantId = tenantId,
            Username = "testmanager",
            Email = "manager@test.com",
            Phone = "2222222222",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            FullName = "Test Manager",
            Role = "Manager",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Users.Add(manager);

        var workerId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var worker = new User
        {
            UserId = workerId,
            TenantId = tenantId,
            Username = "testworker",
            Email = "worker@test.com",
            Phone = "3333333333",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            FullName = "Test Worker",
            Role = "Worker",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Users.Add(worker);

        // Create fuel types
        var petrolId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var petrol = new FuelType
        {
            FuelTypeId = petrolId,
            TenantId = tenantId,
            FuelName = "Petrol",
            FuelCode = "PTR",
            Unit = "Liters",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.FuelTypes.Add(petrol);

        var dieselId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var diesel = new FuelType
        {
            FuelTypeId = dieselId,
            TenantId = tenantId,
            FuelName = "Diesel",
            FuelCode = "DSL",
            Unit = "Liters",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.FuelTypes.Add(diesel);

        // Create tanks
        var tank1Id = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var tank1 = new Tank
        {
            TankId = tank1Id,
            TenantId = tenantId,
            TankName = "Petrol Tank 1",
            TankCode = "PT001",
            FuelTypeId = petrolId,
            Capacity = 10000,
            CurrentStock = 5000,
            MinimumLevel = 1000,
            Location = "North Side",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Tanks.Add(tank1);

        var tank2Id = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var tank2 = new Tank
        {
            TankId = tank2Id,
            TenantId = tenantId,
            TankName = "Diesel Tank 1",
            TankCode = "DT001",
            FuelTypeId = dieselId,
            Capacity = 15000,
            CurrentStock = 500,
            MinimumLevel = 2000,
            Location = "South Side",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Tanks.Add(tank2);

        // Create features
        var reportsFeatureId = Guid.Parse("f0000001-0000-0000-0000-000000000001");
        var reportsFeature = new Feature
        {
            FeatureId = reportsFeatureId,
            FeatureCode = "REPORTS",
            FeatureName = "Reports & Analytics",
            Description = "Access to reports",
            Module = "Analytics",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Features.Add(reportsFeature);

        // Create plan features
        var premiumReportsFeature = new PlanFeature
        {
            PlanFeatureId = Guid.NewGuid(),
            SubscriptionPlan = "Premium",
            FeatureId = reportsFeatureId,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.PlanFeatures.Add(premiumReportsFeature);

        var basicReportsFeature = new PlanFeature
        {
            PlanFeatureId = Guid.NewGuid(),
            SubscriptionPlan = "Basic",
            FeatureId = reportsFeatureId,
            IsEnabled = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.PlanFeatures.Add(basicReportsFeature);

        context.SaveChanges();
    }

    public static Guid TestTenantId => Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static Guid TestTenant2Id => Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static Guid TestOwnerId => Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static Guid TestManagerId => Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static Guid TestWorkerId => Guid.Parse("55555555-5555-5555-5555-555555555555");
    public static Guid TestPetrolId => Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static Guid TestDieselId => Guid.Parse("77777777-7777-7777-7777-777777777777");
    public static Guid TestPetrolTankId => Guid.Parse("88888888-8888-8888-8888-888888888888");
    public static Guid TestDieselTankId => Guid.Parse("99999999-9999-9999-9999-999999999999");
}
