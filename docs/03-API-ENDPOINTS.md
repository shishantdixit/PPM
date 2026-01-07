# API Endpoints - Petrol Pump Management System

## 🌐 API Base URL

```
Development: https://localhost:5001/api
Production: https://api.yourppmapp.com/api
```

## 🔐 Authentication

All endpoints (except login/register) require JWT Bearer token:

```http
Authorization: Bearer {token}
```

## 📋 API Conventions

### HTTP Methods
- `GET`: Retrieve data
- `POST`: Create new resource
- `PUT`: Update entire resource
- `PATCH`: Update partial resource
- `DELETE`: Delete resource

### Response Format
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... },
  "errors": null,
  "timestamp": "2024-01-06T10:30:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    {
      "field": "email",
      "message": "Invalid email format"
    }
  ],
  "timestamp": "2024-01-06T10:30:00Z"
}
```

### HTTP Status Codes
- `200`: OK
- `201`: Created
- `204`: No Content
- `400`: Bad Request
- `401`: Unauthorized
- `403`: Forbidden
- `404`: Not Found
- `422`: Validation Error
- `500`: Internal Server Error

---

## 🔑 SUPER ADMIN APIs

### Tenant Management

#### 1. Get All Tenants
```http
GET /api/superadmin/tenants
```
Query params: `?page=1&limit=20&status=Active&search=pump`

Response:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "tenantId": "uuid",
        "tenantCode": "PUMP001",
        "companyName": "ABC Petrol Pump",
        "ownerName": "Rajesh Kumar",
        "email": "rajesh@abc.com",
        "phone": "9876543210",
        "subscriptionPlan": "Premium",
        "subscriptionStatus": "Active",
        "subscriptionEndDate": "2025-01-06",
        "isActive": true,
        "createdAt": "2024-01-06"
      }
    ],
    "totalCount": 50,
    "page": 1,
    "limit": 20
  }
}
```

#### 2. Create New Tenant
```http
POST /api/superadmin/tenants
```
Request:
```json
{
  "tenantCode": "PUMP002",
  "companyName": "XYZ Petrol Pump",
  "ownerName": "Suresh Patel",
  "email": "suresh@xyz.com",
  "phone": "9876543211",
  "address": "123 Main Street",
  "city": "Mumbai",
  "state": "Maharashtra",
  "pinCode": "400001",
  "subscriptionPlan": "Basic",
  "subscriptionStartDate": "2024-01-06",
  "maxMachines": 3,
  "maxWorkers": 10
}
```

#### 3. Update Tenant
```http
PUT /api/superadmin/tenants/{tenantId}
```

#### 4. Deactivate/Activate Tenant
```http
PATCH /api/superadmin/tenants/{tenantId}/status
```
Request:
```json
{
  "isActive": false,
  "reason": "Payment pending"
}
```

#### 5. Get Tenant Statistics
```http
GET /api/superadmin/dashboard/stats
```
Response:
```json
{
  "success": true,
  "data": {
    "totalTenants": 50,
    "activeTenants": 45,
    "suspendedTenants": 5,
    "revenueThisMonth": 250000,
    "newTenantsThisMonth": 5
  }
}
```

---

## 🔐 AUTHENTICATION APIs

