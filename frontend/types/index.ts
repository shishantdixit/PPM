// API Response wrapper
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors: ErrorDetail[] | null;
  timestamp: string;
}

export interface ErrorDetail {
  field: string;
  message: string;
}

// Auth Types
export interface LoginRequest {
  username: string;
  password: string;
  tenantCode?: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  expiresIn: number;
  user: UserInfo;
}

export interface UserInfo {
  userId: string;
  username: string;
  fullName: string;
  email: string;
  role: string;
  tenantId: string | null;
  tenantCode: string | null;
  tenantName: string | null;
  isSuperAdmin: boolean;
}

// Fuel Types
export interface FuelType {
  fuelTypeId: string;
  tenantId: string;
  fuelName: string;
  fuelCode: string;
  unit: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateFuelTypeDto {
  fuelName: string;
  fuelCode: string;
  unit: string;
}

export interface UpdateFuelTypeDto {
  fuelName?: string;
  unit?: string;
  isActive?: boolean;
}

// Fuel Rates
export interface FuelRate {
  fuelRateId: string;
  tenantId: string;
  fuelTypeId: string;
  fuelName: string;
  fuelCode: string;
  rate: number;
  effectiveFrom: string;
  effectiveTo: string | null;
  updatedBy: string | null;
  updatedByName: string | null;
  createdAt: string;
  isCurrent: boolean;
}

export interface CurrentFuelRate {
  fuelTypeId: string;
  fuelName: string;
  fuelCode: string;
  unit: string;
  currentRate: number;
  effectiveFrom: string;
}

export interface CreateFuelRateDto {
  fuelTypeId: string;
  rate: number;
  effectiveFrom: string;
}

// Machines
export interface Machine {
  machineId: string;
  tenantId: string;
  machineName: string;
  machineCode: string;
  serialNumber: string | null;
  manufacturer: string | null;
  model: string | null;
  installationDate: string | null;
  location: string | null;
  isActive: boolean;
  nozzleCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateMachineDto {
  machineName: string;
  machineCode: string;
  serialNumber?: string;
  manufacturer?: string;
  model?: string;
  installationDate?: string;
  location?: string;
}

export interface UpdateMachineDto {
  machineName?: string;
  serialNumber?: string;
  manufacturer?: string;
  model?: string;
  installationDate?: string;
  location?: string;
  isActive?: boolean;
}

// Nozzles
export interface Nozzle {
  nozzleId: string;
  tenantId: string;
  machineId: string;
  machineName: string;
  machineCode: string;
  fuelTypeId: string;
  fuelName: string;
  fuelCode: string;
  nozzleNumber: string;
  nozzleName: string | null;
  currentMeterReading: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateNozzleDto {
  machineId: string;
  fuelTypeId: string;
  nozzleNumber: string;
  nozzleName?: string;
  currentMeterReading: number;
}

export interface UpdateNozzleDto {
  nozzleName?: string;
  currentMeterReading?: number;
  isActive?: boolean;
}

// Shifts
export enum ShiftStatus {
  Pending = 0,
  Active = 1,
  Closed = 2
}

export enum PaymentMethod {
  Cash = 0,
  Credit = 1,
  Digital = 2,
  Mixed = 3
}

export interface ShiftNozzleReadingDto {
  shiftNozzleReadingId: string;
  shiftId: string;
  nozzleId: string;
  nozzleNumber: string;
  nozzleName: string;
  machineName: string;
  fuelName: string;
  fuelCode: string;
  openingReading: number;
  closingReading: number | null;
  quantitySold: number;
  rateAtShift: number;
  expectedAmount: number;
}

export interface Shift {
  shiftId: string;
  tenantId: string;
  workerId: string;
  workerName: string;
  machineId: string;
  machineName: string;
  shiftDate: string; // DateOnly as string (YYYY-MM-DD)
  startTime: string; // TimeOnly as string (HH:mm:ss)
  endTime: string | null;
  status: ShiftStatus;
  totalSales: number;
  cashCollected: number;
  creditSales: number;
  digitalPayments: number;
  borrowing: number;
  variance: number;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
  nozzleReadings: ShiftNozzleReadingDto[];
  fuelSales?: FuelSale[];
}

export interface NozzleReadingInput {
  nozzleId: string;
  reading: number;
}

export interface CreateShiftDto {
  workerId?: string; // Optional - defaults to logged-in user
  machineId: string; // Required - machine the worker is assigned to
  shiftDate: string; // YYYY-MM-DD
  startTime: string; // HH:mm:ss
  openingReadings: NozzleReadingInput[]; // If empty, all nozzles on machine are included
}

export interface ShiftMachine {
  machineId: string;
  machineName: string;
  machineCode: string;
  location: string | null;
  nozzleCount: number;
}

export interface ShiftNozzle {
  nozzleId: string;
  nozzleNumber: string;
  nozzleName: string | null;
  currentMeterReading: number;
  fuelTypeId: string;
  fuelName: string;
  fuelCode: string;
}

export interface Worker {
  userId: string;
  fullName: string;
  username: string;
}

export interface CloseShiftDto {
  endTime: string; // HH:mm:ss
  closingReadings: NozzleReadingInput[];
  cashCollected: number;
  creditSales: number;
  digitalPayments: number;
  borrowing: number;
  notes?: string;
}

export interface FuelSale {
  fuelSaleId: string;
  shiftId: string;
  nozzleId: string;
  saleNumber: string;
  nozzleNumber: string;
  fuelName: string;
  quantity: number;
  rate: number;
  amount: number;
  paymentMethod: PaymentMethod;
  customerName: string | null;
  customerPhone: string | null;
  vehicleNumber: string | null;
  saleTime: string;
  notes: string | null;
  isVoided: boolean;
  voidedAt: string | null;
  voidedBy: string | null;
  voidReason: string | null;
}

export interface CreateFuelSaleDto {
  nozzleId: string;
  quantity: number;
  paymentMethod: PaymentMethod;
  customerName?: string;
  customerPhone?: string;
  vehicleNumber?: string;
  notes?: string;
}

export interface VoidFuelSaleDto {
  reason: string;
}

// User Management
export interface User {
  userId: string;
  username: string;
  email: string | null;
  phone: string;
  fullName: string;
  role: string;
  employeeCode: string | null;
  dateOfJoining: string | null;
  salary: number | null;
  isActive: boolean;
  lastLoginAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface UserProfile extends User {
  totalShifts: number;
  shiftsLast30Days: number;
  totalSales: number;
  salesLast30Days: number;
  averageVariance: number;
}

export interface CreateUserDto {
  username: string;
  password: string;
  email?: string;
  phone: string;
  fullName: string;
  role: string;
  employeeCode?: string;
  dateOfJoining?: string;
  salary?: number;
}

export interface UpdateUserDto {
  username: string;
  email?: string;
  phone: string;
  fullName: string;
  role: string;
  employeeCode?: string;
  dateOfJoining?: string;
  salary?: number;
}

// Reports & Analytics
export interface DashboardSummary {
  date: string;
  todaySales: number;
  todayQuantity: number;
  todaySalesCount: number;
  activeShifts: number;
  weekSales: number;
  weekQuantity: number;
  monthSales: number;
  monthQuantity: number;
  salesChangePercent: number;
  weekChangePercent: number;
  monthChangePercent: number;
  topFuelTypes: FuelTypePerformance[];
  last7DaysTrend: SalesTrend[];
}

export interface SalesTrend {
  date: string;
  totalAmount: number;
  totalQuantity: number;
  salesCount: number;
}

export interface FuelTypePerformance {
  fuelTypeId: string;
  fuelName: string;
  fuelCode: string;
  totalQuantity: number;
  totalAmount: number;
  averageRate: number;
  salesCount: number;
  percentageOfTotal: number;
}

export interface DailySalesSummary {
  date: string;
  totalShifts: number;
  totalSales: number;
  voidedSales: number;
  totalQuantity: number;
  totalAmount: number;
  cashAmount: number;
  creditAmount: number;
  digitalAmount: number;
  salesByFuelType: FuelSalesByType[];
}

export interface FuelSalesByType {
  fuelTypeId: string;
  fuelName: string;
  fuelCode: string;
  quantity: number;
  amount: number;
  averageRate: number;
}

export interface ShiftSummaryReport {
  shiftId: string;
  shiftDate: string;
  startTime: string;
  endTime: string | null;
  workerName: string;
  status: ShiftStatus;
  salesCount: number;
  totalQuantity: number;
  totalSales: number;
  cashCollected: number;
  creditSales: number;
  digitalPayments: number;
  variance: number;
}

export interface WorkerPerformance {
  userId: string;
  workerName: string;
  totalShifts: number;
  totalSales: number;
  totalAmount: number;
  totalQuantity: number;
  averageShiftSales: number;
  totalVariance: number;
}

export interface NozzlePerformance {
  nozzleId: string;
  nozzleNumber: string;
  machineName: string;
  fuelType: string;
  totalQuantity: number;
  totalAmount: number;
  salesCount: number;
}

// Credit Customer Management
export enum CreditTransactionType {
  Sale = 0,
  Payment = 1,
  Adjustment = 2,
  Refund = 3
}

export enum PaymentModeCredit {
  Cash = 0,
  Cheque = 1,
  BankTransfer = 2,
  UPI = 3,
  Card = 4,
  Other = 5
}

export interface CreditCustomer {
  creditCustomerId: string;
  customerCode: string;
  customerName: string;
  contactPerson: string | null;
  phone: string;
  email: string | null;
  address: string | null;
  creditLimit: number;
  currentBalance: number;
  availableCredit: number;
  paymentTermDays: number;
  isActive: boolean;
  isBlocked: boolean;
  blockReason: string | null;
  vehicleNumbers: string | null;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateCreditCustomerDto {
  customerCode: string;
  customerName: string;
  contactPerson?: string;
  phone: string;
  email?: string;
  address?: string;
  creditLimit: number;
  paymentTermDays?: number;
  vehicleNumbers?: string;
  notes?: string;
}

export interface UpdateCreditCustomerDto {
  customerName?: string;
  contactPerson?: string;
  phone?: string;
  email?: string;
  address?: string;
  creditLimit?: number;
  paymentTermDays?: number;
  vehicleNumbers?: string;
  notes?: string;
  isActive?: boolean;
}

export interface CreditTransaction {
  creditTransactionId: string;
  creditCustomerId: string;
  customerName: string;
  transactionType: CreditTransactionType;
  transactionTypeName: string;
  amount: number;
  balanceAfter: number;
  transactionDate: string;
  fuelSaleId: string | null;
  saleNumber: string | null;
  paymentMode: PaymentModeCredit | null;
  paymentModeName: string | null;
  paymentReference: string | null;
  notes: string | null;
  createdBy: string | null;
  createdAt: string;
}

export interface RecordPaymentDto {
  amount: number;
  paymentMode: PaymentModeCredit;
  paymentReference?: string;
  notes?: string;
}

export interface AdjustBalanceDto {
  amount: number;
  notes: string;
}

export interface BlockCustomerDto {
  reason: string;
}

export interface CreditCustomerSummary {
  totalCustomers: number;
  activeCustomers: number;
  blockedCustomers: number;
  totalOutstanding: number;
  totalCreditLimit: number;
  topDebtors: CreditCustomer[];
  overdueCustomers: CreditCustomer[];
}

export interface CustomerStatement {
  customer: CreditCustomer;
  fromDate: string;
  toDate: string;
  openingBalance: number;
  totalSales: number;
  totalPayments: number;
  closingBalance: number;
  transactions: CreditTransaction[];
}

// Expense Management
export enum ExpenseCategory {
  Electricity = 0,
  Rent = 1,
  Salary = 2,
  Maintenance = 3,
  EquipmentRepair = 4,
  OfficeSupplies = 5,
  Transportation = 6,
  Taxes = 7,
  Insurance = 8,
  Marketing = 9,
  Utilities = 10,
  Communication = 11,
  BankCharges = 12,
  LicenseFees = 13,
  Other = 99
}

export interface Expense {
  expenseId: string;
  tenantId: string;
  category: ExpenseCategory;
  categoryName: string;
  description: string;
  amount: number;
  expenseDate: string;
  paymentMode: PaymentMethod;
  paymentModeName: string;
  reference: string | null;
  vendor: string | null;
  notes: string | null;
  recordedById: string;
  recordedByName: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateExpenseDto {
  category: ExpenseCategory;
  description: string;
  amount: number;
  expenseDate: string;
  paymentMode: PaymentMethod;
  reference?: string;
  vendor?: string;
  notes?: string;
}

export interface UpdateExpenseDto {
  category: ExpenseCategory;
  description: string;
  amount: number;
  expenseDate: string;
  paymentMode: PaymentMethod;
  reference?: string;
  vendor?: string;
  notes?: string;
}

export interface ExpenseSummary {
  totalExpenses: number;
  byCategory: CategorySummary[];
  byDate: DailyExpenseSummary[];
}

export interface CategorySummary {
  category: ExpenseCategory;
  categoryName: string;
  amount: number;
  count: number;
}

export interface DailyExpenseSummary {
  date: string;
  amount: number;
  count: number;
}

// Super Admin Types
export interface Tenant {
  tenantId: string;
  tenantCode: string;
  companyName: string;
  ownerName: string;
  email: string;
  phone: string;
  address: string | null;
  city: string | null;
  state: string | null;
  country: string;
  pinCode: string | null;
  subscriptionPlan: string;
  subscriptionStatus: string;
  subscriptionStartDate: string;
  subscriptionEndDate: string | null;
  maxMachines: number;
  maxWorkers: number;
  maxMonthlyBills: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface TenantSummary {
  tenantId: string;
  tenantCode: string;
  companyName: string;
  ownerName: string;
  email: string;
  phone: string;
  city: string | null;
  state: string | null;
  subscriptionPlan: string;
  subscriptionStatus: string;
  subscriptionStartDate: string;
  subscriptionEndDate: string | null;
  maxMachines: number;
  maxWorkers: number;
  isActive: boolean;
  createdAt: string;
  userCount: number;
  machineCount: number;
  nozzleCount: number;
  totalSales: number;
  thisMonthSales: number;
  totalShifts: number;
}

export interface TenantDetail extends TenantSummary {
  address: string | null;
  pinCode: string | null;
  country: string;
  maxMonthlyBills: number;
  updatedAt: string;
  users: TenantUser[];
  salesHistory: MonthlySales[];
}

export interface TenantUser {
  userId: string;
  username: string;
  fullName: string;
  role: string;
  isActive: boolean;
  lastLoginAt: string | null;
}

export interface MonthlySales {
  month: string;
  sales: number;
  shiftCount: number;
}

export interface SystemDashboard {
  totalTenants: number;
  activeTenants: number;
  suspendedTenants: number;
  expiredTenants: number;
  totalUsers: number;
  totalSalesAllTime: number;
  totalSalesThisMonth: number;
  topTenants: TenantSummary[];
  monthlyGrowth: MonthlyStats[];
  subscriptionBreakdown: SubscriptionBreakdown;
}

export interface MonthlyStats {
  month: string;
  newTenants: number;
  totalSales: number;
}

export interface SubscriptionBreakdown {
  basic: number;
  premium: number;
  enterprise: number;
}

export interface CreateTenantDto {
  tenantCode: string;
  companyName: string;
  ownerName: string;
  email: string;
  phone: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  pinCode?: string;
  subscriptionPlan?: string;
  maxMachines?: number;
  maxWorkers?: number;
  maxMonthlyBills?: number;
}

export interface UpdateTenantDto {
  companyName?: string;
  ownerName?: string;
  email?: string;
  phone?: string;
  address?: string;
  city?: string;
  state?: string;
  pinCode?: string;
  maxMachines?: number;
  maxWorkers?: number;
}

export interface UpdateSubscriptionDto {
  subscriptionPlan?: string;
  subscriptionEndDate?: string;
  maxMachines?: number;
  maxWorkers?: number;
  maxMonthlyBills?: number;
}

export interface CreateOwnerUserDto {
  username: string;
  password: string;
  fullName: string;
  email: string;
  phone: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  limit: number;
}

// Feature Management Types
export interface Feature {
  featureId: string;
  featureCode: string;
  featureName: string;
  description: string;
  module: string | null;
  icon: string | null;
  displayOrder: number;
  isActive: boolean;
}

export interface PlanFeature {
  featureId: string;
  featureCode: string;
  featureName: string;
  description: string;
  module: string | null;
  icon: string | null;
  isEnabled: boolean;
}

export interface TenantFeature {
  featureId: string;
  featureCode: string;
  featureName: string;
  description: string;
  module: string | null;
  icon: string | null;
  isEnabled: boolean;
  isOverridden: boolean;
  planDefault: boolean;
  overriddenBy: string | null;
  overriddenAt: string | null;
}

export interface UpdateTenantFeatureDto {
  featureId: string;
  isEnabled: boolean;
}

export interface TenantSubscription {
  subscriptionPlan: string;
  subscriptionStatus: string;
  subscriptionStartDate: string;
  subscriptionEndDate: string | null;
  daysRemaining: number;
  maxMachines: number;
  maxWorkers: number;
  maxMonthlyBills: number;
  features: TenantFeature[];
}

export interface FeatureAccess {
  features: Record<string, boolean>;
  subscriptionPlan: string;
  requiredPlanForUpgrade: string | null;
}

// Tank & Stock Management Types
export interface Tank {
  tankId: string;
  tenantId: string;
  tankName: string;
  tankCode: string;
  fuelTypeId: string;
  fuelTypeName: string;
  fuelTypeCode: string;
  capacity: number;
  currentStock: number;
  minimumLevel: number;
  availablePercentage: number;
  isLowStock: boolean;
  location: string | null;
  installationDate: string | null;
  lastCalibrationDate: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTankDto {
  tankName: string;
  tankCode: string;
  fuelTypeId: string;
  capacity: number;
  currentStock: number;
  minimumLevel: number;
  location?: string;
  installationDate?: string;
  lastCalibrationDate?: string;
}

export interface UpdateTankDto {
  tankName?: string;
  fuelTypeId?: string;
  capacity?: number;
  minimumLevel?: number;
  location?: string;
  lastCalibrationDate?: string;
  isActive?: boolean;
}

export interface TankSummary {
  totalTanks: number;
  activeTanks: number;
  lowStockTanks: number;
  totalCapacity: number;
  totalCurrentStock: number;
  byFuelType: TankStockByFuelType[];
}

export interface TankStockByFuelType {
  fuelTypeId: string;
  fuelTypeName: string;
  tankCount: number;
  totalCapacity: number;
  totalStock: number;
  availablePercentage: number;
}

// Stock Entry Types
export enum StockEntryType {
  StockIn = 0,
  StockOut = 1,
  Adjustment = 2,
  Transfer = 3
}

export interface StockEntry {
  stockEntryId: string;
  tenantId: string;
  tankId: string;
  tankName: string;
  tankCode: string;
  fuelTypeName: string;
  entryType: StockEntryType;
  entryTypeName: string;
  quantity: number;
  stockBefore: number;
  stockAfter: number;
  entryDate: string;
  reference: string | null;
  vendor: string | null;
  unitPrice: number | null;
  totalAmount: number | null;
  notes: string | null;
  shiftId: string | null;
  fuelSaleId: string | null;
  recordedById: string;
  recordedByName: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateStockInDto {
  tankId: string;
  quantity: number;
  entryDate: string;
  reference?: string;
  vendor?: string;
  unitPrice?: number;
  notes?: string;
}

export interface CreateStockAdjustmentDto {
  tankId: string;
  quantity: number;
  entryDate: string;
  notes?: string;
}

export interface StockHistoryFilter {
  tankId?: string;
  entryType?: StockEntryType;
  fromDate?: string;
  toDate?: string;
  page?: number;
  pageSize?: number;
}

export interface StockSummary {
  tankId: string;
  tankName: string;
  tankCode: string;
  fuelTypeName: string;
  currentStock: number;
  totalStockIn: number;
  totalStockOut: number;
  totalAdjustments: number;
  stockInCount: number;
  stockOutCount: number;
  adjustmentCount: number;
  lastStockInDate: string | null;
  lastStockOutDate: string | null;
}

export interface StockMovementReport {
  periodStart: string;
  periodEnd: string;
  openingStock: number;
  totalReceived: number;
  totalSold: number;
  totalAdjustments: number;
  closingStock: number;
  variance: number;
  dailyMovements: DailyStockMovement[];
}

export interface DailyStockMovement {
  date: string;
  openingStock: number;
  stockIn: number;
  stockOut: number;
  adjustments: number;
  closingStock: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
