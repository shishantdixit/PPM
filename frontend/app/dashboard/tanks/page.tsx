'use client';

import { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import type { Tank, TankSummary, FuelType, CreateTankDto, UpdateTankDto } from '@/types';

export default function TanksPage() {
  const [tanks, setTanks] = useState<Tank[]>([]);
  const [fuelTypes, setFuelTypes] = useState<FuelType[]>([]);
  const [summary, setSummary] = useState<TankSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [editingTank, setEditingTank] = useState<Tank | null>(null);
  const [formData, setFormData] = useState<CreateTankDto>({
    tankName: '',
    tankCode: '',
    fuelTypeId: '',
    capacity: 0,
    currentStock: 0,
    minimumLevel: 0,
    location: '',
  });
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [tanksRes, summaryRes, fuelTypesRes] = await Promise.all([
        api.getTanks(),
        api.getTankSummary(),
        api.getFuelTypes()
      ]);

      if (tanksRes.success) setTanks(tanksRes.data || []);
      if (summaryRes.success) setSummary(summaryRes.data);
      if (fuelTypesRes.success) setFuelTypes((fuelTypesRes.data || []).filter(ft => ft.isActive));
    } catch {
      setError('Failed to load tanks');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setSaving(true);
      const response = await api.createTank(formData);
      if (response.success) {
        await loadData();
        setShowCreateModal(false);
        resetForm();
      } else {
        setError(response.message);
      }
    } catch {
      setError('Failed to create tank');
    } finally {
      setSaving(false);
    }
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingTank) return;

    try {
      setSaving(true);
      const updateData: UpdateTankDto = {
        tankName: formData.tankName,
        fuelTypeId: formData.fuelTypeId,
        capacity: formData.capacity,
        minimumLevel: formData.minimumLevel,
        location: formData.location || undefined,
      };

      const response = await api.updateTank(editingTank.tankId, updateData);
      if (response.success) {
        await loadData();
        setEditingTank(null);
        resetForm();
      } else {
        setError(response.message);
      }
    } catch {
      setError('Failed to update tank');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this tank?')) return;

    try {
      const response = await api.deleteTank(id);
      if (response.success) {
        await loadData();
      } else {
        setError(response.message);
      }
    } catch {
      setError('Failed to delete tank');
    }
  };

  const startEdit = (tank: Tank) => {
    setEditingTank(tank);
    setFormData({
      tankName: tank.tankName,
      tankCode: tank.tankCode,
      fuelTypeId: tank.fuelTypeId,
      capacity: tank.capacity,
      currentStock: tank.currentStock,
      minimumLevel: tank.minimumLevel,
      location: tank.location || '',
    });
  };

  const resetForm = () => {
    setFormData({
      tankName: '',
      tankCode: '',
      fuelTypeId: fuelTypes[0]?.fuelTypeId || '',
      capacity: 0,
      currentStock: 0,
      minimumLevel: 0,
      location: '',
    });
  };

  const getStockLevelColor = (tank: Tank) => {
    if (tank.availablePercentage >= 50) return 'bg-green-500';
    if (tank.availablePercentage >= 25) return 'bg-yellow-500';
    return 'bg-red-500';
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg">Loading tanks...</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Tank Management</h1>
        <button
          onClick={() => {
            resetForm();
            setShowCreateModal(true);
          }}
          className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
        >
          Add Tank
        </button>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md">
          {error}
        </div>
      )}

      {/* Summary Cards */}
      {summary && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="bg-white p-4 rounded-lg shadow">
            <div className="text-sm text-gray-500">Total Tanks</div>
            <div className="text-2xl font-bold">{summary.totalTanks}</div>
          </div>
          <div className="bg-white p-4 rounded-lg shadow">
            <div className="text-sm text-gray-500">Active Tanks</div>
            <div className="text-2xl font-bold text-green-600">{summary.activeTanks}</div>
          </div>
          <div className="bg-white p-4 rounded-lg shadow">
            <div className="text-sm text-gray-500">Low Stock Alerts</div>
            <div className="text-2xl font-bold text-red-600">{summary.lowStockTanks}</div>
          </div>
          <div className="bg-white p-4 rounded-lg shadow">
            <div className="text-sm text-gray-500">Total Stock</div>
            <div className="text-2xl font-bold">{summary.totalCurrentStock.toLocaleString()} L</div>
            <div className="text-xs text-gray-400">of {summary.totalCapacity.toLocaleString()} L capacity</div>
          </div>
        </div>
      )}

      {/* Tanks Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {tanks.map((tank) => (
          <div key={tank.tankId} className={`bg-white rounded-lg shadow p-4 ${!tank.isActive ? 'opacity-60' : ''}`}>
            <div className="flex justify-between items-start mb-3">
              <div>
                <h3 className="font-semibold text-lg">{tank.tankName}</h3>
                <p className="text-sm text-gray-500">{tank.tankCode}</p>
              </div>
              <span className={`px-2 py-1 text-xs rounded-full ${tank.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                {tank.isActive ? 'Active' : 'Inactive'}
              </span>
            </div>

            <div className="mb-3">
              <div className="flex justify-between text-sm mb-1">
                <span className="text-gray-600">Fuel Type:</span>
                <span className="font-medium">{tank.fuelTypeName}</span>
              </div>
              <div className="flex justify-between text-sm mb-1">
                <span className="text-gray-600">Location:</span>
                <span className="font-medium">{tank.location || '-'}</span>
              </div>
            </div>

            {/* Stock Level Bar */}
            <div className="mb-3">
              <div className="flex justify-between text-sm mb-1">
                <span>Stock Level</span>
                <span>{tank.availablePercentage.toFixed(1)}%</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-3">
                <div
                  className={`${getStockLevelColor(tank)} h-3 rounded-full transition-all`}
                  style={{ width: `${Math.min(tank.availablePercentage, 100)}%` }}
                />
              </div>
              <div className="flex justify-between text-xs text-gray-500 mt-1">
                <span>{tank.currentStock.toLocaleString()} L</span>
                <span>{tank.capacity.toLocaleString()} L</span>
              </div>
              {tank.isLowStock && (
                <div className="mt-1 text-xs text-red-600 flex items-center gap-1">
                  <span>Low Stock Alert (Min: {tank.minimumLevel.toLocaleString()} L)</span>
                </div>
              )}
            </div>

            <div className="flex gap-2 pt-3 border-t">
              <button
                onClick={() => startEdit(tank)}
                className="flex-1 text-sm text-blue-600 hover:text-blue-800"
              >
                Edit
              </button>
              <button
                onClick={() => handleDelete(tank.tankId)}
                className="flex-1 text-sm text-red-600 hover:text-red-800"
              >
                Delete
              </button>
            </div>
          </div>
        ))}
      </div>

      {tanks.length === 0 && (
        <div className="text-center py-12 bg-gray-50 rounded-lg">
          <p className="text-gray-500">No tanks configured yet.</p>
          <button
            onClick={() => setShowCreateModal(true)}
            className="mt-2 text-blue-600 hover:text-blue-800"
          >
            Add your first tank
          </button>
        </div>
      )}

      {/* Create/Edit Modal */}
      {(showCreateModal || editingTank) && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">
              {editingTank ? 'Edit Tank' : 'Add New Tank'}
            </h2>
            <form onSubmit={editingTank ? handleUpdate : handleCreate} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Tank Name</label>
                <input
                  type="text"
                  value={formData.tankName}
                  onChange={(e) => setFormData({ ...formData, tankName: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                />
              </div>

              {!editingTank && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Tank Code</label>
                  <input
                    type="text"
                    value={formData.tankCode}
                    onChange={(e) => setFormData({ ...formData, tankCode: e.target.value.toUpperCase() })}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    required
                    placeholder="e.g., T001"
                  />
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700">Fuel Type</label>
                <select
                  value={formData.fuelTypeId}
                  onChange={(e) => setFormData({ ...formData, fuelTypeId: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  required
                >
                  <option value="">Select Fuel Type</option>
                  {fuelTypes.map((ft) => (
                    <option key={ft.fuelTypeId} value={ft.fuelTypeId}>
                      {ft.fuelName} ({ft.fuelCode})
                    </option>
                  ))}
                </select>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Capacity (L)</label>
                  <input
                    type="number"
                    value={formData.capacity}
                    onChange={(e) => setFormData({ ...formData, capacity: parseFloat(e.target.value) || 0 })}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    required
                    min="0"
                    step="0.01"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Minimum Level (L)</label>
                  <input
                    type="number"
                    value={formData.minimumLevel}
                    onChange={(e) => setFormData({ ...formData, minimumLevel: parseFloat(e.target.value) || 0 })}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    required
                    min="0"
                    step="0.01"
                  />
                </div>
              </div>

              {!editingTank && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Current Stock (L)</label>
                  <input
                    type="number"
                    value={formData.currentStock}
                    onChange={(e) => setFormData({ ...formData, currentStock: parseFloat(e.target.value) || 0 })}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    min="0"
                    step="0.01"
                  />
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700">Location</label>
                <input
                  type="text"
                  value={formData.location}
                  onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  placeholder="e.g., North Side"
                />
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => {
                    setShowCreateModal(false);
                    setEditingTank(null);
                    resetForm();
                  }}
                  className="flex-1 px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={saving}
                  className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
                >
                  {saving ? 'Saving...' : (editingTank ? 'Update' : 'Create')}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
