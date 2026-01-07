# Worker Shift Management Flow Diagram

## 🔄 Complete Shift Lifecycle

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                     SHIFT MANAGEMENT COMPLETE FLOW                           │
└─────────────────────────────────────────────────────────────────────────────┘

                              ┌──────────────┐
                              │  SHIFT START │
                              └──────┬───────┘
                                     │
                    ┌────────────────┴────────────────┐
                    │                                  │
              ┌─────▼─────┐                    ┌──────▼──────┐
              │  Worker   │                    │  Manager    │
              │  Login    │                    │  Assigns    │
              └─────┬─────┘                    │  Worker to  │
                    │                          │  Nozzle     │
                    │                          └──────┬──────┘
                    │                                 │
                    └────────────┬────────────────────┘
                                 │
                        ┌────────▼────────┐
                        │ Record Opening  │
                        │ Meter Reading   │
                        └────────┬────────┘
                                 │
                        ┌────────▼────────┐
                        │ Record Opening  │
                        │ Cash in Hand    │
                        └────────┬────────┘
                                 │
                        ┌────────▼────────┐
                        │ Shift Status:   │
                        │ ACTIVE          │
                        └────────┬────────┘
                                 │
                    ┌────────────┴────────────┐
                    │                          │
              ┌─────▼─────┐            ┌──────▼──────┐
              │  DURING   │            │  PARALLEL   │
              │  SHIFT    │            │  TRACKING   │
              └─────┬─────┘            └──────┬──────┘
                    │                          │
        ┌───────────┼───────────┐             │
        │           │           │             │
   ┌────▼────┐ ┌───▼────┐ ┌───▼────┐   ┌────▼────┐
   │ Create  │ │ Record │ │ Handle │   │ System  │
   │ Bills   │ │Payments│ │ Credit │   │Auto-Calc│
   └────┬────┘ └───┬────┘ └───┬────┘   └────┬────┘
        │          │          │             │
        └──────────┼──────────┼─────────────┘
                   │          │
          ┌────────▼──────────▼─────────┐
          │ Continuous Updates:          │
          │ - Total Sales                │
          │ - Cash/Digital collected     │
          │ - Credit given               │
          │ - Bills count                │
          │ - Current meter reading      │
          └────────┬─────────────────────┘
                   │
         ┌─────────▼──────────┐
         │  Optional: Record  │
         │ Intermediate Meter │
         │  Readings (lunch,  │
         │  break times)      │
         └─────────┬──────────┘
                   │
              ┌────▼────┐
              │ SHIFT   │
              │  END    │
              └────┬────┘
                   │
         ┌─────────▼──────────┐
         │ Manager Initiates  │
         │   Shift Close      │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │ Record Closing     │
         │ Meter Reading      │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │ Auto Calculate:    │
         │ Fuel Sold =        │
         │ Closing - Opening  │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │ Expected Amount:   │
         │ (Fuel × Rate)      │
         │ - Credit Given     │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │ Record Actual      │
         │ Cash + Digital     │
         │ Received           │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │ Calculate Variance │
         │ (Shortage/Excess)  │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │ Manager Review &   │
         │ Approval           │
         └─────────┬──────────┘
                   │
              ┌────▼────┐
              │ Shift   │
              │ CLOSED  │
              └────┬────┘
                   │
         ┌─────────▼──────────┐
         │ RECONCILIATION     │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │ Compare:           │
         │ Meter vs Billing   │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │ Generate Detailed  │
         │ Shift Report       │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │ Update Worker      │
         │ Accountability     │
         └─────────┬──────────┘
                   │
              ┌────▼────┐
              │  END    │
              └─────────┘
```

---

## 📋 Detailed Step-by-Step Process

### PHASE 1: SHIFT START

#### Step 1.1: Worker Login
```
Worker → Mobile/Web App → Enter credentials
                        → Select Pump (if multiple)
                        → Login
```

#### Step 1.2: Manager Assignment
```
Manager Dashboard → "Assign Worker to Shift"
                  → Select Worker: Ramesh Kumar
                  → Select Nozzle: N1 (Machine 1 - Petrol)
                  → Select Shift Type: Morning/Afternoon/Evening/Night
                  → Click "Start Shift"
```

#### Step 1.3: Record Opening Meter
```
System → Displays Nozzle current reading: 15000.500 L
Manager/Worker → Physically verify meter reading
               → Confirm or correct if needed
               → Record: 15000.500 L
```

#### Step 1.4: Record Opening Cash
```
Manager → Enter opening cash in hand: ₹500
        → (Optional for petty change)