### 1. Login
```http
POST /api/auth/login
```
Request:
```json
{
  "tenantCode": "PUMP001",  // Empty for super admin
  "username": "worker123",
  "password": "password123"
}
```
Response:
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR...",
    "refreshToken": "refresh_token_here",
    "expiresIn": 1800,
    "user": {
      "userId": "uuid",
      "username": "worker123",
      "fullName": "Ramesh Kumar",
      "role": "Worker",
      "tenantId": "uuid",
      "tenantCode": "PUMP001"
    }
  }
}
```

### 2. Refresh Token
```http
POST /api/auth/refresh
```
Request:
```json
{
  "refreshToken": "refresh_token_here"
}
```

### 3. Logout
```http
POST /api/auth/logout
```

### 4. Change Password
```http
POST /api/auth/change-password
```
Request:
```json
{
  "currentPassword": "old123",
  "newPassword": "new123"
}
```

---

## 👥 USER MANAGEMENT APIs

### 1. Get All Users (Workers/Managers)
```http
GET /api/users
```
Query: `?role=Worker&isActive=true`

### 2. Get User by ID
```http
GET /api/users/{userId}
```

### 3. Create User
```http
POST /api/users
```
Request:
```json
{
  "username": "worker5",
  "email": "worker5@pump.com",
  "phone": "9876543215",
  "password": "temp123",
  "fullName": "Mahesh Singh",
  "role": "Worker",
  "employeeCode": "EMP005",
  "dateOfJoining": "2024-01-06",
  "salary": 15000
}
```

### 4. Update User
```http
PUT /api/users/{userId}
```

### 5. Delete/Deactivate User
```http
DELETE /api/users/{userId}
```

### 6. Get Worker Performance
```http
GET /api/users/{userId}/performance
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31`

Response:
```json
{
  "success": true,
  "data": {
    "workerId": "uuid",
    "workerName": "Ramesh Kumar",
    "totalShifts": 30,
    "totalSales": 450000,
    "totalBills": 1200,
    "averageSalePerShift": 15000,
    "totalCreditGiven": 50000,
    "shortages": 500,
    "excesses": 200
  }
}
```

---

## ⛽ FUEL TYPE & RATE APIs

### 1. Get All Fuel Types
```http
GET /api/fuel-types
```

### 2. Create Fuel Type
```http
POST /api/fuel-types
```
Request:
```json
{
  "fuelName": "Premium Petrol",
  "fuelCode": "PPTR",
  "unit": "Liters"
}
```

### 3. Get Current Fuel Rates
```http
GET /api/fuel-rates/current
```
Response:
```json
{
  "success": true,
  "data": [
    {
      "fuelTypeId": "uuid",
      "fuelName": "Petrol",
      "rate": 102.50,
      "effectiveFrom": "2024-01-06T00:00:00Z"
    },
    {
      "fuelTypeId": "uuid",
      "fuelName": "Diesel",
      "rate": 89.75,
      "effectiveFrom": "2024-01-06T00:00:00Z"
    }
  ]
}
```

### 4. Update Fuel Rate (Owner/Manager only)
```http
POST /api/fuel-rates
```
Request:
```json
{
  "fuelTypeId": "uuid",
  "rate": 103.00,
  "effectiveFrom": "2024-01-07T06:00:00Z"
}
```

### 5. Get Fuel Rate History
```http
GET /api/fuel-rates/history/{fuelTypeId}
```

---

## 🏭 MACHINE & NOZZLE APIs

### 1. Get All Machines
```http
GET /api/machines
```

### 2. Get Machine by ID
```http
GET /api/machines/{machineId}
```

### 3. Create Machine
```http
POST /api/machines
```
Request:
```json
{
  "machineName": "Machine 1",
  "machineCode": "M001",
  "serialNumber": "SN12345",
  "manufacturer": "Tokheim",
  "location": "Left Side"
}
```

### 4. Get Nozzles by Machine
```http
GET /api/machines/{machineId}/nozzles
```

### 5. Create Nozzle
```http
POST /api/nozzles
```
Request:
```json
{
  "machineId": "uuid",
  "fuelTypeId": "uuid",
  "nozzleNumber": "N1",
  "nozzleName": "Nozzle 1 - Petrol",
  "currentMeterReading": 15000.500
}
```

### 6. Update Nozzle Meter Reading
```http
PATCH /api/nozzles/{nozzleId}/meter-reading
```
Request:
```json
{
  "meterReading": 15500.750
}
```

---

## 🔄 SHIFT MANAGEMENT APIs (CRITICAL)

### 1. Get Active Shifts
```http
GET /api/shifts/active
```
Response:
```json
{
  "success": true,
  "data": [
    {
      "shiftId": "uuid",
      "workerName": "Ramesh Kumar",
      "nozzleNumber": "N1",
      "machineName": "Machine 1",
      "fuelType": "Petrol",
      "shiftType": "Morning",
      "startTime": "2024-01-06T06:00:00Z",
      "openingMeterReading": 15000.500,
      "currentSales": 5500,
      "billsCount": 45
    }
  ]
}
```

### 2. Start Shift
```http
POST /api/shifts/start
```
Request:
```json
{
  "workerId": "uuid",
  "nozzleId": "uuid",
  "shiftType": "Morning",
  "openingMeterReading": 15000.500,
  "openingCash": 500
}
```

### 3. Get Shift Details
```http
GET /api/shifts/{shiftId}
```
Response:
```json
{
  "success": true,
  "data": {
    "shiftId": "uuid",
    "shiftDate": "2024-01-06",
    "shiftType": "Morning",
    "worker": {
      "userId": "uuid",
      "fullName": "Ramesh Kumar",
      "employeeCode": "EMP001"
    },
    "nozzle": {
      "nozzleId": "uuid",
      "nozzleNumber": "N1",
      "machineName": "Machine 1",
      "fuelType": "Petrol",
      "currentRate": 102.50
    },
    "startTime": "2024-01-06T06:00:00Z",
    "openingMeterReading": 15000.500,
    "currentMeterReading": 15500.750,
    "totalSalesAmount": 51125,
    "totalBills": 45,
    "cashReceived": 30000,
    "digitalReceived": 15000,
    "creditGiven": 6125,
    "shiftStatus": "Open"
  }
}
```

### 4. Record Intermediate Reading
```http
POST /api/shifts/{shiftId}/readings
```
Request:
```json
{
  "meterReading": 15250.300,
  "notes": "Lunch break reading"
}
```

### 5. Close Shift
```http
POST /api/shifts/{shiftId}/close
```
Request:
```json
{
  "closingMeterReading": 15500.750,
  "actualCashReceived": 30000,
  "actualDigitalReceived": 15000,
  "notes": "Smooth shift, no issues"
}
```
Response:
```json
{
  "success": true,
  "data": {
    "shiftId": "uuid",
    "totalFuelSold": 500.250,  // Liters
    "totalSalesAmount": 51125,
    "expectedCollection": 45000,  // Sales - Credit
    "actualCollection": 45000,   // Cash + Digital
    "variance": 0,
    "shortageOrExcess": "Balanced"
  }
}
```

### 6. Get Shift Report
```http
GET /api/shifts/{shiftId}/report
```

### 7. Get Shifts by Date Range
```http
GET /api/shifts
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31&workerId=uuid&status=Closed`

---

## 📝 BILLING APIs

### 1. Create Bill
```http
POST /api/bills
```
Request:
```json
{
  "shiftId": "uuid",
  "nozzleId": "uuid",
  "customerName": "John Doe",
  "vehicleNumber": "MH-01-AB-1234",
  "customerPhone": "9876543210",
  "items": [
    {
      "fuelTypeId": "uuid",
      "quantity": 10.500,
      "rate": 102.50,
      "meterReadingBefore": 15000.500,
      "meterReadingAfter": 15011.000
    }
  ],
  "paymentMode": "Cash",  // Cash, UPI, Card, Credit
  "isCreditBill": false,
  "creditCustomerId": null
}
```
Response:
```json
{
  "success": true,
  "data": {
    "billId": "uuid",
    "billNumber": "PUMP001-20240106-0045",
    "billDate": "2024-01-06T10:30:00Z",
    "totalAmount": 1076.25,
    "printUrl": "/api/bills/{billId}/print"
  }
}
```

### 2. Get Bill by ID
```http
GET /api/bills/{billId}
```

### 3. Get Bills
```http
GET /api/bills
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31&shiftId=uuid&paymentMode=Cash&page=1&limit=50`

### 4. Void Bill (Manager/Owner only)
```http
POST /api/bills/{billId}/void
```
Request:
```json
{
  "reason": "Entered wrong quantity"
}
```

### 5. Print Bill
```http
GET /api/bills/{billId}/print
```
Returns thermal printer compatible format

### 6. Get Bill Summary
```http
GET /api/bills/summary
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31`
Response:
```json
{
  "success": true,
  "data": {
    "totalBills": 1500,
    "totalAmount": 1500000,
    "cashAmount": 800000,
    "digitalAmount": 600000,
    "creditAmount": 100000,
    "voidedBills": 5,
    "voidedAmount": 5000
  }
}
```

---

## 💰 CREDIT/UDHAAR MANAGEMENT APIs

### 1. Get All Credit Customers
```http
GET /api/credit-customers
```
Query: `?search=transport&hasOutstanding=true`

### 2. Create Credit Customer
```http
POST /api/credit-customers
```
Request:
```json
{
  "customerName": "ABC Transport Co.",
  "phone": "9876543220",
  "email": "abc@transport.com",
  "address": "123 Transport Nagar",
  "city": "Mumbai",
  "vehicleNumbers": ["MH-01-AB-1234", "MH-01-AB-5678"],
  "creditLimit": 50000
}
```

### 3. Get Customer Credit Details
```http
GET /api/credit-customers/{customerId}
```
Response:
```json
{
  "success": true,
  "data": {
    "customerId": "uuid",
    "customerCode": "CC001",
    "customerName": "ABC Transport Co.",
    "phone": "9876543220",
    "creditLimit": 50000,
    "currentBalance": 15000,
    "availableCredit": 35000,
    "lastTransaction": "2024-01-05",
    "ageing": {
      "0-30days": 10000,
      "31-60days": 5000,
      "60+days": 0
    }
  }
}
```

### 4. Get Credit Transactions
```http
GET /api/credit-customers/{customerId}/transactions
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31`

### 5. Record Payment (Credit Transaction)
```http
POST /api/credit-transactions/payment
```
Request:
```json
{
  "customerId": "uuid",
  "amount": 5000,
  "paymentMode": "Cash",
  "transactionReference": "UPI123456",
  "notes": "Partial payment for December bills"
}
```

### 6. Get Credit Outstanding Report
```http
GET /api/credit-customers/outstanding
```
Response:
```json
{
  "success": true,
  "data": [
    {
      "customerId": "uuid",
      "customerName": "ABC Transport Co.",
      "currentBalance": 15000,
      "creditLimit": 50000,
      "lastPaymentDate": "2024-01-01",
      "oldestDueDate": "2023-12-05",
      "daysSinceLastPayment": 5
    }
  ]
}
```

---

## 🛢️ STOCK & TANK MANAGEMENT APIs

### 1. Get All Tanks
```http
GET /api/tanks
```

### 2. Create Tank
```http
POST /api/tanks
```
Request:
```json
{
  "fuelTypeId": "uuid",
  "tankName": "Tank 1 - Petrol",
  "tankNumber": "T1",
  "capacity": 10000,
  "currentStock": 8000,
  "minimumStockLevel": 1000
}
```

### 3. Get Tank Stock Status
```http
GET /api/tanks/{tankId}/stock
```

### 4. Record Stock Entry (Refill)
```http
POST /api/stock-entries
```
Request:
```json
{
  "tankId": "uuid",
  "entryType": "StockIn",
  "quantity": 5000,
  "rate": 95.50,
  "supplierName": "Indian Oil Corporation",
  "invoiceNumber": "IOC123456",
  "invoiceDate": "2024-01-06"
}
```

### 5. Get Stock History
```http
GET /api/stock-entries
```
Query: `?tankId=uuid&fromDate=2024-01-01&toDate=2024-01-31&entryType=StockIn`

### 6. Get Stock Variance Report
```http
GET /api/tanks/variance
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31`
Response:
```json
{
  "success": true,
  "data": [
    {
      "tankId": "uuid",
      "tankName": "Tank 1 - Petrol",
      "fuelType": "Petrol",
      "openingStock": 8000,
      "stockReceived": 5000,
      "stockSold": 4500,
      "expectedClosing": 8500,
      "actualClosing": 8450,
      "variance": -50,
      "variancePercentage": -0.59
    }
  ]
}
```

### 7. Get Low Stock Alerts
```http
GET /api/tanks/alerts
```

---

## 💸 EXPENSE MANAGEMENT APIs

### 1. Get All Expenses
```http
GET /api/expenses
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31&category=Electricity`

### 2. Create Expense
```http
POST /api/expenses
```
Request:
```json
{
  "expenseDate": "2024-01-06",
  "expenseCategory": "Electricity",
  "amount": 5000,
  "description": "Monthly electricity bill",
  "billNumber": "MSEB123456",
  "vendorName": "Maharashtra State Electricity Board",
  "paymentMode": "UPI"
}
```

### 3. Update Expense
```http
PUT /api/expenses/{expenseId}
```

### 4. Delete Expense
```http
DELETE /api/expenses/{expenseId}
```

### 5. Get Expense Summary
```http
GET /api/expenses/summary
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31`
Response:
```json
{
  "success": true,
  "data": {
    "totalExpenses": 50000,
    "byCategory": {
      "Electricity": 5000,
      "Salary": 30000,
      "Maintenance": 10000,
      "Miscellaneous": 5000
    }
  }
}
```

---

## 📊 REPORTS & ANALYTICS APIs

### 1. Daily Sales Report
```http
GET /api/reports/daily-sales
```
Query: `?date=2024-01-06`
Response:
```json
{
  "success": true,
  "data": {
    "date": "2024-01-06",
    "totalSales": 150000,
    "totalBills": 450,
    "fuelSold": {
      "Petrol": 1000,
      "Diesel": 800
    },
    "paymentModes": {
      "Cash": 80000,
      "UPI": 60000,
      "Credit": 10000
    },
    "byNozzle": [
      {
        "nozzleNumber": "N1",
        "fuelType": "Petrol",
        "sales": 50000,
        "quantity": 500,
        "bills": 150
      }
    ],
    "byWorker": [
      {
        "workerName": "Ramesh Kumar",
        "sales": 50000,
        "bills": 150,
        "shifts": 1
      }
    ]
  }
}
```

### 2. Monthly Sales Report
```http
GET /api/reports/monthly-sales
```
Query: `?month=1&year=2024`

### 3. Worker Performance Report
```http
GET /api/reports/worker-performance
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31&workerId=uuid`

### 4. Credit Ageing Report
```http
GET /api/reports/credit-ageing
```

### 5. Profit & Margin Analysis
```http
GET /api/reports/profit-analysis
```
Query: `?fromDate=2024-01-01&toDate=2024-01-31`
Response:
```json
{
  "success": true,
  "data": {
    "totalRevenue": 1500000,
    "totalCost": 1400000,
    "grossProfit": 100000,
    "profitMargin": 6.67,
    "byFuelType": [
      {
        "fuelType": "Petrol",
        "revenue": 900000,
        "cost": 840000,
        "profit": 60000,
        "margin": 6.67
      }
    ]
  }
}
```

### 6. Dashboard Stats
```http
GET /api/reports/dashboard
```
Response:
```json
{
  "success": true,
  "data": {
    "today": {
      "sales": 50000,
      "bills": 150,
      "fuelSold": 500
    },
    "thisMonth": {
      "sales": 1500000,
      "bills": 4500,
      "profit": 100000
    },
    "activeShifts": 3,
    "lowStockAlerts": 1,
    "pendingCredits": 50000,
    "todayExpenses": 5000
  }
}
```

### 7. Export Report
```http
GET /api/reports/export
```
Query: `?reportType=daily-sales&date=2024-01-06&format=pdf` or `format=excel`

Returns downloadable PDF/Excel file

---

## 🔍 AUDIT LOG APIs

### 1. Get Audit Logs
```http
GET /api/audit-logs
```
Query: `?fromDate=2024-01-01&userId=uuid&action=Updated&entityType=Bill`

---

## 📱 NOTIFICATION APIs (Future)

### 1. Get Notifications
```http
GET /api/notifications
```

### 2. Mark as Read
```http
PATCH /api/notifications/{notificationId}/read
```

---

## 🔧 Middleware & Filters

### Tenant Resolution Middleware
```csharp
// Extracts TenantId from JWT token
// Sets in HttpContext for use in controllers
// Applied to all tenant-specific endpoints
```

### Global Query Filter
```csharp
// Automatically filters all queries by TenantId
// Applied in EF Core DbContext
```

### Rate Limiting
```http
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1704537600
```

---

## 📄 Example API Flow: Create Bill

```
1. GET /api/fuel-rates/current
   → Get current petrol rate (₹102.50)

2. POST /api/bills
   → Create bill for 10 liters
   → Amount: 10 × 102.50 = ₹1025
   → Payment: Cash

3. Backend:
   → Validate shift is active
   → Create bill record
   → Create bill item
   → Update shift totals
   → Deduct from tank stock
   → Return bill details

4. GET /api/bills/{billId}/print
   → Generate thermal printer format
```

---

**Next Steps**: Proceed to Worker Shift Flow Diagram
