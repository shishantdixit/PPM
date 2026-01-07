# Database Schema - Multi-Tenant Petrol Pump Management System

## 🗄️ Entity Relationship Diagram (ERD)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          MULTI-TENANT DATABASE SCHEMA                        │
└─────────────────────────────────────────────────────────────────────────────┘

SUPER ADMIN TABLES (No TenantId)
├── Tenants
├── Subscriptions
└── SystemUsers

TENANT-SPECIFIC TABLES (All have TenantId)
├── Users (Owner, Manager, Worker)
├── Machines (Fuel Dispensers)
├── Nozzles
├── FuelTypes
├── FuelRates
├── Shifts
├── ShiftReadings
├── Bills/Invoices
├── BillItems
├── Payments
├── CreditCustomers
├── CreditTransactions
├── Tanks
├── StockEntries
├── Expenses
├── AuditLogs
└── Reports (Materialized/Cached)
```

## 📋 Table Definitions

### 1. SUPER ADMIN TABLES (Global - No Tenant Isolation)

#### Tenants (Petrol Pump Clients)
```sql
CREATE TABLE Tenants (
    TenantId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantCode VARCHAR(50) UNIQUE NOT NULL,  -- e.g., 'PUMP001'
    CompanyName VARCHAR(200) NOT NULL,
    OwnerName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Address TEXT,
    City VARCHAR(100),
    State VARCHAR(100),
    Country VARCHAR(50) DEFAULT 'India',
    PinCode VARCHAR(10),

    -- Subscription
    SubscriptionPlan VARCHAR(50) NOT NULL,    -- Basic, Premium, Enterprise
    SubscriptionStatus VARCHAR(20) NOT NULL,  -- Active, Suspended, Expired
    SubscriptionStartDate TIMESTAMP NOT NULL,
    SubscriptionEndDate TIMESTAMP,

    -- Limits
    MaxMachines INT DEFAULT 5,
    MaxWorkers INT DEFAULT 20,
    MaxMonthlyBills INT DEFAULT 10000,

    -- System
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    -- Indexes
    INDEX idx_tenants_code (TenantCode),
    INDEX idx_tenants_status (SubscriptionStatus, IsActive)
);
```

#### SystemUsers (Super Admin Users)
```sql
CREATE TABLE SystemUsers (
    SystemUserId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Username VARCHAR(50) UNIQUE NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Role VARCHAR(20) DEFAULT 'SuperAdmin',
    IsActive BOOLEAN DEFAULT TRUE,
    LastLoginAt TIMESTAMP,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

---

### 2. TENANT-SPECIFIC TABLES (All include TenantId)

#### Users (Pump Owner, Manager, Worker)
```sql
CREATE TABLE Users (
    UserId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,

    Username VARCHAR(50) NOT NULL,
    Email VARCHAR(100),
    Phone VARCHAR(20) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,

    FullName VARCHAR(100) NOT NULL,
    Role VARCHAR(20) NOT NULL,  -- Owner, Manager, Worker

    -- Worker specific
    EmployeeCode VARCHAR(50),
    DateOfJoining DATE,
    Salary DECIMAL(10,2),

    IsActive BOOLEAN DEFAULT TRUE,
    LastLoginAt TIMESTAMP,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    UNIQUE(TenantId, Username),
    UNIQUE(TenantId, Phone),
    INDEX idx_users_tenant (TenantId),
    INDEX idx_users_role (TenantId, Role),
    INDEX idx_users_active (TenantId, IsActive)
);
```

#### FuelTypes
```sql
CREATE TABLE FuelTypes (
    FuelTypeId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,

    FuelName VARCHAR(50) NOT NULL,  -- Petrol, Diesel, CNG, Premium Petrol
    FuelCode VARCHAR(20) NOT NULL,  -- PTR, DSL, CNG
    Unit VARCHAR(20) DEFAULT 'Liters',  -- Liters, Kg (for CNG)
    IsActive BOOLEAN DEFAULT TRUE,

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    UNIQUE(TenantId, FuelCode),
    INDEX idx_fueltypes_tenant (TenantId)
);
```

#### FuelRates (Price History)
```sql
CREATE TABLE FuelRates (
    FuelRateId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,
    FuelTypeId UUID NOT NULL REFERENCES FuelTypes(FuelTypeId) ON DELETE CASCADE,

    Rate DECIMAL(10,2) NOT NULL,  -- Price per liter/kg
    EffectiveFrom TIMESTAMP NOT NULL,
    EffectiveTo TIMESTAMP,  -- NULL means current rate

    UpdatedBy UUID REFERENCES Users(UserId),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_fuelrates_tenant (TenantId),
    INDEX idx_fuelrates_fuel (FuelTypeId),
    INDEX idx_fuelrates_current (FuelTypeId, EffectiveTo)
);
```

#### Machines (Fuel Dispensers)
```sql
CREATE TABLE Machines (
    MachineId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,

    MachineName VARCHAR(100) NOT NULL,  -- Machine 1, Machine A
    MachineCode VARCHAR(50) NOT NULL,
    SerialNumber VARCHAR(100),
    Manufacturer VARCHAR(100),
    Model VARCHAR(100),
    InstallationDate DATE,

    Location VARCHAR(100),  -- Left Side, Right Side, Center
    IsActive BOOLEAN DEFAULT TRUE,

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    UNIQUE(TenantId, MachineCode),
    INDEX idx_machines_tenant (TenantId),
    INDEX idx_machines_active (TenantId, IsActive)
);
```

#### Nozzles
```sql
CREATE TABLE Nozzles (
    NozzleId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,
    MachineId UUID NOT NULL REFERENCES Machines(MachineId) ON DELETE CASCADE,
    FuelTypeId UUID NOT NULL REFERENCES FuelTypes(FuelTypeId),

    NozzleNumber VARCHAR(20) NOT NULL,  -- N1, N2, N3
    NozzleName VARCHAR(100),  -- Nozzle 1 - Petrol, Nozzle 2 - Diesel

    CurrentMeterReading DECIMAL(12,3) DEFAULT 0,  -- Current total meter reading

    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    UNIQUE(TenantId, MachineId, NozzleNumber),
    INDEX idx_nozzles_tenant (TenantId),
    INDEX idx_nozzles_machine (MachineId),
    INDEX idx_nozzles_fuel (FuelTypeId)
);
```

#### Shifts
```sql
CREATE TABLE Shifts (
    ShiftId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,
    WorkerId UUID NOT NULL REFERENCES Users(UserId),
    NozzleId UUID NOT NULL REFERENCES Nozzles(NozzleId),

    ShiftDate DATE NOT NULL,
    ShiftType VARCHAR(20) NOT NULL,  -- Morning, Afternoon, Evening, Night

    StartTime TIMESTAMP NOT NULL,
    EndTime TIMESTAMP,

    OpeningMeterReading DECIMAL(12,3) NOT NULL,
    ClosingMeterReading DECIMAL(12,3),
    TotalFuelSold DECIMAL(12,3),  -- Calculated: Closing - Opening

    OpeningCash DECIMAL(10,2) DEFAULT 0,

    TotalSalesAmount DECIMAL(12,2) DEFAULT 0,
    TotalCashReceived DECIMAL(12,2) DEFAULT 0,
    TotalDigitalReceived DECIMAL(12,2) DEFAULT 0,
    TotalCreditGiven DECIMAL(12,2) DEFAULT 0,

    ExpectedCollection DECIMAL(12,2),  -- TotalSales - TotalCredit
    ActualCollection DECIMAL(12,2),     -- Cash + Digital
    Variance DECIMAL(12,2),              -- Actual - Expected (shortage/excess)

    ShiftStatus VARCHAR(20) DEFAULT 'Open',  -- Open, Closed, Reconciled

    StartedBy UUID REFERENCES Users(UserId),  -- Manager who started shift
    ClosedBy UUID REFERENCES Users(UserId),   -- Manager who closed shift

    Notes TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_shifts_tenant (TenantId),
    INDEX idx_shifts_worker (WorkerId),
    INDEX idx_shifts_nozzle (NozzleId),
    INDEX idx_shifts_date (TenantId, ShiftDate),
    INDEX idx_shifts_status (TenantId, ShiftStatus)
);
```

#### ShiftReadings (Intermediate meter readings during shift)
```sql
CREATE TABLE ShiftReadings (
    ShiftReadingId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,
    ShiftId UUID NOT NULL REFERENCES Shifts(ShiftId) ON DELETE CASCADE,

    ReadingTime TIMESTAMP NOT NULL,
    MeterReading DECIMAL(12,3) NOT NULL,
    RecordedBy UUID REFERENCES Users(UserId),

    Notes TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_shiftreadings_shift (ShiftId),
    INDEX idx_shiftreadings_tenant (TenantId)
);
```

#### Bills (Invoices)
```sql
CREATE TABLE Bills (
    BillId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,

    BillNumber VARCHAR(50) NOT NULL,  -- Auto-generated: PUMP001-20240106-0001
    BillDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    ShiftId UUID REFERENCES Shifts(ShiftId),
    WorkerId UUID NOT NULL REFERENCES Users(UserId),
    NozzleId UUID NOT NULL REFERENCES Nozzles(NozzleId),

    CustomerName VARCHAR(100),
    VehicleNumber VARCHAR(20),
    CustomerPhone VARCHAR(20),

    TotalAmount DECIMAL(10,2) NOT NULL,
    PaymentMode VARCHAR(20) NOT NULL,  -- Cash, UPI, Card, Credit

    IsCreditBill BOOLEAN DEFAULT FALSE,
    CreditCustomerId UUID REFERENCES CreditCustomers(CustomerId),

    IsPrinted BOOLEAN DEFAULT FALSE,
    IsVoid BOOLEAN DEFAULT FALSE,
    VoidReason TEXT,
    VoidedBy UUID REFERENCES Users(UserId),
    VoidedAt TIMESTAMP,

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    UNIQUE(TenantId, BillNumber),
    INDEX idx_bills_tenant (TenantId),
    INDEX idx_bills_date (TenantId, BillDate),
    INDEX idx_bills_shift (ShiftId),
    INDEX idx_bills_worker (WorkerId),
    INDEX idx_bills_credit (CreditCustomerId)
);
```

#### BillItems
```sql
CREATE TABLE BillItems (
    BillItemId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,
    BillId UUID NOT NULL REFERENCES Bills(BillId) ON DELETE CASCADE,

    FuelTypeId UUID NOT NULL REFERENCES FuelTypes(FuelTypeId),
    Quantity DECIMAL(10,3) NOT NULL,
    Rate DECIMAL(10,2) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,  -- Quantity × Rate

    MeterReadingBefore DECIMAL(12,3),
    MeterReadingAfter DECIMAL(12,3),

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_billitems_bill (BillId),
    INDEX idx_billitems_tenant (TenantId)
);
```

#### Payments (Separate payment tracking)
```sql
CREATE TABLE Payments (
    PaymentId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,
    BillId UUID REFERENCES Bills(BillId),

    PaymentDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMode VARCHAR(20) NOT NULL,  -- Cash, UPI, Card, Cheque

    TransactionId VARCHAR(100),  -- UPI transaction ID
    TransactionDetails TEXT,

    ReceivedBy UUID REFERENCES Users(UserId),

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_payments_tenant (TenantId),
    INDEX idx_payments_bill (BillId),
    INDEX idx_payments_date (TenantId, PaymentDate)
);
```

#### CreditCustomers (Udhaar Customers)
```sql
CREATE TABLE CreditCustomers (
    CustomerId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,

    CustomerCode VARCHAR(50),
    CustomerName VARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    AlternatePhone VARCHAR(20),
    Email VARCHAR(100),

    Address TEXT,
    City VARCHAR(100),

    VehicleNumbers TEXT[],  -- Array of vehicle numbers

    CreditLimit DECIMAL(10,2) DEFAULT 0,
    CurrentBalance DECIMAL(10,2) DEFAULT 0,  -- Outstanding amount

    IsActive BOOLEAN DEFAULT TRUE,

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    UNIQUE(TenantId, Phone),
    INDEX idx_creditcustomers_tenant (TenantId),
    INDEX idx_creditcustomers_balance (TenantId, CurrentBalance),
    INDEX idx_creditcustomers_active (TenantId, IsActive)
);
```

#### CreditTransactions (Udhaar History)
```sql
CREATE TABLE CreditTransactions (
    CreditTransactionId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,
    CustomerId UUID NOT NULL REFERENCES CreditCustomers(CustomerId) ON DELETE CASCADE,

    TransactionDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TransactionType VARCHAR(20) NOT NULL,  -- Debit (fuel taken), Credit (payment made)

    BillId UUID REFERENCES Bills(BillId),
    ShiftId UUID REFERENCES Shifts(ShiftId),
    WorkerId UUID REFERENCES Users(UserId),

    Amount DECIMAL(10,2) NOT NULL,
    BalanceBefore DECIMAL(10,2) NOT NULL,
    BalanceAfter DECIMAL(10,2) NOT NULL,

    PaymentMode VARCHAR(20),  -- For credit transactions (Cash, UPI, etc.)
    TransactionReference VARCHAR(100),

    Notes TEXT,

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_credittxns_tenant (TenantId),
    INDEX idx_credittxns_customer (CustomerId),
    INDEX idx_credittxns_date (TenantId, TransactionDate),
    INDEX idx_credittxns_worker (WorkerId)
);
```

#### Tanks (Fuel Storage)
```sql
CREATE TABLE Tanks (
    TankId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,
    FuelTypeId UUID NOT NULL REFERENCES FuelTypes(FuelTypeId),

    TankName VARCHAR(100) NOT NULL,  -- Tank 1 - Petrol
    TankNumber VARCHAR(20) NOT NULL,
    Capacity DECIMAL(12,3) NOT NULL,  -- In liters

    CurrentStock DECIMAL(12,3) DEFAULT 0,
    MinimumStockLevel DECIMAL(12,3) DEFAULT 0,  -- For alerts

    IsActive BOOLEAN DEFAULT TRUE,

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    UNIQUE(TenantId, TankNumber),
    INDEX idx_tanks_tenant (TenantId),
    INDEX idx_tanks_fuel (FuelTypeId)
);
```

#### StockEntries (Stock In/Out)
```sql
CREATE TABLE StockEntries (
    StockEntryId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,
    TankId UUID NOT NULL REFERENCES Tanks(TankId) ON DELETE CASCADE,

    EntryDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    EntryType VARCHAR(20) NOT NULL,  -- StockIn (refill), StockOut (sold), Adjustment

    Quantity DECIMAL(12,3) NOT NULL,
    Rate DECIMAL(10,2),  -- Purchase rate (for stock in)
    TotalCost DECIMAL(12,2),

    StockBefore DECIMAL(12,3) NOT NULL,
    StockAfter DECIMAL(12,3) NOT NULL,

    -- For StockIn
    SupplierName VARCHAR(100),
    InvoiceNumber VARCHAR(100),
    InvoiceDate DATE,

    -- For Adjustments
    AdjustmentReason TEXT,

    RecordedBy UUID REFERENCES Users(UserId),

    Notes TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_stockentries_tenant (TenantId),
    INDEX idx_stockentries_tank (TankId),
    INDEX idx_stockentries_date (TenantId, EntryDate),
    INDEX idx_stockentries_type (TenantId, EntryType)
);
```

#### Expenses
```sql
CREATE TABLE Expenses (
    ExpenseId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID NOT NULL REFERENCES Tenants(TenantId) ON DELETE CASCADE,

    ExpenseDate DATE NOT NULL,
    ExpenseCategory VARCHAR(50) NOT NULL,  -- Electricity, Salary, Maintenance, etc.
    Amount DECIMAL(10,2) NOT NULL,

    Description TEXT,

    BillNumber VARCHAR(100),
    VendorName VARCHAR(100),

    PaymentMode VARCHAR(20),  -- Cash, UPI, Cheque
    PaymentStatus VARCHAR(20) DEFAULT 'Paid',  -- Paid, Pending

    RecordedBy UUID REFERENCES Users(UserId),

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_expenses_tenant (TenantId),
    INDEX idx_expenses_date (TenantId, ExpenseDate),
    INDEX idx_expenses_category (TenantId, ExpenseCategory)
);
```

#### AuditLogs
```sql
CREATE TABLE AuditLogs (
    AuditLogId UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TenantId UUID REFERENCES Tenants(TenantId),  -- NULL for super admin actions

    UserId UUID REFERENCES Users(UserId),
    Action VARCHAR(100) NOT NULL,  -- Created, Updated, Deleted, Login, etc.
    EntityType VARCHAR(50),  -- Bill, Shift, User, etc.
    EntityId UUID,

    OldValues JSONB,  -- Previous data
    NewValues JSONB,  -- Updated data

    IPAddress VARCHAR(50),
    UserAgent TEXT,

    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_auditlogs_tenant (TenantId),
    INDEX idx_auditlogs_user (UserId),
    INDEX idx_auditlogs_timestamp (Timestamp),
    INDEX idx_auditlogs_entity (EntityType, EntityId)
);
```

---

## 🔍 Key Indexes for Performance

```sql
-- Global query filter optimization
CREATE INDEX idx_all_tenantid ON Users(TenantId);
CREATE INDEX idx_all_tenantid ON Machines(TenantId);
CREATE INDEX idx_all_tenantid ON Nozzles(TenantId);
-- ... repeat for all tenant tables

-- Composite indexes for common queries
CREATE INDEX idx_bills_tenant_date_worker ON Bills(TenantId, BillDate, WorkerId);
CREATE INDEX idx_shifts_tenant_date_status ON Shifts(TenantId, ShiftDate, ShiftStatus);
CREATE INDEX idx_credittxns_tenant_customer_date ON CreditTransactions(TenantId, CustomerId, TransactionDate);
```

---

## 📊 Calculated/Derived Fields Logic

### Shift Calculations
```sql
-- During shift close
TotalFuelSold = ClosingMeterReading - OpeningMeterReading
TotalSalesAmount = SUM(Bills.TotalAmount) WHERE ShiftId = current_shift
TotalCashReceived = SUM(Payments.Amount) WHERE PaymentMode = 'Cash'
TotalDigitalReceived = SUM(Payments.Amount) WHERE PaymentMode IN ('UPI', 'Card')
TotalCreditGiven = SUM(Bills.TotalAmount) WHERE IsCreditBill = TRUE
ExpectedCollection = TotalSalesAmount - TotalCreditGiven
ActualCollection = TotalCashReceived + TotalDigitalReceived
Variance = ActualCollection - ExpectedCollection
```

### Stock Calculations
```sql
-- After each sale
Tank.CurrentStock = Tank.CurrentStock - BillItem.Quantity

-- After stock refill
Tank.CurrentStock = Tank.CurrentStock + StockEntry.Quantity
```

### Credit Balance
```sql
-- After credit transaction
CreditCustomer.CurrentBalance = CreditCustomer.CurrentBalance + Amount (Debit)
CreditCustomer.CurrentBalance = CreditCustomer.CurrentBalance - Amount (Credit)
```

---

## 🔐 Row-Level Security (EF Core Global Query Filters)

### Implementation in EF Core
```csharp
// In DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Apply global filter to all tenant entities
    modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == _currentTenantId);
    modelBuilder.Entity<Machine>().HasQueryFilter(m => m.TenantId == _currentTenantId);
    modelBuilder.Entity<Nozzle>().HasQueryFilter(n => n.TenantId == _currentTenantId);
    modelBuilder.Entity<Bill>().HasQueryFilter(b => b.TenantId == _currentTenantId);
    // ... apply to all tenant tables
}
```

---

## 🚀 Database Migration Strategy

### Initial Setup
1. Create all tables
2. Seed default data:
   - System admin user
   - Demo tenant
   - Fuel types (Petrol, Diesel, CNG)
   - Default expense categories

### Version Control
- Use EF Core Migrations
- Name migrations descriptively: `20240106_InitialCreate`, `20240115_AddCreditModule`
- Keep rollback scripts for critical changes

---

## 📈 Scalability Considerations

### Partitioning (Future)
```sql
-- Partition Bills table by month
CREATE TABLE Bills_2024_01 PARTITION OF Bills
FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');
```

### Archival Strategy
- Archive bills older than 3 years to separate table
- Keep aggregated reports for historical data

---

## 🔄 Sample Data Relationships

```
Tenant: ABC Petrol Pump (TenantId: xxx-xxx)
├── Users
│   ├── Owner: Rajesh Kumar
│   ├── Manager: Suresh Patel
│   └── Workers: Ramesh, Dinesh, Mahesh
├── Machines
│   ├── Machine 1 (Petrol)
│   │   ├── Nozzle 1 (Petrol)
│   │   └── Nozzle 2 (Petrol)
│   └── Machine 2 (Diesel)
│       └── Nozzle 3 (Diesel)
├── Shifts (Today)
│   ├── Morning Shift: Ramesh → Nozzle 1
│   ├── Afternoon Shift: Dinesh → Nozzle 2
│   └── Evening Shift: Mahesh → Nozzle 3
└── Bills
    ├── Bill #1: ₹1500 (Cash) → Shift: Morning
    ├── Bill #2: ₹2000 (UPI) → Shift: Morning
    └── Bill #3: ₹3000 (Credit) → Customer: Transport Co.
```

---

**Next Steps**: Proceed to API Design & Endpoints
