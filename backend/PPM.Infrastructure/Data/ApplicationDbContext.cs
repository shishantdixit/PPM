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
    public DbSet<CreditCustomer> CreditCustomers { get; set; }
    public DbSet<CreditTransaction> CreditTransactions { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    // Stock & Tank Management
    public DbSet<Tank> Tanks { get; set; }
    public DbSet<StockEntry> StockEntries { get; set; }

    // Feature management tables
    public DbSet<Feature> Features { get; set; }
    public DbSet<PlanFeature> PlanFeatures { get; set; }
    public DbSet<TenantFeature> TenantFeatures { get; set; }

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
        ConfigureCreditCustomers(modelBuilder);
        ConfigureCreditTransactions(modelBuilder);
        ConfigureExpenses(modelBuilder);
        ConfigureTanks(modelBuilder);
        ConfigureStockEntries(modelBuilder);
        ConfigureFeatures(modelBuilder);
        ConfigurePlanFeatures(modelBuilder);
        ConfigureTenantFeatures(modelBuilder);

        // Seed data
        SeedData(modelBuilder);
        SeedFeatures(modelBuilder);
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

    private void ConfigureShifts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ShiftId);
            entity.Property(e => e.ShiftDate).IsRequired();
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.TotalSales).HasPrecision(18, 2);
            entity.Property(e => e.CashCollected).HasPrecision(18, 2);
            entity.Property(e => e.CreditSales).HasPrecision(18, 2);
            entity.Property(e => e.DigitalPayments).HasPrecision(18, 2);
            entity.Property(e => e.Borrowing).HasPrecision(18, 2);
            entity.Property(e => e.Variance).HasPrecision(18, 2);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.WorkerId);
            entity.HasIndex(e => e.ShiftDate);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Worker)
                .WithMany()
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureShiftNozzleReadings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShiftNozzleReading>(entity =>
        {
            entity.HasKey(e => e.ShiftNozzleReadingId);
            entity.Property(e => e.OpeningReading).HasPrecision(18, 3).IsRequired();
            entity.Property(e => e.ClosingReading).HasPrecision(18, 3);
            entity.Property(e => e.QuantitySold).HasPrecision(18, 3);
            entity.Property(e => e.RateAtShift).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.ExpectedAmount).HasPrecision(18, 2);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.ShiftId);
            entity.HasIndex(e => e.NozzleId);
            entity.HasIndex(e => new { e.ShiftId, e.NozzleId }).IsUnique();

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Shift)
                .WithMany(s => s.NozzleReadings)
                .HasForeignKey(e => e.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Nozzle)
                .WithMany()
                .HasForeignKey(e => e.NozzleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureFuelSales(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FuelSale>(entity =>
        {
            entity.HasKey(e => e.FuelSaleId);
            entity.Property(e => e.Quantity).HasPrecision(18, 3).IsRequired();
            entity.Property(e => e.Rate).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.PaymentMethod).IsRequired();
            entity.Property(e => e.SaleTime).IsRequired();
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.VehicleNumber).HasMaxLength(50);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.ShiftId);
            entity.HasIndex(e => e.NozzleId);
            entity.HasIndex(e => e.SaleTime);
            entity.HasIndex(e => e.PaymentMethod);

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Shift)
                .WithMany(s => s.FuelSales)
                .HasForeignKey(e => e.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Nozzle)
                .WithMany()
                .HasForeignKey(e => e.NozzleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureCreditCustomers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CreditCustomer>(entity =>
        {
            entity.HasKey(e => e.CreditCustomerId);
            entity.Property(e => e.CustomerCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactPerson).HasMaxLength(200);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.VehicleNumbers).HasMaxLength(1000);
            entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
            entity.Property(e => e.PaymentTermDays).HasDefaultValue(30);
            entity.Property(e => e.BlockReason).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.CustomerCode }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.Phone });
            entity.HasIndex(e => new { e.TenantId, e.IsActive });
            entity.HasIndex(e => new { e.TenantId, e.IsBlocked });

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureCreditTransactions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CreditTransaction>(entity =>
        {
            entity.HasKey(e => e.CreditTransactionId);
            entity.Property(e => e.TransactionType).IsRequired();
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.BalanceAfter).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.TransactionDate).IsRequired();
            entity.Property(e => e.PaymentReference).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(200);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.CreditCustomerId);
            entity.HasIndex(e => e.FuelSaleId);
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.TransactionType);

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreditCustomer)
                .WithMany(c => c.Transactions)
                .HasForeignKey(e => e.CreditCustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.FuelSale)
                .WithMany()
                .HasForeignKey(e => e.FuelSaleId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureExpenses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId);
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.ExpenseDate).IsRequired();
            entity.Property(e => e.PaymentMode).IsRequired();
            entity.Property(e => e.Reference).HasMaxLength(100);
            entity.Property(e => e.Vendor).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.ExpenseDate);
            entity.HasIndex(e => new { e.TenantId, e.ExpenseDate });
            entity.HasIndex(e => new { e.TenantId, e.Category });

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.RecordedBy)
                .WithMany()
                .HasForeignKey(e => e.RecordedById)
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

    private void ConfigureTanks(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tank>(entity =>
        {
            entity.HasKey(e => e.TankId);
            entity.Property(e => e.TankName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TankCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Capacity).HasPrecision(18, 3).IsRequired();
            entity.Property(e => e.CurrentStock).HasPrecision(18, 3);
            entity.Property(e => e.MinimumLevel).HasPrecision(18, 3);
            entity.Property(e => e.Location).HasMaxLength(200);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.TankCode }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.FuelTypeId });
            entity.HasIndex(e => new { e.TenantId, e.IsActive });

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.FuelType)
                .WithMany()
                .HasForeignKey(e => e.FuelTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureStockEntries(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockEntry>(entity =>
        {
            entity.HasKey(e => e.StockEntryId);
            entity.Property(e => e.EntryType).IsRequired();
            entity.Property(e => e.Quantity).HasPrecision(18, 3).IsRequired();
            entity.Property(e => e.StockBefore).HasPrecision(18, 3).IsRequired();
            entity.Property(e => e.StockAfter).HasPrecision(18, 3).IsRequired();
            entity.Property(e => e.EntryDate).IsRequired();
            entity.Property(e => e.Reference).HasMaxLength(100);
            entity.Property(e => e.Vendor).HasMaxLength(200);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.TankId);
            entity.HasIndex(e => e.EntryDate);
            entity.HasIndex(e => e.EntryType);
            entity.HasIndex(e => new { e.TenantId, e.TankId, e.EntryDate });
            entity.HasIndex(e => new { e.TenantId, e.ShiftId });

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tank)
                .WithMany(t => t.StockEntries)
                .HasForeignKey(e => e.TankId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Shift)
                .WithMany()
                .HasForeignKey(e => e.ShiftId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.FuelSale)
                .WithMany()
                .HasForeignKey(e => e.FuelSaleId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RecordedBy)
                .WithMany()
                .HasForeignKey(e => e.RecordedById)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureFeatures(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeatureId);
            entity.Property(e => e.FeatureCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FeatureName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Module).HasMaxLength(50);
            entity.Property(e => e.Icon).HasMaxLength(50);

            entity.HasIndex(e => e.FeatureCode).IsUnique();
        });
    }

    private void ConfigurePlanFeatures(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlanFeature>(entity =>
        {
            entity.HasKey(e => e.PlanFeatureId);
            entity.Property(e => e.SubscriptionPlan).IsRequired().HasMaxLength(50);

            entity.HasIndex(e => new { e.SubscriptionPlan, e.FeatureId }).IsUnique();

            entity.HasOne(e => e.Feature)
                .WithMany()
                .HasForeignKey(e => e.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureTenantFeatures(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantFeature>(entity =>
        {
            entity.HasKey(e => e.TenantFeatureId);
            entity.Property(e => e.OverriddenBy).HasMaxLength(100);

            entity.HasIndex(e => new { e.TenantId, e.FeatureId }).IsUnique();

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Feature)
                .WithMany()
                .HasForeignKey(e => e.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void SeedFeatures(ModelBuilder modelBuilder)
    {
        // Define feature IDs
        var reportsId = Guid.Parse("f0000001-0000-0000-0000-000000000001");
        var creditCustomersId = Guid.Parse("f0000002-0000-0000-0000-000000000002");
        var expensesId = Guid.Parse("f0000003-0000-0000-0000-000000000003");
        var multiShiftId = Guid.Parse("f0000004-0000-0000-0000-000000000004");
        var exportId = Guid.Parse("f0000005-0000-0000-0000-000000000005");
        var apiAccessId = Guid.Parse("f0000006-0000-0000-0000-000000000006");
        var advancedReportsId = Guid.Parse("f0000007-0000-0000-0000-000000000007");
        var bulkOperationsId = Guid.Parse("f0000008-0000-0000-0000-000000000008");

        // Seed Features
        modelBuilder.Entity<Feature>().HasData(
            new Feature
            {
                FeatureId = reportsId,
                FeatureCode = "REPORTS",
                FeatureName = "Reports & Analytics",
                Description = "Access to sales reports, dashboards, and analytics",
                Module = "Analytics",
                Icon = "chart-bar",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Feature
            {
                FeatureId = creditCustomersId,
                FeatureCode = "CREDIT_CUSTOMERS",
                FeatureName = "Credit Customer Management",
                Description = "Manage credit customers and their balances",
                Module = "Sales",
                Icon = "credit-card",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Feature
            {
                FeatureId = expensesId,
                FeatureCode = "EXPENSES",
                FeatureName = "Expense Tracking",
                Description = "Track and manage business expenses",
                Module = "Finance",
                Icon = "receipt",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Feature
            {
                FeatureId = multiShiftId,
                FeatureCode = "MULTI_SHIFT",
                FeatureName = "Multiple Shifts",
                Description = "Support for multiple shifts per day",
                Module = "Operations",
                Icon = "clock",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Feature
            {
                FeatureId = exportId,
                FeatureCode = "EXPORT",
                FeatureName = "Data Export",
                Description = "Export data to Excel and PDF",
                Module = "Utilities",
                Icon = "download",
                DisplayOrder = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Feature
            {
                FeatureId = apiAccessId,
                FeatureCode = "API_ACCESS",
                FeatureName = "API Access",
                Description = "Programmatic API access for integrations",
                Module = "Integration",
                Icon = "code",
                DisplayOrder = 6,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Feature
            {
                FeatureId = advancedReportsId,
                FeatureCode = "ADVANCED_REPORTS",
                FeatureName = "Advanced Reports",
                Description = "Advanced analytics and custom reports",
                Module = "Analytics",
                Icon = "chart-pie",
                DisplayOrder = 7,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Feature
            {
                FeatureId = bulkOperationsId,
                FeatureCode = "BULK_OPERATIONS",
                FeatureName = "Bulk Operations",
                Description = "Bulk import/export and batch operations",
                Module = "Utilities",
                Icon = "database",
                DisplayOrder = 8,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed Plan Features
        // Basic Plan - All features disabled
        modelBuilder.Entity<PlanFeature>().HasData(
            new PlanFeature { PlanFeatureId = Guid.Parse("ba000001-0000-0000-0000-000000000001"), SubscriptionPlan = "Basic", FeatureId = reportsId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ba000002-0000-0000-0000-000000000002"), SubscriptionPlan = "Basic", FeatureId = creditCustomersId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ba000003-0000-0000-0000-000000000003"), SubscriptionPlan = "Basic", FeatureId = expensesId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ba000004-0000-0000-0000-000000000004"), SubscriptionPlan = "Basic", FeatureId = multiShiftId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ba000005-0000-0000-0000-000000000005"), SubscriptionPlan = "Basic", FeatureId = exportId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ba000006-0000-0000-0000-000000000006"), SubscriptionPlan = "Basic", FeatureId = apiAccessId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ba000007-0000-0000-0000-000000000007"), SubscriptionPlan = "Basic", FeatureId = advancedReportsId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ba000008-0000-0000-0000-000000000008"), SubscriptionPlan = "Basic", FeatureId = bulkOperationsId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        // Premium Plan - Most features enabled
        modelBuilder.Entity<PlanFeature>().HasData(
            new PlanFeature { PlanFeatureId = Guid.Parse("ca000001-0000-0000-0000-000000000001"), SubscriptionPlan = "Premium", FeatureId = reportsId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ca000002-0000-0000-0000-000000000002"), SubscriptionPlan = "Premium", FeatureId = creditCustomersId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ca000003-0000-0000-0000-000000000003"), SubscriptionPlan = "Premium", FeatureId = expensesId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ca000004-0000-0000-0000-000000000004"), SubscriptionPlan = "Premium", FeatureId = multiShiftId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ca000005-0000-0000-0000-000000000005"), SubscriptionPlan = "Premium", FeatureId = exportId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ca000006-0000-0000-0000-000000000006"), SubscriptionPlan = "Premium", FeatureId = apiAccessId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ca000007-0000-0000-0000-000000000007"), SubscriptionPlan = "Premium", FeatureId = advancedReportsId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("ca000008-0000-0000-0000-000000000008"), SubscriptionPlan = "Premium", FeatureId = bulkOperationsId, IsEnabled = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        // Enterprise Plan - All features enabled
        modelBuilder.Entity<PlanFeature>().HasData(
            new PlanFeature { PlanFeatureId = Guid.Parse("da000001-0000-0000-0000-000000000001"), SubscriptionPlan = "Enterprise", FeatureId = reportsId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("da000002-0000-0000-0000-000000000002"), SubscriptionPlan = "Enterprise", FeatureId = creditCustomersId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("da000003-0000-0000-0000-000000000003"), SubscriptionPlan = "Enterprise", FeatureId = expensesId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("da000004-0000-0000-0000-000000000004"), SubscriptionPlan = "Enterprise", FeatureId = multiShiftId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("da000005-0000-0000-0000-000000000005"), SubscriptionPlan = "Enterprise", FeatureId = exportId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("da000006-0000-0000-0000-000000000006"), SubscriptionPlan = "Enterprise", FeatureId = apiAccessId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("da000007-0000-0000-0000-000000000007"), SubscriptionPlan = "Enterprise", FeatureId = advancedReportsId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new PlanFeature { PlanFeatureId = Guid.Parse("da000008-0000-0000-0000-000000000008"), SubscriptionPlan = "Enterprise", FeatureId = bulkOperationsId, IsEnabled = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
    }
}
