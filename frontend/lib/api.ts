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
  FuelSale,
  CreateFuelSaleDto,
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
          // Unauthorized - clear token and redirect to login
          this.clearToken();
          if (typeof window !== 'undefined') {
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

  getUser(): any | null {
    if (typeof window === 'undefined') return null;
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  setUser(user: any): void {
    if (typeof window !== 'undefined') {
      localStorage.setItem('user', JSON.stringify(user));
    }
  }

  // Auth API
  async login(credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const { data } = await this.client.post<ApiResponse<LoginResponse>>('/auth/login', credentials);
    if (data.success && data.data) {
      this.setToken(data.data.token);
      this.setUser(data.data.user);
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
}

export const api = new ApiClient();
