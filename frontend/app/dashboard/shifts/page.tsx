'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { api } from '@/lib/api';
import type { Shift, CreateShiftDto, CloseShiftDto, NozzleReadingInput, ShiftStatus, Worker, ShiftMachine, ShiftNozzle } from '@/types';
import SaleEntryForm from '@/components/SaleEntryForm';
import SalesList from '@/components/SalesList';

export default function ShiftsPage() {
  const router = useRouter();
  const [activeShift, setActiveShift] = useState<Shift | null>(null);
  const [workers, setWorkers] = useState<Worker[]>([]);
  const [machines, setMachines] = useState<ShiftMachine[]>([]);
  const [nozzles, setNozzles] = useState<ShiftNozzle[]>([]);
  const [selectedMachineId, setSelectedMachineId] = useState<string>('');
  const [currentUser, setCurrentUser] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [loadingNozzles, setLoadingNozzles] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [mode, setMode] = useState<'view' | 'start' | 'close'>('view');
  const [salesRefreshTrigger, setSalesRefreshTrigger] = useState(0);
  const [showAddSaleModal, setShowAddSaleModal] = useState(false);

  // Start shift form state
  const [startShiftData, setStartShiftData] = useState<CreateShiftDto>({
    workerId: undefined,
    machineId: '',
    shiftDate: new Date().toISOString().split('T')[0],
    startTime: new Date().toTimeString().split(' ')[0].substring(0, 5),
    openingReadings: [],
  });

  // Close shift form state
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
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      // Load current user from localStorage
      const user = api.getStoredUser();
      setCurrentUser(user);

      // Load active shift
      const shiftResponse = await api.getMyActiveShift();
      if (shiftResponse.success && shiftResponse.data) {
        setActiveShift(shiftResponse.data);
        setMode('view');

        // Initialize closing readings with opening readings
        const closingReadings: NozzleReadingInput[] = shiftResponse.data.nozzleReadings.map(nr => ({
          nozzleId: nr.nozzleId,
          reading: nr.openingReading,
        }));
        setCloseShiftData(prev => ({ ...prev, closingReadings }));
      } else {
        // No active shift, load machines for starting new shift
        const machinesResponse = await api.getShiftMachines();
        if (machinesResponse.success && machinesResponse.data) {
          setMachines(machinesResponse.data);
        }

        // Load workers if user is Manager or Owner
        if (user && (user.role === 'Manager' || user.role === 'Owner')) {
          const workersResponse = await api.getWorkers();
          if (workersResponse.success && workersResponse.data) {
            setWorkers(workersResponse.data);
          }
        } else if (user) {
          // For Worker role, auto-select themselves
          setStartShiftData(prev => ({ ...prev, workerId: user.userId }));
        }
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  // Load nozzles when machine is selected
  const loadMachineNozzles = async (machineId: string) => {
    if (!machineId) {
      setNozzles([]);
      setStartShiftData(prev => ({ ...prev, machineId: '', openingReadings: [] }));
      return;
    }

    try {
      setLoadingNozzles(true);
      const response = await api.getShiftMachineNozzles(machineId);
      if (response.success && response.data) {
        setNozzles(response.data);
        // Initialize opening readings with current meter readings
        const openingReadings: NozzleReadingInput[] = response.data.map(n => ({
          nozzleId: n.nozzleId,
          reading: n.currentMeterReading,
        }));
        setStartShiftData(prev => ({ ...prev, machineId, openingReadings }));
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load nozzles');
    } finally {
      setLoadingNozzles(false);
    }
  };

  const handleMachineChange = (machineId: string) => {
    setSelectedMachineId(machineId);
    loadMachineNozzles(machineId);
  };

  const handleStartShift = async () => {
    try {
      setError(null);
      const response = await api.startShift(startShiftData);
      if (response.success && response.data) {
        setActiveShift(response.data);
        setMode('view');
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to start shift');
    }
  };

  const handleCloseShift = async () => {
    if (!activeShift) return;

    try {
      setError(null);
      const response = await api.closeShift(activeShift.shiftId, closeShiftData);
      if (response.success) {
        router.push('/dashboard');
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to close shift');
    }
  };

  const updateOpeningReading = (nozzleId: string, reading: number) => {
    setStartShiftData(prev => ({
      ...prev,
      openingReadings: prev.openingReadings.map(r =>
        r.nozzleId === nozzleId ? { ...r, reading } : r
      ),
    }));
  };

  const updateClosingReading = (nozzleId: string, reading: number) => {
    setCloseShiftData(prev => ({
      ...prev,
      closingReadings: prev.closingReadings.map(r =>
        r.nozzleId === nozzleId ? { ...r, reading } : r
      ),
    }));
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-gray-500">Loading...</div>
      </div>
    );
  }

  // View active shift
  if (mode === 'view' && activeShift) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <h1 className="text-2xl font-bold">Active Shift</h1>
          <button
            onClick={() => setMode('close')}
            className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
          >
            Close Shift
          </button>
        </div>

        {error && (
          <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-lg">
            {error}
          </div>
        )}

        <div className="bg-white rounded-lg shadow p-6 space-y-4">
          <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
            <div>
              <p className="text-sm text-gray-500">Worker</p>
              <p className="font-medium">{activeShift.workerName}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Machine</p>
              <p className="font-medium">{activeShift.machineName}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Shift Date</p>
              <p className="font-medium">{activeShift.shiftDate}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Start Time</p>
              <p className="font-medium">{activeShift.startTime}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Status</p>
              <p className="font-medium">
                <span className="px-2 py-1 bg-green-100 text-green-800 rounded text-sm">
                  Active
                </span>
              </p>
            </div>
          </div>

          <div className="mt-6">
            <h3 className="font-semibold mb-3">Nozzle Readings</h3>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Nozzle</th>
                    <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Fuel</th>
                    <th className="px-4 py-2 text-right text-xs font-medium text-gray-500">Opening</th>
                    <th className="px-4 py-2 text-right text-xs font-medium text-gray-500">Rate</th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {activeShift.nozzleReadings.map((reading) => (
                    <tr key={reading.shiftNozzleReadingId}>
                      <td className="px-4 py-2 text-sm">{reading.nozzleNumber} - {reading.machineName}</td>
                      <td className="px-4 py-2 text-sm">{reading.fuelName}</td>
                      <td className="px-4 py-2 text-sm text-right">{reading.openingReading.toFixed(2)}</td>
                      <td className="px-4 py-2 text-sm text-right">₹{reading.rateAtShift.toFixed(2)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        {/* Sales Section */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Sale Entry Form - Left Side */}
          <div className="lg:col-span-1">
            <SaleEntryForm
              shift={activeShift}
              onSaleCreated={() => setSalesRefreshTrigger(prev => prev + 1)}
            />
          </div>

          {/* Sales List - Right Side */}
          <div className="lg:col-span-2">
            <SalesList
              shiftId={activeShift.shiftId}
              refreshTrigger={salesRefreshTrigger}
            />
          </div>
        </div>
      </div>
    );
  }

  // Close shift form
  if (mode === 'close' && activeShift) {
    const totalCollected = closeShiftData.cashCollected + closeShiftData.creditSales + closeShiftData.digitalPayments;
    const calculatedVariance = closeShiftData.closingReadings.reduce((sum, reading) => {
      const nozzleReading = activeShift.nozzleReadings.find(nr => nr.nozzleId === reading.nozzleId);
      if (!nozzleReading) return sum;
      const quantity = reading.reading - nozzleReading.openingReading;
      return sum + (quantity * nozzleReading.rateAtShift);
    }, 0) - totalCollected;

    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <h1 className="text-2xl font-bold">Close Shift</h1>
          <button
            onClick={() => setMode('view')}
            className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300"
          >
            Cancel
          </button>
        </div>

        {error && (
          <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-lg">
            {error}
          </div>
        )}

        {/* Sales Section */}
        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-semibold">Individual Sales</h3>
            <button
              onClick={() => setShowAddSaleModal(true)}
              className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 text-sm"
            >
              + Add Sale
            </button>
          </div>
          <SalesList shiftId={activeShift.shiftId} refreshTrigger={salesRefreshTrigger} />
        </div>

        {/* Add Sale Modal */}
        {showAddSaleModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
              <div className="sticky top-0 bg-white border-b px-6 py-4 flex items-center justify-between">
                <h3 className="text-lg font-semibold">Add Sale</h3>
                <button
                  onClick={() => setShowAddSaleModal(false)}
                  className="text-gray-400 hover:text-gray-600 text-2xl leading-none"
                >
                  ×
                </button>
              </div>
              <div className="p-6">
                <SaleEntryForm
                  shift={activeShift}
                  onSaleCreated={() => {
                    setSalesRefreshTrigger(prev => prev + 1);
                    setShowAddSaleModal(false);
                  }}
                />
              </div>
            </div>
          </div>
        )}

        <div className="bg-white rounded-lg shadow p-6 space-y-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">End Time</label>
            <input
              type="time"
              value={closeShiftData.endTime}
              onChange={(e) => setCloseShiftData(prev => ({ ...prev, endTime: e.target.value }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <h3 className="font-semibold mb-3">Closing Meter Readings</h3>
            <div className="space-y-3">
              {activeShift.nozzleReadings.map((reading) => {
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
                <p className="text-sm text-gray-600">Total Collected</p>
                <p className="text-lg font-bold">₹{totalCollected.toFixed(2)}</p>
              </div>
              <div>
                <p className="text-sm text-gray-600">Variance</p>
                <p className={`text-lg font-bold ${calculatedVariance < 0 ? 'text-red-600' : 'text-green-600'}`}>
                  ₹{calculatedVariance.toFixed(2)}
                </p>
              </div>
            </div>
          </div>

          <button
            onClick={handleCloseShift}
            className="w-full py-3 bg-red-600 text-white rounded-lg hover:bg-red-700 font-medium"
          >
            Close Shift
          </button>
        </div>
      </div>
    );
  }

  // Start new shift form
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Start New Shift</h1>

      {error && (
        <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-lg">
          {error}
        </div>
      )}

      <div className="bg-white rounded-lg shadow p-6 space-y-6">
        {/* Step 1: Worker Selection (for Manager/Owner) */}
        {currentUser && (currentUser.role === 'Manager' || currentUser.role === 'Owner') && (
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              <span className="bg-blue-100 text-blue-800 px-2 py-0.5 rounded text-xs mr-2">Step 1</span>
              Select Worker *
            </label>
            <select
              value={startShiftData.workerId || ''}
              onChange={(e) => setStartShiftData(prev => ({ ...prev, workerId: e.target.value || undefined }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              required
            >
              <option value="">-- Select Worker --</option>
              {workers.map((worker) => (
                <option key={worker.userId} value={worker.userId}>
                  {worker.fullName} ({worker.username})
                </option>
              ))}
            </select>
          </div>
        )}

        {/* Step 2: Machine Selection */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            <span className="bg-blue-100 text-blue-800 px-2 py-0.5 rounded text-xs mr-2">
              {currentUser && (currentUser.role === 'Manager' || currentUser.role === 'Owner') ? 'Step 2' : 'Step 1'}
            </span>
            Select Machine *
          </label>
          <select
            value={selectedMachineId}
            onChange={(e) => handleMachineChange(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            required
          >
            <option value="">-- Select Machine --</option>
            {machines.map((machine) => (
              <option key={machine.machineId} value={machine.machineId}>
                {machine.machineName} ({machine.machineCode}) - {machine.nozzleCount} nozzles
                {machine.location && ` - ${machine.location}`}
              </option>
            ))}
          </select>
          <p className="text-xs text-gray-500 mt-1">
            Worker will be responsible for all nozzles on the selected machine
          </p>
        </div>

        {/* Date/Time Selection */}
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Shift Date</label>
            <input
              type="date"
              value={startShiftData.shiftDate}
              onChange={(e) => setStartShiftData(prev => ({ ...prev, shiftDate: e.target.value }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Start Time</label>
            <input
              type="time"
              value={startShiftData.startTime}
              onChange={(e) => setStartShiftData(prev => ({ ...prev, startTime: e.target.value }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>

        {/* Step 3: Nozzle Readings (shown after machine is selected) */}
        {selectedMachineId && (
          <div>
            <h3 className="font-semibold mb-3">
              <span className="bg-blue-100 text-blue-800 px-2 py-0.5 rounded text-xs mr-2">
                {currentUser && (currentUser.role === 'Manager' || currentUser.role === 'Owner') ? 'Step 3' : 'Step 2'}
              </span>
              Opening Meter Readings
            </h3>
            {loadingNozzles ? (
              <div className="p-4 text-center text-gray-500">Loading nozzles...</div>
            ) : nozzles.length === 0 ? (
              <div className="p-4 text-center text-yellow-600 bg-yellow-50 rounded">
                No active nozzles found for this machine
              </div>
            ) : (
              <div className="space-y-3">
                {nozzles.map((nozzle) => {
                  const reading = startShiftData.openingReadings.find(r => r.nozzleId === nozzle.nozzleId);
                  return (
                    <div key={nozzle.nozzleId} className="grid grid-cols-4 gap-4 items-center p-3 bg-gray-50 rounded">
                      <div className="col-span-2">
                        <p className="text-sm font-medium">{nozzle.nozzleNumber} - {nozzle.fuelName}</p>
                        <p className="text-xs text-gray-500">{nozzle.nozzleName || nozzle.fuelCode}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-xs text-gray-500">Current Reading</p>
                        <p className="text-sm">{nozzle.currentMeterReading.toFixed(2)}</p>
                      </div>
                      <div>
                        <label className="text-xs text-gray-500">Opening Reading</label>
                        <input
                          type="number"
                          step="0.01"
                          value={reading?.reading ?? nozzle.currentMeterReading}
                          onChange={(e) => updateOpeningReading(nozzle.nozzleId, parseFloat(e.target.value) || 0)}
                          className="w-full px-2 py-1 border border-gray-300 rounded text-sm"
                        />
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        )}

        <button
          onClick={handleStartShift}
          disabled={!selectedMachineId || nozzles.length === 0}
          className={`w-full py-3 rounded-lg font-medium ${
            !selectedMachineId || nozzles.length === 0
              ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
              : 'bg-blue-600 text-white hover:bg-blue-700'
          }`}
        >
          Start Shift
        </button>
      </div>
    </div>
  );
}
