using Microsoft.EntityFrameworkCore;
using PPM.Core.Entities;

namespace PPM.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Super Admin tables (no tenant isolation)
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<SystemUser> SystemUsers { get; set; }

    // Tenant-specific tables
    public DbSet<User> Users { get; set; }
    public DbSet<FuelType> FuelTypes { get; set; }
    public DbSet<FuelRate> FuelRates { get; set; }
    public DbSet<Machine> Machines { get; set; }
    public DbSet<Nozzle> Nozzles { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<ShiftNozzleReading> ShiftNozzleReadings { get; set; }
    public DbSet<FuelSale> FuelSales { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Suppress the pending model changes warning (known EF Core 9 issue)
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        ConfigureTenants(modelBuilder);
        ConfigureSystemUsers(modelBuilder);
        ConfigureUsers(modelBuilder);
        ConfigureFuelTypes(modelBuilder);
        ConfigureFuelRates(modelBuilder);
        ConfigureMachines(modelBuilder);
        ConfigureNozzles(modelBuilder);
        ConfigureShifts(modelBuilder);
        ConfigureShiftNozzleReadings(modelBuilder);
        ConfigureFuelSales(modelBuilder);

        // Seed data
        SeedData(modelBuilder);
    }

    private void ConfigureTenants(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.TenantId);
            entity.Property(e => e.TenantCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.OwnerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(50).HasDefaultValue("India");
            entity.Property(e => e.PinCode).HasMaxLength(10);
            entity.Property(e => e.SubscriptionPlan).HasMaxLength(50);
            entity.Property(e => e.SubscriptionStatus).HasMaxLength(20);

            entity.HasIndex(e => e.TenantCode).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => new { e.SubscriptionStatus, e.IsActive });
        });
    }

    private void ConfigureSystemUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemUser>(entity =>
        {
            entity.HasKey(e => e.SystemUserId);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("SuperAdmin");

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }

    private void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.Salary).HasColumnType("decimal(10,2)");

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.Username }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.Phone }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.Role });
            entity.HasIndex(e => new { e.TenantId, e.IsActive });

            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureFuelTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FuelType>(entity =>
        {
            entity.HasKey(e => e.FuelTypeId);
            entity.Property(e => e.FuelName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FuelCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Unit).HasMaxLength(20).HasDefaultValue("Liters");

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.FuelCode }).IsUnique();

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureFuelRates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FuelRate>(entity =>
        {
            entity.HasKey(e => e.FuelRateId);
            entity.Property(e => e.Rate).HasColumnType("decimal(10,2)");

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.FuelTypeId);
            entity.HasIndex(e => new { e.FuelTypeId, e.EffectiveTo });

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.FuelType)
                .WithMany(f => f.FuelRates)
                .HasForeignKey(e => e.FuelTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureMachines(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.MachineId);
            entity.Property(e => e.MachineName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MachineCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(100);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.MachineCode }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.IsActive });

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureNozzles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Nozzle>(entity =>
        {
            entity.HasKey(e => e.NozzleId);
            entity.Property(e => e.NozzleNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.NozzleName).HasMaxLength(100);
            entity.Property(e => e.CurrentMeterReading).HasColumnType("decimal(12,3)");

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.MachineId);
            entity.HasIndex(e => e.FuelTypeId);
            entity.HasIndex(e => new { e.TenantId, e.MachineId, e.NozzleNumber }).IsUnique();

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Machine)
                .WithMany(m => m.Nozzles)
                .HasForeignKey(e => e.MachineId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.FuelType)
                .WithMany()
                .HasForeignKey(e => e.FuelTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Super Admin
        var superAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        modelBuilder.Entity<SystemUser>().HasData(new SystemUser
        {
            SystemUserId = superAdminId,
            Username = "superadmin",
            Email = "admin@ppmapp.com",
            // Password: Admin@123
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            FullName = "Super Administrator",
            Role = "SuperAdmin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed Demo Tenant
        var demoTenantId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        modelBuilder.Entity<Tenant>().HasData(new Tenant
        {
            TenantId = demoTenantId,
            TenantCode = "DEMO001",
            CompanyName = "Demo Petrol Pump",
            OwnerName = "Rajesh Kumar",
            Email = "demo@petroldemo.com",
            Phone = "9876543210",
            Address = "123 Demo Street, Near City Center",
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India",
            PinCode = "400001",
            SubscriptionPlan = "Premium",
            SubscriptionStatus = "Active",
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionEndDate = DateTime.UtcNow.AddYears(1),
            MaxMachines = 5,
            MaxWorkers = 20,
            MaxMonthlyBills = 10000,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed Demo Pump Owner
        var ownerId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = ownerId,
            TenantId = demoTenantId,
            Username = "owner",
            Email = "owner@petroldemo.com",
            Phone = "9876543210",
            // Password: Owner@123
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Owner@123"),
            FullName = "Rajesh Kumar",
            Role = "Owner",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed Demo Manager
        var managerId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = managerId,
            TenantId = demoTenantId,
            Username = "manager",
            Email = "manager@petroldemo.com",
            Phone = "9876543211",
            // Password: Manager@123
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
            FullName = "Suresh Patel",
            Role = "Manager",
            EmployeeCode = "MGR001",
            DateOfJoining = DateTime.UtcNow.AddMonths(-6),
            Salary = 30000,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed Demo Workers
        var worker1Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = worker1Id,
            TenantId = demoTenantId,
            Username = "ramesh",
            Email = "ramesh@petroldemo.com",
            Phone = "9876543212",
            // Password: Worker@123
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Worker@123"),
            FullName = "Ramesh Kumar",
            Role = "Worker",
            EmployeeCode = "EMP001",
            DateOfJoining = DateTime.UtcNow.AddMonths(-3),
            Salary = 15000,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        var worker2Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = worker2Id,
            TenantId = demoTenantId,
            Username = "dinesh",
            Email = "dinesh@petroldemo.com",
            Phone = "9876543213",
            // Password: Worker@123
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Worker@123"),
            FullName = "Dinesh Sharma",
            Role = "Worker",
            EmployeeCode = "EMP002",
            DateOfJoining = DateTime.UtcNow.AddMonths(-2),
            Salary = 15000,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Seed Fuel Types for Demo Tenant
        var petrolId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var dieselId = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var cngId = Guid.Parse("99999999-9999-9999-9999-999999999999");

        modelBuilder.Entity<FuelType>().HasData(
            new FuelType
            {
                FuelTypeId = petrolId,
                TenantId = demoTenantId,
                FuelName = "Petrol",
                FuelCode = "PTR",
                Unit = "Liters",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FuelType
            {
                FuelTypeId = dieselId,
                TenantId = demoTenantId,
                FuelName = "Diesel",
                FuelCode = "DSL",
                Unit = "Liters",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FuelType
            {
                FuelTypeId = cngId,
                TenantId = demoTenantId,
                FuelName = "CNG",
                FuelCode = "CNG",
                Unit = "Kg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed Fuel Rates
        modelBuilder.Entity<FuelRate>().HasData(
            new FuelRate
            {
                FuelRateId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                TenantId = demoTenantId,
                FuelTypeId = petrolId,
                Rate = 102.50m,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = null, // Current rate
                UpdatedBy = ownerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FuelRate
            {
                FuelRateId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                TenantId = demoTenantId,
                FuelTypeId = dieselId,
                Rate = 89.75m,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = null,
                UpdatedBy = ownerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FuelRate
            {
                FuelRateId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                TenantId = demoTenantId,
                FuelTypeId = cngId,
                Rate = 75.00m,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = null,
                UpdatedBy = ownerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed Machines
        var machine1Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var machine2Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

        modelBuilder.Entity<Machine>().HasData(
            new Machine
            {
                MachineId = machine1Id,
                TenantId = demoTenantId,
                MachineName = "Machine 1",
                MachineCode = "M001",
                SerialNumber = "SRL-M1-2023",
                Manufacturer = "Tokheim",
                Model = "Premier B",
                InstallationDate = new DateOnly(2023, 1, 15),
                Location = "Left Side",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Machine
            {
                MachineId = machine2Id,
                TenantId = demoTenantId,
                MachineName = "Machine 2",
                MachineCode = "M002",
                SerialNumber = "SRL-M2-2023",
                Manufacturer = "Wayne",
                Model = "Helix 6000",
                InstallationDate = new DateOnly(2023, 3, 20),
                Location = "Right Side",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed Nozzles
        modelBuilder.Entity<Nozzle>().HasData(
            // Machine 1 Nozzles
            new Nozzle
            {
                NozzleId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                TenantId = demoTenantId,
                MachineId = machine1Id,
                FuelTypeId = petrolId,
                NozzleNumber = "N1",
                NozzleName = "Petrol Nozzle 1",
                CurrentMeterReading = 15234.567m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Nozzle
            {
                NozzleId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                TenantId = demoTenantId,
                MachineId = machine1Id,
                FuelTypeId = dieselId,
                NozzleNumber = "N2",
                NozzleName = "Diesel Nozzle 1",
                CurrentMeterReading = 23456.789m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            // Machine 2 Nozzles
            new Nozzle
            {
                NozzleId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                TenantId = demoTenantId,
                MachineId = machine2Id,
                FuelTypeId = petrolId,
                NozzleNumber = "N1",
                NozzleName = "Petrol Nozzle 2",
                CurrentMeterReading = 18765.432m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Nozzle
            {
                NozzleId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                TenantId = demoTenantId,
                MachineId = machine2Id,
                FuelTypeId = cngId,
                NozzleNumber = "N2",
                NozzleName = "CNG Nozzle 1",
                CurrentMeterReading = 5678.123m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