```

#### Step 1.5: Shift Activation
```
Database INSERT:
{
  ShiftId: new UUID,
  WorkerId: ramesh_uuid,
  NozzleId: n1_uuid,
  ShiftDate: 2024-01-06,
  ShiftType: "Morning",
  StartTime: 2024-01-06 06:00:00,
  OpeningMeterReading: 15000.500,
  OpeningCash: 500,
  ShiftStatus: "Open"
}
```

---

### PHASE 2: DURING SHIFT (Operations)

#### Operation A: Create Bill

```
┌─────────────────────────────────────┐
│       CREATE BILL FLOW              │
└─────────────────────────────────────┘

1. Customer arrives → Requests fuel
   ↓
2. Worker selects fuel type: Petrol
   ↓
3. Worker notes meter BEFORE: 15000.500
   ↓
4. Dispenses fuel → Meter AFTER: 15010.500
   ↓
5. System auto-calculates:
   Quantity = 15010.500 - 15000.500 = 10 L
   Rate = ₹102.50 (current rate from database)
   Amount = 10 × 102.50 = ₹1,025
   ↓
6. Enter customer details (optional):
   - Name: John Doe
   - Vehicle: MH-01-AB-1234
   - Phone: 9876543210
   ↓
7. Select payment mode:
   ┌─────────┬─────────┬─────────┬─────────┐
   │  Cash   │   UPI   │  Card   │ Credit  │
   └─────────┴─────────┴─────────┴─────────┘
   ↓
8. If Cash/UPI/Card → Record payment immediately
   If Credit → Link to credit customer
   ↓
9. Generate Bill Number: PUMP001-20240106-0045
   ↓
10. Save bill to database
    ↓
11. Update shift totals:
    - TotalSalesAmount += 1025
    - TotalCashReceived += 1025 (if cash)
    - TotalBills++
    ↓
12. Update tank stock:
    - Tank.CurrentStock -= 10
    ↓
13. Print thermal receipt
    ↓
14. Customer receives receipt → Transaction complete
```

#### Operation B: Handle Credit Transaction

```
┌─────────────────────────────────────┐
│     CREDIT BILL FLOW                │
└─────────────────────────────────────┘

1. Worker selects "Credit Sale"
   ↓
2. Search credit customer:
   - By phone: 9876543220
   - By name: "ABC Transport"
   ↓
3. System displays:
   Customer: ABC Transport Co.
   Credit Limit: ₹50,000
   Current Balance: ₹15,000
   Available Credit: ₹35,000
   ↓
4. Dispense fuel → Amount: ₹3,000
   ↓
5. Check: 15,000 + 3,000 = 18,000 < 50,000 ✓
   ↓
6. Create bill with IsCreditBill = TRUE
   ↓
7. Update customer balance:
   CurrentBalance = 15,000 + 3,000 = ₹18,000
   ↓
8. Create credit transaction record:
   {
     TransactionType: "Debit",
     Amount: 3000,
     BalanceBefore: 15000,
     BalanceAfter: 18000,
     WorkerId: ramesh_uuid,
     ShiftId: current_shift_uuid
   }
   ↓
9. Update shift totals:
   - TotalSalesAmount += 3000
   - TotalCreditGiven += 3000
   ↓
10. Print receipt with "CREDIT" stamp
```

#### Operation C: Record Payment from Credit Customer

```
1. Customer comes to pay outstanding balance
   ↓
2. Manager/Worker selects "Record Payment"
   ↓
3. Select customer: ABC Transport Co.
   Current Balance: ₹18,000
   ↓
4. Enter payment amount: ₹5,000
   ↓
5. Select payment mode: Cash/UPI/Card
   ↓
6. Create credit transaction:
   {
     TransactionType: "Credit",
     Amount: 5000,
     BalanceBefore: 18000,
     BalanceAfter: 13000,
     PaymentMode: "Cash"
   }
   ↓
7. Update customer balance: ₹13,000
   ↓
8. If during shift:
   TotalCashReceived += 5000 (not part of sales)
   ↓
9. Generate payment receipt
```

#### Operation D: Intermediate Meter Readings

```
Purpose: Track meter reading at break times for accountability

1. Worker takes lunch break (12:00 PM)
   ↓
2. Record intermediate meter: 15250.300
   ↓
3. System calculates fuel sold so far:
   15250.300 - 15000.500 = 249.8 L
   ↓
4. Compare with bills created:
   Sum of bill quantities = 250 L
   Variance = 0.2 L (acceptable)
   ↓
5. Worker resumes work
```

---

### PHASE 3: SHIFT END (Closure)

#### Step 3.1: Initiate Shift Close
```
Worker completes shift duration (e.g., 8 hours)
                ↓
