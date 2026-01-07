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
  shiftDate: string; // YYYY-MM-DD
  startTime: string; // HH:mm:ss
  openingReadings: NozzleReadingInput[];
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
