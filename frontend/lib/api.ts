import axios, { AxiosInstance, AxiosError } from 'axios';
import type {
  ApiResponse,
  LoginRequest,
  LoginResponse,
  FuelType,
  CreateFuelTypeDto,
  UpdateFuelTypeDto,
  CurrentFuelRate,
  FuelRate,
  CreateFuelRateDto,
  Machine,
  CreateMachineDto,
  UpdateMachineDto,
  Nozzle,
  CreateNozzleDto,
  UpdateNozzleDto,
  Shift,
  CreateShiftDto,
  CloseShiftDto,
  ShiftStatus,
  Worker,
  ShiftMachine,
  ShiftNozzle,
  FuelSale,
  CreateFuelSaleDto,
  User,
  UserProfile,
  CreateUserDto,
  UpdateUserDto,
  DashboardSummary,
  DailySalesSummary,
  ShiftSummaryReport,
  FuelTypePerformance,
  WorkerPerformance,
  NozzlePerformance,
  SalesTrend,
  CreditCustomer,
  CreateCreditCustomerDto,
  UpdateCreditCustomerDto,
  CreditTransaction,
  CreditTransactionType,
  RecordPaymentDto,
  AdjustBalanceDto,
  CreditCustomerSummary,
  CustomerStatement,
  Expense,
  CreateExpenseDto,
  UpdateExpenseDto,
  ExpenseSummary,
  ExpenseCategory,
  PaymentMethod,
  Tenant,
  TenantSummary,
  TenantDetail,
  SystemDashboard,
  CreateTenantDto,
  UpdateTenantDto,
  UpdateSubscriptionDto,
  CreateOwnerUserDto,
  PagedResponse,
  Feature,
  TenantFeature,
  TenantSubscription,
  FeatureAccess,
  UpdateTenantFeatureDto,
  Tank,
  CreateTankDto,
  UpdateTankDto,
  TankSummary,
  StockEntry,
  CreateStockInDto,
  CreateStockAdjustmentDto,
  StockHistoryFilter,
  StockSummary,
  StockMovementReport,
  StockEntryType,
  PagedResult,
} from '@/types';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Add request interceptor to include auth token
    this.client.interceptors.request.use(
      (config) => {
        const token = this.getToken();
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Add response interceptor to handle errors
    this.client.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        if (error.response?.status === 401) {
          // Unauthorized - clear token and redirect to login (only if not already on login page)
          this.clearToken();
          if (typeof window !== 'undefined' && !window.location.pathname.includes('/auth/login')) {
            window.location.href = '/auth/login';
          }
        }
        return Promise.reject(error);
      }
    );
  }

  // Token management
  getToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem('token');
  }

  setToken(token: string): void {
    if (typeof window !== 'undefined') {
      localStorage.setItem('token', token);
    }
  }

  clearToken(): void {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }
  }

  getStoredUser(): any | null {
    if (typeof window === 'undefined') return null;
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  setStoredUser(user: any): void {
    if (typeof window !== 'undefined') {
      localStorage.setItem('user', JSON.stringify(user));
    }
  }

  // Auth API
  async login(credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const { data } = await this.client.post<ApiResponse<LoginResponse>>('/auth/login', credentials);
    if (data.success && data.data) {
      this.setToken(data.data.token);
      this.setStoredUser(data.data.user);
    }
    return data;
  }

  async logout(): Promise<void> {
    try {
      await this.client.post('/auth/logout');
    } finally {
      this.clearToken();
    }
  }

  // Fuel Types API
  async getFuelTypes(): Promise<ApiResponse<FuelType[]>> {
    const { data } = await this.client.get<ApiResponse<FuelType[]>>('/fueltypes');
    return data;
  }

  async getFuelType(id: string): Promise<ApiResponse<FuelType>> {
    const { data } = await this.client.get<ApiResponse<FuelType>>(`/fueltypes/${id}`);
    return data;
  }

  async createFuelType(dto: CreateFuelTypeDto): Promise<ApiResponse<FuelType>> {
    const { data } = await this.client.post<ApiResponse<FuelType>>('/fueltypes', dto);
    return data;
  }

  async updateFuelType(id: string, dto: UpdateFuelTypeDto): Promise<ApiResponse<FuelType>> {
    const { data } = await this.client.put<ApiResponse<FuelType>>(`/fueltypes/${id}`, dto);
    return data;
  }

  async deleteFuelType(id: string): Promise<ApiResponse<null>> {
    const { data} = await this.client.delete<ApiResponse<null>>(`/fueltypes/${id}`);
    return data;
  }

  // Fuel Rates API
  async getCurrentRates(): Promise<ApiResponse<CurrentFuelRate[]>> {
    const { data } = await this.client.get<ApiResponse<CurrentFuelRate[]>>('/fuelrates/current');
    return data;
  }

  async getRateHistory(fuelTypeId: string): Promise<ApiResponse<FuelRate[]>> {
    const { data } = await this.client.get<ApiResponse<FuelRate[]>>(`/fuelrates/history/${fuelTypeId}`);
    return data;
  }

  async createFuelRate(dto: CreateFuelRateDto): Promise<ApiResponse<FuelRate>> {
    const { data } = await this.client.post<ApiResponse<FuelRate>>('/fuelrates', dto);
    return data;
  }

  // Machines API
  async getMachines(activeOnly?: boolean): Promise<ApiResponse<Machine[]>> {
    const params = activeOnly !== undefined ? { activeOnly } : {};
    const { data } = await this.client.get<ApiResponse<Machine[]>>('/machines', { params });
    return data;
  }

  async getMachine(id: string): Promise<ApiResponse<Machine>> {
    const { data } = await this.client.get<ApiResponse<Machine>>(`/machines/${id}`);
    return data;
  }

  async createMachine(dto: CreateMachineDto): Promise<ApiResponse<Machine>> {
    const { data } = await this.client.post<ApiResponse<Machine>>('/machines', dto);
    return data;
  }

  async updateMachine(id: string, dto: UpdateMachineDto): Promise<ApiResponse<Machine>> {
    const { data } = await this.client.put<ApiResponse<Machine>>(`/machines/${id}`, dto);
    return data;
  }

  async deleteMachine(id: string): Promise<ApiResponse<null>> {
    const { data } = await this.client.delete<ApiResponse<null>>(`/machines/${id}`);
    return data;
  }

  // Nozzles API
  async getNozzles(machineId?: string, activeOnly?: boolean): Promise<ApiResponse<Nozzle[]>> {
    const params: any = {};
    if (machineId) params.machineId = machineId;
    if (activeOnly !== undefined) params.activeOnly = activeOnly;
    const { data } = await this.client.get<ApiResponse<Nozzle[]>>('/nozzles', { params });
    return data;
  }

  async getNozzlesByMachine(machineId: string): Promise<ApiResponse<Nozzle[]>> {
    const { data } = await this.client.get<ApiResponse<Nozzle[]>>(`/nozzles/machine/${machineId}`);
    return data;
  }

  async getNozzle(id: string): Promise<ApiResponse<Nozzle>> {
    const { data } = await this.client.get<ApiResponse<Nozzle>>(`/nozzles/${id}`);
    return data;
  }

  async createNozzle(dto: CreateNozzleDto): Promise<ApiResponse<Nozzle>> {
    const { data } = await this.client.post<ApiResponse<Nozzle>>('/nozzles', dto);
    return data;
  }

  async updateNozzle(id: string, dto: UpdateNozzleDto): Promise<ApiResponse<Nozzle>> {
    const { data } = await this.client.put<ApiResponse<Nozzle>>(`/nozzles/${id}`, dto);
    return data;
  }

  async deleteNozzle(id: string): Promise<ApiResponse<null>> {
    const { data } = await this.client.delete<ApiResponse<null>>(`/nozzles/${id}`);
    return data;
  }

  // Shifts API
  async getWorkers(): Promise<ApiResponse<Worker[]>> {
    const { data } = await this.client.get<ApiResponse<Worker[]>>('/shifts/workers');
    return data;
  }

  async getShiftMachines(): Promise<ApiResponse<ShiftMachine[]>> {
    const { data } = await this.client.get<ApiResponse<ShiftMachine[]>>('/shifts/machines');
    return data;
  }

  async getShiftMachineNozzles(machineId: string): Promise<ApiResponse<ShiftNozzle[]>> {
    const { data } = await this.client.get<ApiResponse<ShiftNozzle[]>>(`/shifts/machines/${machineId}/nozzles`);
    return data;
  }

  async getShifts(params?: {
    fromDate?: string;
    toDate?: string;
    status?: ShiftStatus;
    workerId?: string;
  }): Promise<ApiResponse<Shift[]>> {
    const { data } = await this.client.get<ApiResponse<Shift[]>>('/shifts', { params });
    return data;
  }

  async getShift(id: string): Promise<ApiResponse<Shift>> {
    const { data } = await this.client.get<ApiResponse<Shift>>(`/shifts/${id}`);
    return data;
  }

  async getMyActiveShift(): Promise<ApiResponse<Shift | null>> {
    const { data } = await this.client.get<ApiResponse<Shift | null>>('/shifts/my-active');
    return data;
  }

  async startShift(dto: CreateShiftDto): Promise<ApiResponse<Shift>> {
    const { data } = await this.client.post<ApiResponse<Shift>>('/shifts', dto);
    return data;
  }

  async closeShift(id: string, dto: CloseShiftDto): Promise<ApiResponse<Shift>> {
    const { data} = await this.client.put<ApiResponse<Shift>>(`/shifts/${id}/close`, dto);
    return data;
  }

  // Fuel Sales API
  async createFuelSale(dto: CreateFuelSaleDto): Promise<ApiResponse<FuelSale>> {
    const { data } = await this.client.post<ApiResponse<FuelSale>>('/fuelsales', dto);
    return data;
  }

  async getFuelSale(id: string): Promise<ApiResponse<FuelSale>> {
    const { data } = await this.client.get<ApiResponse<FuelSale>>(`/fuelsales/${id}`);
    return data;
  }

  async getFuelSalesByShift(shiftId: string): Promise<ApiResponse<FuelSale[]>> {
    const { data } = await this.client.get<ApiResponse<FuelSale[]>>(`/fuelsales/shift/${shiftId}`);
    return data;
  }

  async updateFuelSale(id: string, dto: CreateFuelSaleDto): Promise<ApiResponse<FuelSale>> {
    const { data } = await this.client.put<ApiResponse<FuelSale>>(`/fuelsales/${id}`, dto);
    return data;
  }

  async deleteFuelSale(id: string): Promise<ApiResponse<null>> {
    const { data } = await this.client.delete<ApiResponse<null>>(`/fuelsales/${id}`);
    return data;
  }

  async voidFuelSale(id: string, reason: string): Promise<ApiResponse<FuelSale>> {
    const { data } = await this.client.put<ApiResponse<FuelSale>>(`/fuelsales/${id}/void`, { reason });
    return data;
  }

  // User Management API
  async getUsers(role?: string, isActive?: boolean): Promise<ApiResponse<User[]>> {
    const params = new URLSearchParams();
    if (role) params.append('role', role);
    if (isActive !== undefined) params.append('isActive', String(isActive));
    const { data } = await this.client.get<ApiResponse<User[]>>(`/users?${params.toString()}`);
    return data;
  }

  async getUser(id: string): Promise<ApiResponse<User>> {
    const { data } = await this.client.get<ApiResponse<User>>(`/users/${id}`);
    return data;
  }

  async getUserProfile(id: string): Promise<ApiResponse<UserProfile>> {
    const { data } = await this.client.get<ApiResponse<UserProfile>>(`/users/${id}/profile`);
    return data;
  }

  async createUser(dto: CreateUserDto): Promise<ApiResponse<User>> {
    const { data } = await this.client.post<ApiResponse<User>>('/users', dto);
    return data;
  }

  async updateUser(id: string, dto: UpdateUserDto): Promise<ApiResponse<User>> {
    const { data } = await this.client.put<ApiResponse<User>>(`/users/${id}`, dto);
    return data;
  }

  async resetUserPassword(id: string, newPassword: string): Promise<ApiResponse<null>> {
    const { data } = await this.client.put<ApiResponse<null>>(`/users/${id}/reset-password`, { newPassword });
    return data;
  }

  async updateUserStatus(id: string, isActive: boolean): Promise<ApiResponse<null>> {
    const { data } = await this.client.put<ApiResponse<null>>(`/users/${id}/status`, { isActive });
    return data;
  }

  async deleteUser(id: string): Promise<ApiResponse<null>> {
    const { data } = await this.client.delete<ApiResponse<null>>(`/users/${id}`);
    return data;
  }

  // Reports API
  async getDashboardSummary(): Promise<ApiResponse<DashboardSummary>> {
    const { data } = await this.client.get<ApiResponse<DashboardSummary>>('/reports/dashboard');
    return data;
  }

  async getDailySalesSummary(fromDate?: string, toDate?: string): Promise<ApiResponse<DailySalesSummary[]>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const { data } = await this.client.get<ApiResponse<DailySalesSummary[]>>(`/reports/daily-sales?${params.toString()}`);
    return data;
  }

  async getShiftSummaryReport(fromDate?: string, toDate?: string, workerId?: string): Promise<ApiResponse<ShiftSummaryReport[]>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    if (workerId) params.append('workerId', workerId);
    const { data } = await this.client.get<ApiResponse<ShiftSummaryReport[]>>(`/reports/shifts?${params.toString()}`);
    return data;
  }

  async getFuelTypePerformance(fromDate?: string, toDate?: string): Promise<ApiResponse<FuelTypePerformance[]>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const { data } = await this.client.get<ApiResponse<FuelTypePerformance[]>>(`/reports/fuel-performance?${params.toString()}`);
    return data;
  }

  async getWorkerPerformance(fromDate?: string, toDate?: string): Promise<ApiResponse<WorkerPerformance[]>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const { data } = await this.client.get<ApiResponse<WorkerPerformance[]>>(`/reports/worker-performance?${params.toString()}`);
    return data;
  }

  async getNozzlePerformance(fromDate?: string, toDate?: string, machineId?: string): Promise<ApiResponse<NozzlePerformance[]>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    if (machineId) params.append('machineId', machineId);
    const { data } = await this.client.get<ApiResponse<NozzlePerformance[]>>(`/reports/nozzle-performance?${params.toString()}`);
    return data;
  }

  async getSalesTrend(fromDate?: string, toDate?: string): Promise<ApiResponse<SalesTrend[]>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const { data } = await this.client.get<ApiResponse<SalesTrend[]>>(`/reports/sales-trend?${params.toString()}`);
    return data;
  }

  // Credit Customer Management
  async getCreditCustomerSummary(): Promise<ApiResponse<CreditCustomerSummary>> {
    const { data } = await this.client.get<ApiResponse<CreditCustomerSummary>>('/creditcustomers/summary');
    return data;
  }

  async getCreditCustomers(isActive?: boolean, isBlocked?: boolean, search?: string): Promise<ApiResponse<CreditCustomer[]>> {
    const params = new URLSearchParams();
    if (isActive !== undefined) params.append('isActive', isActive.toString());
    if (isBlocked !== undefined) params.append('isBlocked', isBlocked.toString());
    if (search) params.append('search', search);
    const { data } = await this.client.get<ApiResponse<CreditCustomer[]>>(`/creditcustomers?${params.toString()}`);
    return data;
  }

  async getCreditCustomer(id: string): Promise<ApiResponse<CreditCustomer>> {
    const { data } = await this.client.get<ApiResponse<CreditCustomer>>(`/creditcustomers/${id}`);
    return data;
  }

  async createCreditCustomer(dto: CreateCreditCustomerDto): Promise<ApiResponse<CreditCustomer>> {
    const { data } = await this.client.post<ApiResponse<CreditCustomer>>('/creditcustomers', dto);
    return data;
  }

  async updateCreditCustomer(id: string, dto: UpdateCreditCustomerDto): Promise<ApiResponse<CreditCustomer>> {
    const { data } = await this.client.put<ApiResponse<CreditCustomer>>(`/creditcustomers/${id}`, dto);
    return data;
  }

  async deleteCreditCustomer(id: string): Promise<ApiResponse<null>> {
    const { data } = await this.client.delete<ApiResponse<null>>(`/creditcustomers/${id}`);
    return data;
  }

  async recordPayment(customerId: string, dto: RecordPaymentDto): Promise<ApiResponse<CreditTransaction>> {
    const { data } = await this.client.post<ApiResponse<CreditTransaction>>(`/creditcustomers/${customerId}/payments`, dto);
    return data;
  }

  async adjustBalance(customerId: string, dto: AdjustBalanceDto): Promise<ApiResponse<CreditTransaction>> {
    const { data } = await this.client.post<ApiResponse<CreditTransaction>>(`/creditcustomers/${customerId}/adjustments`, dto);
    return data;
  }

  async blockCreditCustomer(id: string, reason: string): Promise<ApiResponse<CreditCustomer>> {
    const { data } = await this.client.post<ApiResponse<CreditCustomer>>(`/creditcustomers/${id}/block`, { reason });
    return data;
  }

  async unblockCreditCustomer(id: string): Promise<ApiResponse<CreditCustomer>> {
    const { data } = await this.client.post<ApiResponse<CreditCustomer>>(`/creditcustomers/${id}/unblock`);
    return data;
  }

  async getCreditTransactions(
    customerId: string,
    fromDate?: string,
    toDate?: string,
    type?: CreditTransactionType
  ): Promise<ApiResponse<CreditTransaction[]>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    if (type !== undefined) params.append('type', type.toString());
    const { data } = await this.client.get<ApiResponse<CreditTransaction[]>>(`/creditcustomers/${customerId}/transactions?${params.toString()}`);
    return data;
  }

  async getCustomerStatement(customerId: string, fromDate?: string, toDate?: string): Promise<ApiResponse<CustomerStatement>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const { data } = await this.client.get<ApiResponse<CustomerStatement>>(`/creditcustomers/${customerId}/statement?${params.toString()}`);
    return data;
  }

  async getCreditCustomerByVehicle(vehicleNumber: string): Promise<ApiResponse<CreditCustomer>> {
    const { data } = await this.client.get<ApiResponse<CreditCustomer>>(`/creditcustomers/by-vehicle/${encodeURIComponent(vehicleNumber)}`);
    return data;
  }

  // Export API methods
  async exportDailySales(fromDate?: string, toDate?: string, format: string = 'pdf'): Promise<void> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    params.append('format', format);
    await this.downloadFile(`/reports/daily-sales/export?${params.toString()}`, `daily-sales.${this.getExtension(format)}`);
  }

  async exportShifts(fromDate?: string, toDate?: string, workerId?: string, format: string = 'pdf'): Promise<void> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    if (workerId) params.append('workerId', workerId);
    params.append('format', format);
    await this.downloadFile(`/reports/shifts/export?${params.toString()}`, `shifts.${this.getExtension(format)}`);
  }

  async exportFuelPerformance(fromDate?: string, toDate?: string, format: string = 'pdf'): Promise<void> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    params.append('format', format);
    await this.downloadFile(`/reports/fuel-performance/export?${params.toString()}`, `fuel-performance.${this.getExtension(format)}`);
  }

  async exportWorkerPerformance(fromDate?: string, toDate?: string, format: string = 'pdf'): Promise<void> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    params.append('format', format);
    await this.downloadFile(`/reports/worker-performance/export?${params.toString()}`, `worker-performance.${this.getExtension(format)}`);
  }

  async exportNozzlePerformance(fromDate?: string, toDate?: string, format: string = 'pdf'): Promise<void> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    params.append('format', format);
    await this.downloadFile(`/reports/nozzle-performance/export?${params.toString()}`, `nozzle-performance.${this.getExtension(format)}`);
  }

  async exportCustomerStatement(customerId: string, fromDate?: string, toDate?: string, format: string = 'pdf'): Promise<void> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    params.append('format', format);
    await this.downloadFile(`/creditcustomers/${customerId}/statement/export?${params.toString()}`, `customer-statement.${this.getExtension(format)}`);
  }

  async exportCreditCustomers(isActive?: boolean, isBlocked?: boolean, format: string = 'pdf'): Promise<void> {
    const params = new URLSearchParams();
    if (isActive !== undefined) params.append('isActive', isActive.toString());
    if (isBlocked !== undefined) params.append('isBlocked', isBlocked.toString());
    params.append('format', format);
    await this.downloadFile(`/creditcustomers/export?${params.toString()}`, `credit-customers.${this.getExtension(format)}`);
  }

  // Expense Management API
  async getExpenseSummary(fromDate?: string, toDate?: string): Promise<ApiResponse<ExpenseSummary>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const { data } = await this.client.get<ApiResponse<ExpenseSummary>>(`/expenses/summary?${params.toString()}`);
    return data;
  }

  async getExpenses(
    fromDate?: string,
    toDate?: string,
    category?: ExpenseCategory,
    paymentMode?: PaymentMethod,
    search?: string
  ): Promise<ApiResponse<Expense[]>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    if (category !== undefined) params.append('category', category.toString());
    if (paymentMode !== undefined) params.append('paymentMode', paymentMode.toString());
    if (search) params.append('search', search);
    const { data } = await this.client.get<ApiResponse<Expense[]>>(`/expenses?${params.toString()}`);
    return data;
  }

  async getExpense(id: string): Promise<ApiResponse<Expense>> {
    const { data } = await this.client.get<ApiResponse<Expense>>(`/expenses/${id}`);
    return data;
  }

  async createExpense(dto: CreateExpenseDto): Promise<ApiResponse<Expense>> {
    const { data } = await this.client.post<ApiResponse<Expense>>('/expenses', dto);
    return data;
  }

  async updateExpense(id: string, dto: UpdateExpenseDto): Promise<ApiResponse<Expense>> {
    const { data } = await this.client.put<ApiResponse<Expense>>(`/expenses/${id}`, dto);
    return data;
  }

  async deleteExpense(id: string): Promise<ApiResponse<null>> {
    const { data } = await this.client.delete<ApiResponse<null>>(`/expenses/${id}`);
    return data;
  }

  async getExpenseCategories(): Promise<ApiResponse<{ value: number; name: string }[]>> {
    const { data } = await this.client.get<ApiResponse<{ value: number; name: string }[]>>('/expenses/categories');
    return data;
  }

  async exportExpenses(fromDate?: string, toDate?: string, category?: ExpenseCategory, format: string = 'pdf'): Promise<void> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    if (category !== undefined) params.append('category', category.toString());
    params.append('format', format);
    await this.downloadFile(`/expenses/export?${params.toString()}`, `expenses.${this.getExtension(format)}`);
  }

  async exportExpenseSummary(fromDate?: string, toDate?: string, format: string = 'pdf'): Promise<void> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    params.append('format', format);
    await this.downloadFile(`/expenses/summary/export?${params.toString()}`, `expense-summary.${this.getExtension(format)}`);
  }

  private getExtension(format: string): string {
    switch (format.toLowerCase()) {
      case 'excel':
      case 'xlsx':
        return 'xlsx';
      case 'csv':
        return 'csv';
      default:
        return 'pdf';
    }
  }

  private async downloadFile(url: string, filename: string): Promise<void> {
    const response = await this.client.get(url, { responseType: 'blob' });
    const blob = new Blob([response.data]);
    const downloadUrl = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = downloadUrl;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(downloadUrl);
  }

  // Super Admin API
  async getAdminDashboard(): Promise<ApiResponse<SystemDashboard>> {
    const { data } = await this.client.get<ApiResponse<SystemDashboard>>('/superadmin/dashboard');
    return data;
  }

  async getAdminStats(): Promise<ApiResponse<object>> {
    const { data } = await this.client.get<ApiResponse<object>>('/superadmin/stats');
    return data;
  }

  async getExpiringTenants(days: number = 30): Promise<ApiResponse<object[]>> {
    const { data } = await this.client.get<ApiResponse<object[]>>(`/superadmin/expiring-tenants?days=${days}`);
    return data;
  }

  async getRecentActivity(limit: number = 20): Promise<ApiResponse<object>> {
    const { data } = await this.client.get<ApiResponse<object>>(`/superadmin/recent-activity?limit=${limit}`);
    return data;
  }

  // Tenant Management API
  async getTenants(page: number = 1, limit: number = 20, status?: string, search?: string): Promise<ApiResponse<PagedResponse<Tenant>>> {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('limit', limit.toString());
    if (status) params.append('status', status);
    if (search) params.append('search', search);
    const { data } = await this.client.get<ApiResponse<PagedResponse<Tenant>>>(`/tenants?${params.toString()}`);
    return data;
  }

  async getTenantsWithStats(page: number = 1, limit: number = 20, status?: string, plan?: string, search?: string): Promise<ApiResponse<PagedResponse<TenantSummary>>> {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('limit', limit.toString());
    if (status) params.append('status', status);
    if (plan) params.append('plan', plan);
    if (search) params.append('search', search);
    const { data } = await this.client.get<ApiResponse<PagedResponse<TenantSummary>>>(`/tenants/with-stats?${params.toString()}`);
    return data;
  }

  async getTenant(id: string): Promise<ApiResponse<Tenant>> {
    const { data } = await this.client.get<ApiResponse<Tenant>>(`/tenants/${id}`);
    return data;
  }

  async getTenantDetails(id: string): Promise<ApiResponse<TenantDetail>> {
    const { data } = await this.client.get<ApiResponse<TenantDetail>>(`/tenants/${id}/details`);
    return data;
  }

  async createTenant(dto: CreateTenantDto): Promise<ApiResponse<Tenant>> {
    const { data } = await this.client.post<ApiResponse<Tenant>>('/tenants', dto);
    return data;
  }

  async updateTenant(id: string, dto: UpdateTenantDto): Promise<ApiResponse<Tenant>> {
    const { data } = await this.client.put<ApiResponse<Tenant>>(`/tenants/${id}`, dto);
    return data;
  }

  async updateTenantStatus(id: string, isActive: boolean, reason?: string): Promise<ApiResponse<Tenant>> {
    const { data } = await this.client.patch<ApiResponse<Tenant>>(`/tenants/${id}/status`, { isActive, reason });
    return data;
  }

  async updateTenantSubscription(id: string, dto: UpdateSubscriptionDto): Promise<ApiResponse<Tenant>> {
    const { data } = await this.client.patch<ApiResponse<Tenant>>(`/tenants/${id}/subscription`, dto);
    return data;
  }

  async createTenantOwner(tenantId: string, dto: CreateOwnerUserDto): Promise<ApiResponse<object>> {
    const { data } = await this.client.post<ApiResponse<object>>(`/tenants/${tenantId}/owner`, dto);
    return data;
  }

  // Feature Management API
  async getAllFeatures(): Promise<ApiResponse<Feature[]>> {
    const { data } = await this.client.get<ApiResponse<Feature[]>>('/features');
    return data;
  }

  async getPlanFeatures(plan: string): Promise<ApiResponse<Feature[]>> {
    const { data } = await this.client.get<ApiResponse<Feature[]>>(`/features/plans/${plan}`);
    return data;
  }

  async getMyFeatureAccess(): Promise<ApiResponse<FeatureAccess>> {
    const { data } = await this.client.get<ApiResponse<FeatureAccess>>('/features/my-access');
    return data;
  }

  async getMySubscription(): Promise<ApiResponse<TenantSubscription>> {
    const { data } = await this.client.get<ApiResponse<TenantSubscription>>('/features/subscription');
    return data;
  }

  async getTenantFeatures(tenantId: string): Promise<ApiResponse<TenantFeature[]>> {
    const { data } = await this.client.get<ApiResponse<TenantFeature[]>>(`/features/tenant/${tenantId}`);
    return data;
  }

  async updateTenantFeatures(tenantId: string, updates: UpdateTenantFeatureDto[]): Promise<ApiResponse<TenantFeature[]>> {
    const { data } = await this.client.put<ApiResponse<TenantFeature[]>>(`/features/tenant/${tenantId}`, updates);
    return data;
  }

  async resetTenantFeatures(tenantId: string): Promise<ApiResponse<TenantFeature[]>> {
    const { data } = await this.client.post<ApiResponse<TenantFeature[]>>(`/features/tenant/${tenantId}/reset`);
    return data;
  }

  // Tank Management API
  async getTanks(): Promise<ApiResponse<Tank[]>> {
    const { data } = await this.client.get<ApiResponse<Tank[]>>('/tanks');
    return data;
  }

  async getTank(id: string): Promise<ApiResponse<Tank>> {
    const { data } = await this.client.get<ApiResponse<Tank>>(`/tanks/${id}`);
    return data;
  }

  async getTankSummary(): Promise<ApiResponse<TankSummary>> {
    const { data } = await this.client.get<ApiResponse<TankSummary>>('/tanks/summary');
    return data;
  }

  async createTank(dto: CreateTankDto): Promise<ApiResponse<Tank>> {
    const { data } = await this.client.post<ApiResponse<Tank>>('/tanks', dto);
    return data;
  }

  async updateTank(id: string, dto: UpdateTankDto): Promise<ApiResponse<Tank>> {
    const { data } = await this.client.put<ApiResponse<Tank>>(`/tanks/${id}`, dto);
    return data;
  }

  async deleteTank(id: string): Promise<ApiResponse<null>> {
    const { data } = await this.client.delete<ApiResponse<null>>(`/tanks/${id}`);
    return data;
  }

  async getTankStockSummary(tankId: string, fromDate?: string, toDate?: string): Promise<ApiResponse<StockSummary>> {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const url = params.toString() ? `/tanks/${tankId}/stock-summary?${params.toString()}` : `/tanks/${tankId}/stock-summary`;
    const { data } = await this.client.get<ApiResponse<StockSummary>>(url);
    return data;
  }

  async getTankMovementReport(tankId: string, fromDate: string, toDate: string): Promise<ApiResponse<StockMovementReport>> {
    const params = new URLSearchParams();
    params.append('fromDate', fromDate);
    params.append('toDate', toDate);
    const { data } = await this.client.get<ApiResponse<StockMovementReport>>(`/tanks/${tankId}/movement-report?${params.toString()}`);
    return data;
  }

  // Stock Entry API
  async getStockHistory(filter: StockHistoryFilter): Promise<ApiResponse<PagedResult<StockEntry>>> {
    const params = new URLSearchParams();
    if (filter.tankId) params.append('tankId', filter.tankId);
    if (filter.entryType !== undefined) params.append('entryType', filter.entryType.toString());
    if (filter.fromDate) params.append('fromDate', filter.fromDate);
    if (filter.toDate) params.append('toDate', filter.toDate);
    if (filter.page) params.append('page', filter.page.toString());
    if (filter.pageSize) params.append('pageSize', filter.pageSize.toString());
    const { data } = await this.client.get<ApiResponse<PagedResult<StockEntry>>>(`/stockentries?${params.toString()}`);
    return data;
  }

  async recordStockIn(dto: CreateStockInDto): Promise<ApiResponse<StockEntry>> {
    const { data } = await this.client.post<ApiResponse<StockEntry>>('/stockentries/stock-in', dto);
    return data;
  }

  async recordStockAdjustment(dto: CreateStockAdjustmentDto): Promise<ApiResponse<StockEntry>> {
    const { data } = await this.client.post<ApiResponse<StockEntry>>('/stockentries/adjustment', dto);
    return data;
  }

  async getStockEntriesByTank(tankId: string, page: number = 1, pageSize: number = 20): Promise<ApiResponse<PagedResult<StockEntry>>> {
    const { data } = await this.client.get<ApiResponse<PagedResult<StockEntry>>>(`/stockentries/by-tank/${tankId}?page=${page}&pageSize=${pageSize}`);
    return data;
  }

  async getStockEntriesByType(entryType: StockEntryType, page: number = 1, pageSize: number = 20): Promise<ApiResponse<PagedResult<StockEntry>>> {
    const { data } = await this.client.get<ApiResponse<PagedResult<StockEntry>>>(`/stockentries/by-type/${entryType}?page=${page}&pageSize=${pageSize}`);
    return data;
  }

  // ============ Registration API ============

  async register(dto: {
    companyName: string;
    ownerName: string;
    email: string;
    phone: string;
    tenantCode?: string;
    address?: string;
    city?: string;
    state?: string;
    pinCode?: string;
    username: string;
    password: string;
  }): Promise<ApiResponse<{
    tenantId: string;
    tenantCode: string;
    companyName: string;
    username: string;
    trialStartDate: string;
    trialEndDate: string;
    trialDaysRemaining: number;
    message: string;
  }>> {
    const { data } = await this.client.post('/registration/register', dto);
    return data;
  }

  async checkEmailAvailability(email: string): Promise<ApiResponse<{
    isAvailable: boolean;
    message?: string;
  }>> {
    const { data } = await this.client.get(`/registration/check-email/${encodeURIComponent(email)}`);
    return data;
  }

  async checkTenantCodeAvailability(code: string): Promise<ApiResponse<{
    isAvailable: boolean;
    message?: string;
    suggestedValue?: string;
  }>> {
    const { data } = await this.client.get(`/registration/check-tenant-code/${encodeURIComponent(code)}`);
    return data;
  }

  async suggestTenantCode(companyName: string): Promise<ApiResponse<{ tenantCode: string }>> {
    const { data } = await this.client.get(`/registration/suggest-tenant-code?companyName=${encodeURIComponent(companyName)}`);
    return data;
  }

  // ============ Subscription API ============

  async getSubscriptionStatus(): Promise<ApiResponse<{
    tenantId: string;
    tenantCode: string;
    companyName: string;
    isTrial: boolean;
    trialStartDate: string | null;
    trialEndDate: string | null;
    trialDaysRemaining: number | null;
    subscriptionPlan: string;
    subscriptionStatus: string;
    subscriptionStartDate: string | null;
    subscriptionEndDate: string | null;
    daysRemaining: number | null;
    maxMachines: number;
    maxWorkers: number;
    maxMonthlyBills: number;
    currentMachineCount: number;
    currentWorkerCount: number;
    currentMonthBillCount: number;
    activeLicenseKeyId: string | null;
    licenseActivatedAt: string | null;
  }>> {
    const { data } = await this.client.get('/subscription/status');
    return data;
  }

  async activateLicenseKey(licenseKey: string): Promise<ApiResponse<{
    success: boolean;
    message: string;
    newSubscriptionPlan?: string;
    newSubscriptionEndDate?: string;
    maxMachines?: number;
    maxWorkers?: number;
    maxMonthlyBills?: number;
  }>> {
    const { data } = await this.client.post('/subscription/activate', { licenseKey });
    return data;
  }

  // ============ License Key Management API (Super Admin) ============

  async generateLicenseKey(dto: {
    subscriptionPlan: string;
    durationMonths: number;
    maxMachines?: number;
    maxWorkers?: number;
    maxMonthlyBills?: number;
    notes?: string;
  }): Promise<ApiResponse<{
    licenseKeyId: string;
    key: string;
    subscriptionPlan: string;
    durationMonths: number;
    maxMachines: number;
    maxWorkers: number;
    maxMonthlyBills: number;
    status: string;
    notes?: string;
    createdAt: string;
  }>> {
    const { data } = await this.client.post('/superadmin/license-keys', dto);
    return data;
  }

  async generateBatchLicenseKeys(dto: {
    count: number;
    subscriptionPlan: string;
    durationMonths: number;
    maxMachines?: number;
    maxWorkers?: number;
    maxMonthlyBills?: number;
    notes?: string;
  }): Promise<ApiResponse<Array<{
    licenseKeyId: string;
    key: string;
    subscriptionPlan: string;
    durationMonths: number;
    maxMachines: number;
    maxWorkers: number;
    maxMonthlyBills: number;
    status: string;
    notes?: string;
    createdAt: string;
  }>>> {
    const { data } = await this.client.post('/superadmin/license-keys/batch', dto);
    return data;
  }

  async getLicenseKeys(params?: {
    status?: string;
    subscriptionPlan?: string;
    search?: string;
    page?: number;
    limit?: number;
  }): Promise<ApiResponse<{
    keys: Array<{
      licenseKeyId: string;
      key: string;
      subscriptionPlan: string;
      durationMonths: number;
      maxMachines: number;
      maxWorkers: number;
      maxMonthlyBills: number;
      status: string;
      activatedByTenantId?: string;
      activatedByTenantCode?: string;
      activatedByCompanyName?: string;
      activatedAt?: string;
      generatedByUsername: string;
      notes?: string;
      createdAt: string;
    }>;
    totalCount: number;
    page: number;
    limit: number;
    totalPages: number;
  }>> {
    const searchParams = new URLSearchParams();
    if (params?.status) searchParams.append('status', params.status);
    if (params?.subscriptionPlan) searchParams.append('subscriptionPlan', params.subscriptionPlan);
    if (params?.search) searchParams.append('search', params.search);
    if (params?.page) searchParams.append('page', params.page.toString());
    if (params?.limit) searchParams.append('limit', params.limit.toString());
    const { data } = await this.client.get(`/superadmin/license-keys?${searchParams.toString()}`);
    return data;
  }

  async getLicenseKey(id: string): Promise<ApiResponse<{
    licenseKeyId: string;
    key: string;
    subscriptionPlan: string;
    durationMonths: number;
    maxMachines: number;
    maxWorkers: number;
    maxMonthlyBills: number;
    status: string;
    activatedByTenantId?: string;
    activatedByTenantCode?: string;
    activatedByCompanyName?: string;
    activatedAt?: string;
    generatedByUsername: string;
    notes?: string;
    createdAt: string;
  }>> {
    const { data } = await this.client.get(`/superadmin/license-keys/${id}`);
    return data;
  }

  async revokeLicenseKey(id: string, reason?: string): Promise<ApiResponse<null>> {
    const { data } = await this.client.patch(`/superadmin/license-keys/${id}/revoke`, { reason });
    return data;
  }

  async getLicenseKeyStats(): Promise<ApiResponse<{
    totalKeys: number;
    availableKeys: number;
    usedKeys: number;
    revokedKeys: number;
    keysByPlan: Array<{ plan: string; count: number }>;
  }>> {
    const { data } = await this.client.get('/superadmin/license-keys/stats');
    return data;
  }
}

export const api = new ApiClient();