Worker → Clicks "End Shift" in app
                ↓
Notification sent to Manager
                ↓
Manager → Opens shift details
```

#### Step 3.2: Record Closing Meter Reading
```
Manager/Worker → Physically check nozzle meter
                → Current reading: 15500.750 L
                → Enter in system
                ↓
System validates: 15500.750 > 15000.500 ✓
```

#### Step 3.3: Auto Calculations
```
Total Fuel Sold = Closing - Opening
                = 15500.750 - 15000.500
                = 500.250 Liters

Total Sales Amount (from bills) = ₹51,125

Expected Collection = Total Sales - Credit Given
                    = 51,125 - 6,125
                    = ₹45,000

Actual Collection = Cash + Digital
                  = 30,000 + 15,000
                  = ₹45,000

Variance = Actual - Expected
         = 45,000 - 45,000
         = ₹0 (Balanced)
```

#### Step 3.4: Fuel vs Billing Verification
```
Physical Fuel Sold (Meter) = 500.250 L
Billed Fuel (from bills)   = SUM(all bill quantities)
                           = 500.500 L

Difference = 500.500 - 500.250 = 0.250 L
Variance % = (0.250 / 500.250) × 100 = 0.05%

Status: ✓ Within acceptable range (<0.5%)
```

#### Step 3.5: Manager Review
```
Manager Dashboard shows:

┌─────────────────────────────────────────┐
│      SHIFT CLOSURE SUMMARY              │
├─────────────────────────────────────────┤
│ Worker: Ramesh Kumar                    │
│ Shift: Morning (06:00 - 14:00)          │
│ Nozzle: N1 - Machine 1 - Petrol         │
├─────────────────────────────────────────┤
│ Opening Meter: 15000.500 L              │
│ Closing Meter: 15500.750 L              │
│ Fuel Sold:     500.250 L                │
├─────────────────────────────────────────┤
│ Total Sales:   ₹51,125                  │
│ Total Bills:   45                       │
├─────────────────────────────────────────┤
│ Cash Received:    ₹30,000               │
│ Digital Received: ₹15,000               │
│ Credit Given:     ₹6,125                │
├─────────────────────────────────────────┤
│ Expected Collection: ₹45,000            │
│ Actual Collection:   ₹45,000            │
│ Variance:            ₹0 ✓               │
├─────────────────────────────────────────┤
│ Meter vs Billing:    0.25 L (0.05%) ✓   │
└─────────────────────────────────────────┘

Manager Actions:
[ Approve & Close Shift ]  [ Request Clarification ]
```

#### Step 3.6: Approve & Close
```
Manager → Reviews all details
        → Clicks "Approve & Close Shift"
        ↓
Database UPDATE:
{
  ShiftId: current_uuid,
  EndTime: 2024-01-06 14:00:00,
  ClosingMeterReading: 15500.750,
  TotalFuelSold: 500.250,
  ShiftStatus: "Closed",
  ClosedBy: manager_uuid
}
        ↓
Worker accountability updated
        ↓
Shift report generated
```

---

### PHASE 4: RECONCILIATION & REPORTING

#### Reconciliation Checks

```
1. Meter Reading Validation
   ✓ Closing > Opening
   ✓ Meter reading matches physical dispenser

2. Sales vs Collection
   ✓ Total Sales = Cash + Digital + Credit
   ✓ Variance within acceptable limit

3. Fuel Dispensed vs Billed
   ✓ Meter difference ≈ Sum of bill quantities
   ✓ Variance < 0.5%

4. Stock Deduction
   ✓ Tank stock reduced by fuel sold
   ✓ No negative stock

5. Credit Accountability
   ✓ All credit bills linked to customers
   ✓ Customer balances updated
```

#### Shift Report Generated

```
═════════════════════════════════════════
    SHIFT PERFORMANCE REPORT
