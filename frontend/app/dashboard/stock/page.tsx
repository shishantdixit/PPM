'use client';

import { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import type { Tank, StockEntry, CreateStockInDto, CreateStockAdjustmentDto, StockHistoryFilter, StockEntryType } from '@/types';

export default function StockPage() {
  const [tanks, setTanks] = useState<Tank[]>([]);
  const [entries, setEntries] = useState<StockEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showStockInModal, setShowStockInModal] = useState(false);
  const [showAdjustmentModal, setShowAdjustmentModal] = useState(false);
  const [saving, setSaving] = useState(false);
  const [filter, setFilter] = useState<StockHistoryFilter>({
    page: 1,
    pageSize: 20
  });
  const [totalPages, setTotalPages] = useState(1);

  const [stockInData, setStockInData] = useState<CreateStockInDto>({
    tankId: '',
    quantity: 0,
    entryDate: new Date().toISOString(),
    reference: '',
    vendor: '',
    unitPrice: undefined,
    notes: ''
  });

  const [adjustmentData, setAdjustmentData] = useState<CreateStockAdjustmentDto>({
    tankId: '',
    quantity: 0,
    entryDate: new Date().toISOString(),
    notes: ''
  });

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    loadStockHistory();
  }, [filter]);

  const loadData = async () => {
    try {
      setLoading(true);
      const tanksRes = await api.getTanks();
      if (tanksRes.success) {
        setTanks((tanksRes.data || []).filter(t => t.isActive));
      }
    } catch {
      setError('Failed to load tanks');
    } finally {
      setLoading(false);
    }
  };

  const loadStockHistory = async () => {
    try {
      const response = await api.getStockHistory(filter);
      if (response.success && response.data) {
        setEntries(response.data.items);
        setTotalPages(response.data.totalPages);
      }
    } catch {
      setError('Failed to load stock history');
    }
  };

  const handleStockIn = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setSaving(true);
      const response = await api.recordStockIn({
        ...stockInData,
        entryDate: new Date(stockInData.entryDate).toISOString()
      });
      if (response.success) {
        await loadData();
        await loadStockHistory();
        setShowStockInModal(false);
        setStockInData({
          tankId: '',
          quantity: 0,
          entryDate: new Date().toISOString(),
          reference: '',
          vendor: '',
          unitPrice: undefined,
          notes: ''
        });
      } else {
        setError(response.message);
      }
    } catch {
      setError('Failed to record stock in');
    } finally {
      setSaving(false);
    }
  };

  const handleAdjustment = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setSaving(true);
      const response = await api.recordStockAdjustment({
        ...adjustmentData,
        entryDate: new Date(adjustmentData.entryDate).toISOString()
      });
      if (response.success) {
        await loadData();
        await loadStockHistory();
        setShowAdjustmentModal(false);
        setAdjustmentData({
          tankId: '',
          quantity: 0,
          entryDate: new Date().toISOString(),
          notes: ''
        });
      } else {
        setError(response.message);
      }
    } catch {
      setError('Failed to record adjustment');
    } finally {
      setSaving(false);
    }
  };

  const getEntryTypeLabel = (type: StockEntryType) => {
    switch (type) {
      case 0: return { label: 'Stock In', color: 'bg-green-100 text-green-800' };
      case 1: return { label: 'Stock Out', color: 'bg-red-100 text-red-800' };
      case 2: return { label: 'Adjustment', color: 'bg-yellow-100 text-yellow-800' };
      case 3: return { label: 'Transfer', color: 'bg-blue-100 text-blue-800' };
      default: return { label: 'Unknown', color: 'bg-gray-100 text-gray-800' };
    }
  };

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleString();
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg">Loading...</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Stock Management</h1>
        <div className="flex gap-2">
          <button
            onClick={() => setShowStockInModal(true)}
            className="bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700"
          >
            Record Stock In
          </button>
          <button
            onClick={() => setShowAdjustmentModal(true)}
            className="bg-yellow-600 text-white px-4 py-2 rounded-md hover:bg-yellow-700"
          >
            Record Adjustment
          </button>
        </div>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md">
          {error}
          <button onClick={() => setError(null)} className="float-right">X</button>
        </div>
      )}

      {/* Quick Stock View */}
      <div className="bg-white rounded-lg shadow p-4">
        <h2 className="text-lg font-semibold mb-4">Current Stock Levels</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {tanks.map((tank) => (
            <div key={tank.tankId} className={`border rounded-lg p-3 ${tank.isLowStock ? 'border-red-300 bg-red-50' : 'border-gray-200'}`}>
              <div className="font-medium">{tank.tankName}</div>
              <div className="text-sm text-gray-500">{tank.fuelTypeName}</div>
              <div className="mt-2 text-xl font-bold">{tank.currentStock.toLocaleString()} L</div>
              <div className="text-xs text-gray-400">of {tank.capacity.toLocaleString()} L ({tank.availablePercentage.toFixed(1)}%)</div>
              {tank.isLowStock && (
                <div className="text-xs text-red-600 mt-1">Low Stock</div>
              )}
            </div>
          ))}
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-lg shadow p-4">
        <div className="flex flex-wrap gap-4 items-end">
          <div>
            <label className="block text-sm font-medium text-gray-700">Tank</label>
            <select
              value={filter.tankId || ''}
              onChange={(e) => setFilter({ ...filter, tankId: e.target.value || undefined, page: 1 })}
              className="mt-1 block w-48 rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
              <option value="">All Tanks</option>
              {tanks.map((tank) => (
                <option key={tank.tankId} value={tank.tankId}>{tank.tankName}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">Entry Type</label>
            <select
              value={filter.entryType?.toString() || ''}
              onChange={(e) => setFilter({ ...filter, entryType: e.target.value ? parseInt(e.target.value) as StockEntryType : undefined, page: 1 })}
              className="mt-1 block w-40 rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
              <option value="">All Types</option>
              <option value="0">Stock In</option>
              <option value="1">Stock Out</option>
              <option value="2">Adjustment</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">From Date</label>
            <input
              type="date"
              value={filter.fromDate?.split('T')[0] || ''}
              onChange={(e) => setFilter({ ...filter, fromDate: e.target.value ? e.target.value + 'T00:00:00' : undefined, page: 1 })}
              className="mt-1 block w-40 rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">To Date</label>
            <input
              type="date"
              value={filter.toDate?.split('T')[0] || ''}
              onChange={(e) => setFilter({ ...filter, toDate: e.target.value ? e.target.value + 'T23:59:59' : undefined, page: 1 })}
              className="mt-1 block w-40 rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            />
          </div>

          <button
            onClick={() => setFilter({ page: 1, pageSize: 20 })}
            className="px-4 py-2 text-gray-600 hover:text-gray-800"
          >
            Clear Filters
          </button>
        </div>
      </div>

      {/* Stock History Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tank</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Type</th>
              <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Quantity</th>
              <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Before</th>
              <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">After</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Reference</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Recorded By</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {entries.map((entry) => {
              const typeInfo = getEntryTypeLabel(entry.entryType);
              return (
                <tr key={entry.stockEntryId}>
                  <td className="px-4 py-3 text-sm">{formatDate(entry.entryDate)}</td>
                  <td className="px-4 py-3 text-sm">
                    <div>{entry.tankName}</div>
                    <div className="text-xs text-gray-500">{entry.fuelTypeName}</div>
                  </td>
                  <td className="px-4 py-3">
                    <span className={`px-2 py-1 text-xs rounded-full ${typeInfo.color}`}>
                      {typeInfo.label}
                    </span>
                  </td>
                  <td className={`px-4 py-3 text-sm text-right font-medium ${entry.quantity >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                    {entry.quantity >= 0 ? '+' : ''}{entry.quantity.toLocaleString()} L
                  </td>
                  <td className="px-4 py-3 text-sm text-right">{entry.stockBefore.toLocaleString()} L</td>
                  <td className="px-4 py-3 text-sm text-right">{entry.stockAfter.toLocaleString()} L</td>
                  <td className="px-4 py-3 text-sm">
                    {entry.reference && <div>{entry.reference}</div>}
                    {entry.vendor && <div className="text-xs text-gray-500">{entry.vendor}</div>}
                    {entry.notes && <div className="text-xs text-gray-400">{entry.notes}</div>}
                  </td>
                  <td className="px-4 py-3 text-sm">{entry.recordedByName}</td>
                </tr>
              );
            })}
          </tbody>
        </table>

        {entries.length === 0 && (
          <div className="text-center py-8 text-gray-500">
            No stock entries found.
          </div>
        )}

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="px-4 py-3 bg-gray-50 flex justify-between items-center">
            <button
              onClick={() => setFilter({ ...filter, page: (filter.page || 1) - 1 })}
              disabled={(filter.page || 1) <= 1}
              className="px-3 py-1 border rounded disabled:opacity-50"
            >
              Previous
            </button>
            <span>Page {filter.page || 1} of {totalPages}</span>
            <button
              onClick={() => setFilter({ ...filter, page: (filter.page || 1) + 1 })}
              disabled={(filter.page || 1) >= totalPages}
              className="px-3 py-1 border rounded disabled:opacity-50"
            >
              Next
            </button>
          </div>
        )}
      </div>

      {/* Stock In Modal */}
      {showStockInModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Record Stock In</h2>
            <form onSubmit={handleStockIn} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Tank</label>
                <select
                  value={stockInData.tankId}
                  onChange={(e) => setStockInData({ ...stockInData, tankId: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                >
                  <option value="">Select Tank</option>
                  {tanks.map((tank) => (
                    <option key={tank.tankId} value={tank.tankId}>
                      {tank.tankName} ({tank.fuelTypeName}) - {tank.currentStock.toLocaleString()} L
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Quantity (L)</label>
                <input
                  type="number"
                  value={stockInData.quantity}
                  onChange={(e) => setStockInData({ ...stockInData, quantity: parseFloat(e.target.value) || 0 })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                  min="0.001"
                  step="0.001"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Date & Time</label>
                <input
                  type="datetime-local"
                  value={stockInData.entryDate.slice(0, 16)}
                  onChange={(e) => setStockInData({ ...stockInData, entryDate: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Invoice/Challan No.</label>
                  <input
                    type="text"
                    value={stockInData.reference}
                    onChange={(e) => setStockInData({ ...stockInData, reference: e.target.value })}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Unit Price</label>
                  <input
                    type="number"
                    value={stockInData.unitPrice || ''}
                    onChange={(e) => setStockInData({ ...stockInData, unitPrice: e.target.value ? parseFloat(e.target.value) : undefined })}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    step="0.01"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Vendor/Supplier</label>
                <input
                  type="text"
                  value={stockInData.vendor}
                  onChange={(e) => setStockInData({ ...stockInData, vendor: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Notes</label>
                <textarea
                  value={stockInData.notes}
                  onChange={(e) => setStockInData({ ...stockInData, notes: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  rows={2}
                />
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => setShowStockInModal(false)}
                  className="flex-1 px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={saving}
                  className="flex-1 px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 disabled:opacity-50"
                >
                  {saving ? 'Saving...' : 'Record Stock In'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Adjustment Modal */}
      {showAdjustmentModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Record Stock Adjustment</h2>
            <p className="text-sm text-gray-500 mb-4">
              Use positive values to increase stock, negative values to decrease.
            </p>
            <form onSubmit={handleAdjustment} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Tank</label>
                <select
                  value={adjustmentData.tankId}
                  onChange={(e) => setAdjustmentData({ ...adjustmentData, tankId: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                >
                  <option value="">Select Tank</option>
                  {tanks.map((tank) => (
                    <option key={tank.tankId} value={tank.tankId}>
                      {tank.tankName} ({tank.fuelTypeName}) - {tank.currentStock.toLocaleString()} L
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Adjustment Quantity (L)</label>
                <input
                  type="number"
                  value={adjustmentData.quantity}
                  onChange={(e) => setAdjustmentData({ ...adjustmentData, quantity: parseFloat(e.target.value) || 0 })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                  step="0.001"
                />
                <p className="text-xs text-gray-500 mt-1">
                  Positive = add stock, Negative = remove stock
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Date & Time</label>
                <input
                  type="datetime-local"
                  value={adjustmentData.entryDate.slice(0, 16)}
                  onChange={(e) => setAdjustmentData({ ...adjustmentData, entryDate: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Reason/Notes</label>
                <textarea
                  value={adjustmentData.notes}
                  onChange={(e) => setAdjustmentData({ ...adjustmentData, notes: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  rows={3}
                  placeholder="Enter the reason for this adjustment"
                  required
                />
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => setShowAdjustmentModal(false)}
                  className="flex-1 px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={saving}
                  className="flex-1 px-4 py-2 bg-yellow-600 text-white rounded-md hover:bg-yellow-700 disabled:opacity-50"
                >
                  {saving ? 'Saving...' : 'Record Adjustment'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
