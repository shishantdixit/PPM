'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { api } from '@/lib/api';
import type { Shift, ShiftStatus, CloseShiftDto, NozzleReadingInput } from '@/types';

export default function AllShiftsPage() {
  const router = useRouter();
  const [shifts, setShifts] = useState<Shift[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedShift, setSelectedShift] = useState<Shift | null>(null);
  const [showCloseModal, setShowCloseModal] = useState(false);

  // Filters
  const [statusFilter, setStatusFilter] = useState<ShiftStatus | ''>('');
  const [dateFilter, setDateFilter] = useState('');

  // Close shift form
  const [closeShiftData, setCloseShiftData] = useState<CloseShiftDto>({
    endTime: new Date().toTimeString().split(' ')[0].substring(0, 5),
    closingReadings: [],
    cashCollected: 0,
    creditSales: 0,
    digitalPayments: 0,
    borrowing: 0,
    notes: '',
  });

  useEffect(() => {
    loadShifts();
  }, [statusFilter, dateFilter]);

  const loadShifts = async () => {
    try {
      setLoading(true);
      setError(null);

      const params: any = {};
      if (statusFilter !== '') params.status = statusFilter;
      if (dateFilter) {
        params.fromDate = dateFilter;
        params.toDate = dateFilter;
      }

      const response = await api.getShifts(params);
      if (response.success && response.data) {
        setShifts(response.data);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load shifts');
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCloseModal = async (shift: Shift) => {
    try {
      // Load full shift details with readings
      const response = await api.getShift(shift.shiftId);
      if (response.success && response.data) {
        setSelectedShift(response.data);

        // Initialize closing readings
        const closingReadings: NozzleReadingInput[] = response.data.nozzleReadings.map(nr => ({
          nozzleId: nr.nozzleId,
          reading: nr.closingReading || nr.openingReading,
        }));

        setCloseShiftData({
          endTime: new Date().toTimeString().split(' ')[0].substring(0, 5),
          closingReadings,
          cashCollected: 0,
          creditSales: 0,
          digitalPayments: 0,
          borrowing: 0,
          notes: '',
        });

        setShowCloseModal(true);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load shift details');
    }
  };

  const handleCloseShift = async () => {
    if (!selectedShift) return;

    try {
      setError(null);
      const response = await api.closeShift(selectedShift.shiftId, closeShiftData);
      if (response.success) {
        setShowCloseModal(false);
        setSelectedShift(null);
        loadShifts();
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to close shift');
    }
  };

  const updateClosingReading = (nozzleId: string, reading: number) => {
    setCloseShiftData(prev => ({
      ...prev,
      closingReadings: prev.closingReadings.map(r =>
        r.nozzleId === nozzleId ? { ...r, reading } : r
      ),
    }));
  };

  const getStatusBadge = (status: ShiftStatus) => {
    const styles = {
      0: 'bg-gray-100 text-gray-800',  // Pending
      1: 'bg-green-100 text-green-800', // Active
      2: 'bg-blue-100 text-blue-800',   // Closed
    };
    const labels = ['Pending', 'Active', 'Closed'];
    return (
      <span className={`px-2 py-1 rounded text-xs font-medium ${styles[status]}`}>
        {labels[status]}
      </span>
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-gray-500">Loading shifts...</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">All Shifts</h1>
        <button
          onClick={() => router.push('/dashboard/shifts')}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
        >
          Start New Shift
        </button>
      </div>

      {error && (
        <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-lg">
          {error}
        </div>
      )}

      {/* Filters */}
      <div className="bg-white rounded-lg shadow p-4">
        <div className="grid grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Status</label>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value as ShiftStatus | '')}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg"
            >
              <option value="">All Statuses</option>
              <option value="0">Pending</option>
              <option value="1">Active</option>
              <option value="2">Closed</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Date</label>
            <input
              type="date"
              value={dateFilter}
              onChange={(e) => setDateFilter(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg"
            />
          </div>
          <div className="flex items-end">
            <button
              onClick={() => {
                setStatusFilter('');
                setDateFilter('');
              }}
              className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300"
            >
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      {/* Shifts List */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        {shifts.length === 0 ? (
          <div className="p-8 text-center text-gray-500">
            No shifts found. Try adjusting your filters.
          </div>
        ) : (
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Worker</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Time</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Total Sales</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Variance</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {shifts.map((shift) => (
                <tr key={shift.shiftId} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="text-sm font-medium text-gray-900">{shift.workerName}</div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {shift.shiftDate}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {shift.startTime} - {shift.endTime || 'Ongoing'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    {getStatusBadge(shift.status)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-right">
                    ₹{shift.totalSales.toFixed(2)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-right">
                    <span className={shift.variance < 0 ? 'text-red-600 font-medium' : 'text-green-600'}>
                      ₹{shift.variance.toFixed(2)}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm">
                    <div className="flex gap-2">
                      <button
                        onClick={() => router.push(`/dashboard/shifts/${shift.shiftId}`)}
                        className="text-blue-600 hover:text-blue-800"
                      >
                        View
                      </button>
                      {shift.status === 1 && (
                        <button
                          onClick={() => handleOpenCloseModal(shift)}
                          className="text-red-600 hover:text-red-800"
                        >
                          Close
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Close Shift Modal */}
      {showCloseModal && selectedShift && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 space-y-6">
              <div className="flex items-center justify-between">
                <h2 className="text-xl font-bold">Close Shift - {selectedShift.workerName}</h2>
                <button
                  onClick={() => setShowCloseModal(false)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  ✕
                </button>
              </div>

              {error && (
                <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-lg">
                  {error}
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">End Time</label>
                <input
                  type="time"
                  value={closeShiftData.endTime}
                  onChange={(e) => setCloseShiftData(prev => ({ ...prev, endTime: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                />
              </div>

              <div>
                <h3 className="font-semibold mb-3">Closing Meter Readings</h3>
                <div className="space-y-3">
                  {selectedShift.nozzleReadings.map((reading) => {
                    const closingReading = closeShiftData.closingReadings.find(r => r.nozzleId === reading.nozzleId);
                    const quantity = closingReading ? closingReading.reading - reading.openingReading : 0;
                    const amount = quantity * reading.rateAtShift;

                    return (
                      <div key={reading.nozzleId} className="grid grid-cols-6 gap-4 items-center p-3 bg-gray-50 rounded">
                        <div className="col-span-2">
                          <p className="text-sm font-medium">{reading.nozzleNumber} - {reading.fuelName}</p>
                          <p className="text-xs text-gray-500">{reading.machineName}</p>
                        </div>
                        <div className="text-right">
                          <p className="text-xs text-gray-500">Opening</p>
                          <p className="text-sm">{reading.openingReading.toFixed(2)}</p>
                        </div>
                        <div>
                          <label className="text-xs text-gray-500">Closing</label>
                          <input
                            type="number"
                            step="0.01"
                            value={closingReading?.reading || reading.openingReading}
                            onChange={(e) => updateClosingReading(reading.nozzleId, parseFloat(e.target.value) || 0)}
                            className="w-full px-2 py-1 border border-gray-300 rounded text-sm"
                          />
                        </div>
                        <div className="text-right">
                          <p className="text-xs text-gray-500">Quantity</p>
                          <p className="text-sm font-medium">{quantity.toFixed(2)}</p>
                        </div>
                        <div className="text-right">
                          <p className="text-xs text-gray-500">Amount</p>
                          <p className="text-sm font-medium">₹{amount.toFixed(2)}</p>
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Cash Collected</label>
                  <input
                    type="number"
                    step="0.01"
                    value={closeShiftData.cashCollected}
                    onChange={(e) => setCloseShiftData(prev => ({ ...prev, cashCollected: parseFloat(e.target.value) || 0 }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Credit Sales</label>
                  <input
                    type="number"
                    step="0.01"
                    value={closeShiftData.creditSales}
                    onChange={(e) => setCloseShiftData(prev => ({ ...prev, creditSales: parseFloat(e.target.value) || 0 }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Digital Payments</label>
                  <input
                    type="number"
                    step="0.01"
                    value={closeShiftData.digitalPayments}
                    onChange={(e) => setCloseShiftData(prev => ({ ...prev, digitalPayments: parseFloat(e.target.value) || 0 }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Borrowing</label>
                  <input
                    type="number"
                    step="0.01"
                    value={closeShiftData.borrowing}
                    onChange={(e) => setCloseShiftData(prev => ({ ...prev, borrowing: parseFloat(e.target.value) || 0 }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Notes</label>
                <textarea
                  value={closeShiftData.notes}
                  onChange={(e) => setCloseShiftData(prev => ({ ...prev, notes: e.target.value }))}
                  rows={3}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                />
              </div>

              <div className="bg-blue-50 p-4 rounded-lg">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm text-gray-600">Total Expected</p>
                    <p className="text-lg font-bold">
                      ₹{closeShiftData.closingReadings.reduce((sum, reading) => {
                        const nozzleReading = selectedShift.nozzleReadings.find(nr => nr.nozzleId === reading.nozzleId);
                        if (!nozzleReading) return sum;
                        const quantity = reading.reading - nozzleReading.openingReading;
                        return sum + (quantity * nozzleReading.rateAtShift);
                      }, 0).toFixed(2)}
                    </p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Variance</p>
                    <p className={`text-lg font-bold ${
                      (closeShiftData.closingReadings.reduce((sum, reading) => {
                        const nozzleReading = selectedShift.nozzleReadings.find(nr => nr.nozzleId === reading.nozzleId);
                        if (!nozzleReading) return sum;
                        const quantity = reading.reading - nozzleReading.openingReading;
                        return sum + (quantity * nozzleReading.rateAtShift);
                      }, 0) - (closeShiftData.cashCollected + closeShiftData.creditSales + closeShiftData.digitalPayments)) < 0
                        ? 'text-red-600'
                        : 'text-green-600'
                    }`}>
                      ₹{(closeShiftData.closingReadings.reduce((sum, reading) => {
                        const nozzleReading = selectedShift.nozzleReadings.find(nr => nr.nozzleId === reading.nozzleId);
                        if (!nozzleReading) return sum;
                        const quantity = reading.reading - nozzleReading.openingReading;
                        return sum + (quantity * nozzleReading.rateAtShift);
                      }, 0) - (closeShiftData.cashCollected + closeShiftData.creditSales + closeShiftData.digitalPayments)).toFixed(2)}
                    </p>
                  </div>
                </div>
              </div>

              <div className="flex gap-3">
                <button
                  onClick={handleCloseShift}
                  className="flex-1 py-3 bg-red-600 text-white rounded-lg hover:bg-red-700 font-medium"
                >
                  Close Shift
                </button>
                <button
                  onClick={() => setShowCloseModal(false)}
                  className="flex-1 py-3 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 font-medium"
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