═════════════════════════════════════════
Shift ID: SH-20240106-001
Worker: Ramesh Kumar (EMP001)
Nozzle: N1 - Machine 1 - Petrol
Date: 06-Jan-2024
Shift Type: Morning
Duration: 06:00 AM - 02:00 PM (8 hours)
─────────────────────────────────────────
FUEL DISPENSED
─────────────────────────────────────────
Opening Meter:    15,000.500 L
Closing Meter:    15,500.750 L
Total Fuel Sold:  500.250 L
Current Rate:     ₹102.50/L
─────────────────────────────────────────
SALES SUMMARY
─────────────────────────────────────────
Total Bills:      45
Total Sales:      ₹51,125
Average per Bill: ₹1,136
─────────────────────────────────────────
PAYMENT BREAKDOWN
─────────────────────────────────────────
Cash:             ₹30,000 (58.7%)
Digital (UPI):    ₹15,000 (29.3%)
Credit:           ₹6,125  (12.0%)
─────────────────────────────────────────
RECONCILIATION
─────────────────────────────────────────
Expected Amount:  ₹45,000
Received Amount:  ₹45,000
Variance:         ₹0 ✓ BALANCED
─────────────────────────────────────────
ACCOUNTABILITY
─────────────────────────────────────────
Meter vs Billing: 0.25 L (0.05%) ✓ OK
Credit Given:     3 transactions
Cash Deposited:   ₹30,000 ✓
─────────────────────────────────────────
APPROVED BY: Suresh Patel (Manager)
APPROVED AT: 06-Jan-2024 02:15 PM
═════════════════════════════════════════
```

---

## 🚨 Exception Handling Scenarios

### Scenario 1: Shortage in Cash
```
Expected Collection: ₹45,000
Actual Cash + Digital: ₹44,500
Shortage: ₹500

Manager Action:
1. Review all bills for the shift
2. Check for missing payments
3. Worker provides explanation
4. Manager can:
   - Approve with note (deduct from worker salary)
   - Request worker to pay shortage
   - Mark for investigation
```

### Scenario 2: Excess Cash
```
Expected Collection: ₹45,000
Actual Cash + Digital: ₹45,500
Excess: ₹500

Possible Reasons:
- Previous shift pending amount deposited
- Customer payment for old credit
- Error in recording

Manager Action:
1. Worker provides explanation
2. Manager verifies and approves
3. Excess added to cash in hand for next shift
```

### Scenario 3: Meter vs Billing Mismatch
```
Meter Fuel Sold: 500.250 L
Billed Fuel: 510.500 L
Difference: 10.250 L (2.05%) ⚠️ HIGH

Possible Reasons:
- Bill recorded without actual fuel dispensing
- Meter reading error
- Fuel leakage/spillage
- Fraud

Manager Action:
1. Review all bills for anomalies
2. Check for duplicate bills
3. Investigate worker
4. Cannot close shift until resolved
```

### Scenario 4: System Failure During Shift
```
Problem: Internet connection lost, system down

Fallback:
1. Worker maintains manual bill register
2. Records:
   - Vehicle number
   - Quantity dispensed
   - Amount
   - Payment mode
3. After system restoration:
   - Manager enters all bills manually
   - Verifies meter reading
   - Completes shift closure
```

---

## 🔐 Security & Accountability

### Data Integrity
```
1. Meter readings cannot be edited after shift close
2. All changes logged in audit trail
3. Manager approval required for shift closure
4. Worker cannot close own shift
5. Bills cannot be deleted, only voided (with reason)
```

### Worker Accountability
```
Track per worker:
- Total shifts worked
- Average sales per shift
- Shortage/excess history
- Credit issued (for tracking limits)
- Customer complaints (if any)
```

### Fraud Prevention
```
1. Meter reading must increase monotonically
2. Bill quantity must match meter difference
3. Payment modes cross-verified
4. Credit customers pre-approved
5. Manager notifications for:
   - Large cash shortages
   - Excessive credit in one shift
   - High meter vs billing variance
```

---

## 📱 UI/UX Flow

### Worker Mobile App View
```
┌─────────────────────────────────┐
│  ⚡ Active Shift                 │
│  Ramesh Kumar • Nozzle N1       │
├─────────────────────────────────┤
│  Started: 06:00 AM              │
│  Duration: 4h 30m               │
├─────────────────────────────────┤
│  📊 Today's Performance         │
│  Sales: ₹25,500                 │
│  Bills: 22                      │
│  Fuel: 250 L                    │
├─────────────────────────────────┤
│  [+ Create Bill]                │
│  [💰 Record Payment]            │
│  [📝 View Bills]                │
│  [🛑 Request Shift End]         │
└─────────────────────────────────┘
```

### Manager Dashboard View
```
┌─────────────────────────────────┐
│  📍 Active Shifts (3)            │
├─────────────────────────────────┤
│  • Ramesh | N1 | ₹25,500        │
│  • Dinesh | N2 | ₹18,200        │
│  • Mahesh | N3 | ₹22,800        │
├─────────────────────────────────┤
│  [+ Assign New Shift]           │
│  [📊 View All Shifts]           │
│  [⚠️ Close Pending Shifts (1)]  │
└─────────────────────────────────┘
```

---

**Next Steps**: Proceed to UI/Dashboard Wireframes
