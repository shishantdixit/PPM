'use client';

import { useState, useEffect } from 'react';
import { api } from '@/lib/api';
import type {
  DailySalesSummary,
  ShiftSummaryReport,
  FuelTypePerformance,
  WorkerPerformance,
  NozzlePerformance,
  SalesTrend,
  Worker,
  ShiftStatus,
} from '@/types';
import { useAuth } from '@/context/AuthContext';

type ReportType = 'daily' | 'shifts' | 'fuel' | 'workers' | 'nozzles';

export default function ReportsPage() {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState<ReportType>('daily');
  const [fromDate, setFromDate] = useState(() => {
    const date = new Date();
    date.setDate(date.getDate() - 30);
    return date.toISOString().split('T')[0];
  });
  const [toDate, setToDate] = useState(() => new Date().toISOString().split('T')[0]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [exporting, setExporting] = useState(false);
  const [showExportMenu, setShowExportMenu] = useState(false);

  // Report data
  const [dailySales, setDailySales] = useState<DailySalesSummary[]>([]);
  const [shifts, setShifts] = useState<ShiftSummaryReport[]>([]);
  const [fuelPerformance, setFuelPerformance] = useState<FuelTypePerformance[]>([]);
  const [workerPerformance, setWorkerPerformance] = useState<WorkerPerformance[]>([]);
  const [nozzlePerformance, setNozzlePerformance] = useState<NozzlePerformance[]>([]);
  const [workers, setWorkers] = useState<Worker[]>([]);
  const [selectedWorker, setSelectedWorker] = useState<string>('');

  useEffect(() => {
    loadWorkers();
  }, []);

  useEffect(() => {
    loadReport();
  }, [activeTab, fromDate, toDate, selectedWorker]);

  const loadWorkers = async () => {
    try {
      const response = await api.getWorkers();
      if (response.success && response.data) {
        setWorkers(response.data);
      }
    } catch (err) {
      console.error('Failed to load workers', err);
    }
  };

  const loadReport = async () => {
    setLoading(true);
    setError(null);
    try {
      switch (activeTab) {
        case 'daily':
          const dailyRes = await api.getDailySalesSummary(fromDate, toDate);
          if (dailyRes.success && dailyRes.data) setDailySales(dailyRes.data);
          break;
        case 'shifts':
          const shiftsRes = await api.getShiftSummaryReport(fromDate, toDate, selectedWorker || undefined);
          if (shiftsRes.success && shiftsRes.data) setShifts(shiftsRes.data);
          break;
        case 'fuel':
          const fuelRes = await api.getFuelTypePerformance(fromDate, toDate);
          if (fuelRes.success && fuelRes.data) setFuelPerformance(fuelRes.data);
          break;
        case 'workers':
          const workerRes = await api.getWorkerPerformance(fromDate, toDate);
          if (workerRes.success && workerRes.data) setWorkerPerformance(workerRes.data);
          break;
        case 'nozzles':
          const nozzleRes = await api.getNozzlePerformance(fromDate, toDate);
          if (nozzleRes.success && nozzleRes.data) setNozzlePerformance(nozzleRes.data);
          break;
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load report');
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 2,
    }).format(value);
  };

  const formatNumber = (value: number) => {
    return new Intl.NumberFormat('en-IN', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value);
  };

  const getStatusLabel = (status: ShiftStatus) => {
    const labels = ['Pending', 'Active', 'Closed'];
    return labels[status];
  };

  const getStatusBadge = (status: ShiftStatus) => {
    const styles: Record<number, string> = {
      0: 'bg-gray-100 text-gray-800',
      1: 'bg-green-100 text-green-800',
      2: 'bg-blue-100 text-blue-800',
    };
    return (
      <span className={`px-2 py-1 rounded text-xs font-medium ${styles[status]}`}>
        {getStatusLabel(status)}
      </span>
    );
  };

  const handleExport = async (format: string) => {
    setExporting(true);
    setShowExportMenu(false);
    try {
      switch (activeTab) {
        case 'daily':
          await api.exportDailySales(fromDate, toDate, format);
          break;
        case 'shifts':
          await api.exportShifts(fromDate, toDate, selectedWorker || undefined, format);
          break;
        case 'fuel':
          await api.exportFuelPerformance(fromDate, toDate, format);
          break;
        case 'workers':
          await api.exportWorkerPerformance(fromDate, toDate, format);
          break;
        case 'nozzles':
          await api.exportNozzlePerformance(fromDate, toDate, format);
          break;
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to export report');
    } finally {
      setExporting(false);
    }
  };

  const tabs: { id: ReportType; label: string; icon: string; ownerOnly?: boolean }[] = [
    { id: 'daily', label: 'Daily Sales', icon: '📊' },
    { id: 'shifts', label: 'Shifts', icon: '📋' },
    { id: 'fuel', label: 'Fuel Performance', icon: '⛽' },
    { id: 'workers', label: 'Worker Performance', icon: '👷', ownerOnly: true },
    { id: 'nozzles', label: 'Nozzle Performance', icon: '🔧' },
  ];

  const filteredTabs = tabs.filter(tab => !tab.ownerOnly || user?.role === 'Owner');

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Reports & Analytics</h1>

      {/* Date Filters */}
      <div className="bg-white rounded-lg shadow p-4">
        <div className="flex flex-wrap items-center gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">From Date</label>
            <input
              type="date"
              value={fromDate}
              onChange={(e) => setFromDate(e.target.value)}
              className="border rounded-lg px-3 py-2"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">To Date</label>
            <input
              type="date"
              value={toDate}
              onChange={(e) => setToDate(e.target.value)}
              className="border rounded-lg px-3 py-2"
            />
          </div>
          {activeTab === 'shifts' && (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Worker</label>
              <select
                value={selectedWorker}
                onChange={(e) => setSelectedWorker(e.target.value)}
                className="border rounded-lg px-3 py-2"
              >
                <option value="">All Workers</option>
                {workers.map((worker) => (
                  <option key={worker.userId} value={worker.userId}>
                    {worker.fullName}
                  </option>
                ))}
              </select>
            </div>
          )}
          <div className="flex-1" />
          <div className="flex items-center gap-2">
            <button
              onClick={loadReport}
              disabled={loading}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
            >
              {loading ? 'Loading...' : 'Refresh'}
            </button>
            <div className="relative">
              <button
                onClick={() => setShowExportMenu(!showExportMenu)}
                disabled={exporting}
                className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 flex items-center gap-2"
              >
                {exporting ? 'Exporting...' : 'Export'}
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                </svg>
              </button>
              {showExportMenu && (
                <div className="absolute right-0 mt-2 w-40 bg-white rounded-lg shadow-lg border z-10">
                  <button
                    onClick={() => handleExport('pdf')}
                    className="w-full px-4 py-2 text-left hover:bg-gray-100 rounded-t-lg"
                  >
                    PDF
                  </button>
                  <button
                    onClick={() => handleExport('excel')}
                    className="w-full px-4 py-2 text-left hover:bg-gray-100"
                  >
                    Excel
                  </button>
                  <button
                    onClick={() => handleExport('csv')}
                    className="w-full px-4 py-2 text-left hover:bg-gray-100 rounded-b-lg"
                  >
                    CSV
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200">
        <nav className="flex space-x-4">
          {filteredTabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`py-3 px-4 border-b-2 font-medium text-sm ${
                activeTab === tab.id
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }`}
            >
              <span className="mr-2">{tab.icon}</span>
              {tab.label}
            </button>
          ))}
        </nav>
      </div>

      {error && (
        <div className="bg-red-50 text-red-700 p-4 rounded-lg">{error}</div>
      )}

      {/* Report Content */}
      <div className="bg-white rounded-lg shadow">
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading report...</div>
        ) : (
          <>
            {/* Daily Sales Report */}
            {activeTab === 'daily' && (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Shifts</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Sales</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Voided</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Quantity</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Total</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Cash</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Credit</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Digital</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {dailySales.map((day) => (
                      <tr key={day.date} className="hover:bg-gray-50">
                        <td className="px-4 py-3 text-sm font-medium">{new Date(day.date).toLocaleDateString()}</td>
                        <td className="px-4 py-3 text-sm text-right">{day.totalShifts}</td>
                        <td className="px-4 py-3 text-sm text-right">{day.totalSales}</td>
                        <td className="px-4 py-3 text-sm text-right text-red-600">{day.voidedSales}</td>
                        <td className="px-4 py-3 text-sm text-right">{formatNumber(day.totalQuantity)} L</td>
                        <td className="px-4 py-3 text-sm text-right font-bold">{formatCurrency(day.totalAmount)}</td>
                        <td className="px-4 py-3 text-sm text-right text-green-600">{formatCurrency(day.cashAmount)}</td>
                        <td className="px-4 py-3 text-sm text-right text-yellow-600">{formatCurrency(day.creditAmount)}</td>
                        <td className="px-4 py-3 text-sm text-right text-blue-600">{formatCurrency(day.digitalAmount)}</td>
                      </tr>
                    ))}
                    {dailySales.length === 0 && (
                      <tr>
                        <td colSpan={9} className="px-4 py-8 text-center text-gray-500">No data available</td>
                      </tr>
                    )}
                  </tbody>
                  {dailySales.length > 0 && (
                    <tfoot className="bg-gray-100">
                      <tr className="font-bold">
                        <td className="px-4 py-3 text-sm">Total</td>
                        <td className="px-4 py-3 text-sm text-right">{dailySales.reduce((sum, d) => sum + d.totalShifts, 0)}</td>
                        <td className="px-4 py-3 text-sm text-right">{dailySales.reduce((sum, d) => sum + d.totalSales, 0)}</td>
                        <td className="px-4 py-3 text-sm text-right text-red-600">{dailySales.reduce((sum, d) => sum + d.voidedSales, 0)}</td>
                        <td className="px-4 py-3 text-sm text-right">{formatNumber(dailySales.reduce((sum, d) => sum + d.totalQuantity, 0))} L</td>
                        <td className="px-4 py-3 text-sm text-right">{formatCurrency(dailySales.reduce((sum, d) => sum + d.totalAmount, 0))}</td>
                        <td className="px-4 py-3 text-sm text-right text-green-600">{formatCurrency(dailySales.reduce((sum, d) => sum + d.cashAmount, 0))}</td>
                        <td className="px-4 py-3 text-sm text-right text-yellow-600">{formatCurrency(dailySales.reduce((sum, d) => sum + d.creditAmount, 0))}</td>
                        <td className="px-4 py-3 text-sm text-right text-blue-600">{formatCurrency(dailySales.reduce((sum, d) => sum + d.digitalAmount, 0))}</td>
                      </tr>
                    </tfoot>
                  )}
                </table>
              </div>
            )}

            {/* Shifts Report */}
            {activeTab === 'shifts' && (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Worker</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Time</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Sales</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Quantity</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Total</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Cash</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Variance</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {shifts.map((shift) => (
                      <tr key={shift.shiftId} className="hover:bg-gray-50">
                        <td className="px-4 py-3 text-sm font-medium">{new Date(shift.shiftDate).toLocaleDateString()}</td>
                        <td className="px-4 py-3 text-sm">{shift.workerName}</td>
                        <td className="px-4 py-3 text-sm">
                          {shift.startTime.substring(0, 5)} - {shift.endTime ? shift.endTime.substring(0, 5) : '...'}
                        </td>
                        <td className="px-4 py-3 text-sm">{getStatusBadge(shift.status)}</td>
                        <td className="px-4 py-3 text-sm text-right">{shift.salesCount}</td>
                        <td className="px-4 py-3 text-sm text-right">{formatNumber(shift.totalQuantity)} L</td>
                        <td className="px-4 py-3 text-sm text-right font-bold">{formatCurrency(shift.totalSales)}</td>
                        <td className="px-4 py-3 text-sm text-right text-green-600">{formatCurrency(shift.cashCollected)}</td>
                        <td className={`px-4 py-3 text-sm text-right font-medium ${shift.variance < 0 ? 'text-red-600' : shift.variance > 0 ? 'text-green-600' : 'text-gray-600'}`}>
                          {formatCurrency(shift.variance)}
                        </td>
                      </tr>
                    ))}
                    {shifts.length === 0 && (
                      <tr>
                        <td colSpan={9} className="px-4 py-8 text-center text-gray-500">No data available</td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            )}

            {/* Fuel Performance Report */}
            {activeTab === 'fuel' && (
              <div className="p-6">
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  {fuelPerformance.map((fuel) => (
                    <div key={fuel.fuelTypeId} className="bg-gray-50 rounded-lg p-4">
                      <div className="flex items-center justify-between mb-3">
                        <h3 className="text-lg font-semibold">{fuel.fuelName}</h3>
                        <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">{fuel.fuelCode}</span>
                      </div>
                      <div className="space-y-2 text-sm">
                        <div className="flex justify-between">
                          <span className="text-gray-500">Total Sales</span>
                          <span className="font-bold">{formatCurrency(fuel.totalAmount)}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-gray-500">Quantity</span>
                          <span>{formatNumber(fuel.totalQuantity)} L</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-gray-500">Avg Rate</span>
                          <span>{formatCurrency(fuel.averageRate)}/L</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-gray-500">Transactions</span>
                          <span>{fuel.salesCount}</span>
                        </div>
                        <div className="mt-3">
                          <div className="flex justify-between text-xs text-gray-500 mb-1">
                            <span>Share of Total</span>
                            <span>{fuel.percentageOfTotal.toFixed(1)}%</span>
                          </div>
                          <div className="h-2 bg-gray-200 rounded-full overflow-hidden">
                            <div
                              className="h-full bg-blue-500 rounded-full"
                              style={{ width: `${fuel.percentageOfTotal}%` }}
                            />
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                  {fuelPerformance.length === 0 && (
                    <div className="col-span-full text-center text-gray-500 py-8">No data available</div>
                  )}
                </div>
              </div>
            )}

            {/* Worker Performance Report */}
            {activeTab === 'workers' && (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Worker</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Shifts</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Sales</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Quantity</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Total Amount</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Avg/Shift</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Variance</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {workerPerformance.map((worker) => (
                      <tr key={worker.userId} className="hover:bg-gray-50">
                        <td className="px-4 py-3 text-sm font-medium">{worker.workerName}</td>
                        <td className="px-4 py-3 text-sm text-right">{worker.totalShifts}</td>
                        <td className="px-4 py-3 text-sm text-right">{worker.totalSales}</td>
                        <td className="px-4 py-3 text-sm text-right">{formatNumber(worker.totalQuantity)} L</td>
                        <td className="px-4 py-3 text-sm text-right font-bold">{formatCurrency(worker.totalAmount)}</td>
                        <td className="px-4 py-3 text-sm text-right">{formatCurrency(worker.averageShiftSales)}</td>
                        <td className={`px-4 py-3 text-sm text-right font-medium ${worker.totalVariance < 0 ? 'text-red-600' : worker.totalVariance > 0 ? 'text-green-600' : 'text-gray-600'}`}>
                          {formatCurrency(worker.totalVariance)}
                        </td>
                      </tr>
                    ))}
                    {workerPerformance.length === 0 && (
                      <tr>
                        <td colSpan={7} className="px-4 py-8 text-center text-gray-500">No data available</td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            )}

            {/* Nozzle Performance Report */}
            {activeTab === 'nozzles' && (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Nozzle</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Machine</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Fuel Type</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Sales</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Quantity</th>
                      <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Total Amount</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {nozzlePerformance.map((nozzle) => (
                      <tr key={nozzle.nozzleId} className="hover:bg-gray-50">
                        <td className="px-4 py-3 text-sm font-medium">{nozzle.nozzleNumber}</td>
                        <td className="px-4 py-3 text-sm">{nozzle.machineName}</td>
                        <td className="px-4 py-3 text-sm">{nozzle.fuelType}</td>
                        <td className="px-4 py-3 text-sm text-right">{nozzle.salesCount}</td>
                        <td className="px-4 py-3 text-sm text-right">{formatNumber(nozzle.totalQuantity)} L</td>
                        <td className="px-4 py-3 text-sm text-right font-bold">{formatCurrency(nozzle.totalAmount)}</td>
                      </tr>
                    ))}
                    {nozzlePerformance.length === 0 && (
                      <tr>
                        <td colSpan={6} className="px-4 py-8 text-center text-gray-500">No data available</td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}
