'use client';

import { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import type { FuelType, CreateFuelTypeDto, UpdateFuelTypeDto } from '@/types';

export default function FuelTypesPage() {
  const [fuelTypes, setFuelTypes] = useState<FuelType[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    fuelName: '',
    fuelCode: '',
    unit: 'Liters',
  });

  const fetchFuelTypes = async () => {
    try {
      setLoading(true);
      const response = await api.getFuelTypes();
      if (response.success && response.data) {
        setFuelTypes(response.data);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to fetch fuel types');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchFuelTypes();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    try {
      if (editingId) {
        const dto: UpdateFuelTypeDto = {
          fuelName: formData.fuelName,
          unit: formData.unit,
        };
        await api.updateFuelType(editingId, dto);
      } else {
        const dto: CreateFuelTypeDto = {
          fuelName: formData.fuelName,
          fuelCode: formData.fuelCode,
          unit: formData.unit,
        };
        await api.createFuelType(dto);
      }

      setShowForm(false);
      setEditingId(null);
      setFormData({ fuelName: '', fuelCode: '', unit: 'Liters' });
      fetchFuelTypes();
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Operation failed');
    }
  };

  const handleEdit = (fuelType: FuelType) => {
    setEditingId(fuelType.fuelTypeId);
    setFormData({
      fuelName: fuelType.fuelName,
      fuelCode: fuelType.fuelCode,
      unit: fuelType.unit,
    });
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this fuel type?')) return;

    try {
      await api.deleteFuelType(id);
      fetchFuelTypes();
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Delete failed');
    }
  };

  const handleCancel = () => {
    setShowForm(false);
    setEditingId(null);
    setFormData({ fuelName: '', fuelCode: '', unit: 'Liters' });
    setError('');
  };

  if (loading) {
    return <div className="text-center py-8">Loading...</div>;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Fuel Types</h1>
        {!showForm && (
          <button
            onClick={() => setShowForm(true)}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
          >
            Add Fuel Type
          </button>
        )}
      </div>

      {error && (
        <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      )}

      {showForm && (
        <div className="bg-white p-6 rounded-lg shadow mb-6">
          <h2 className="text-lg font-semibold mb-4">
            {editingId ? 'Edit Fuel Type' : 'Add New Fuel Type'}
          </h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Fuel Name
              </label>
              <input
                type="text"
                required
                value={formData.fuelName}
                onChange={(e) => setFormData({ ...formData, fuelName: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="e.g., Petrol, Diesel"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Fuel Code
              </label>
              <input
                type="text"
                required
                disabled={!!editingId}
                value={formData.fuelCode}
                onChange={(e) => setFormData({ ...formData, fuelCode: e.target.value.toUpperCase() })}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
                placeholder="e.g., PTL, DSL"
              />
              {editingId && (
                <p className="text-xs text-gray-500 mt-1">Fuel code cannot be changed</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Unit
              </label>
              <input
                type="text"
                required
                value={formData.unit}
                onChange={(e) => setFormData({ ...formData, unit: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="e.g., Liters, Gallons"
              />
            </div>

            <div className="flex gap-2">
              <button
                type="submit"
                className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
              >
                {editingId ? 'Update' : 'Create'}
              </button>
              <button
                type="button"
                onClick={handleCancel}
                className="bg-gray-200 hover:bg-gray-300 text-gray-700 px-4 py-2 rounded-md text-sm font-medium"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Fuel Name
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Code
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Unit
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {fuelTypes.length === 0 ? (
              <tr>
                <td colSpan={5} className="px-6 py-4 text-center text-gray-500">
                  No fuel types found. Add one to get started.
                </td>
              </tr>
            ) : (
              fuelTypes.map((fuelType) => (
                <tr key={fuelType.fuelTypeId}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {fuelType.fuelName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {fuelType.fuelCode}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {fuelType.unit}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span
                      className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                        fuelType.isActive
                          ? 'bg-green-100 text-green-800'
                          : 'bg-red-100 text-red-800'
                      }`}
                    >
                      {fuelType.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(fuelType)}
                      className="text-blue-600 hover:text-blue-900 mr-4"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(fuelType.fuelTypeId)}
                      className="text-red-600 hover:text-red-900"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
