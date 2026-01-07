'use client';

import { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import type { CurrentFuelRate, FuelRate, CreateFuelRateDto, FuelType } from '@/types';
import { format } from 'date-fns';

export default function FuelRatesPage() {
  const [currentRates, setCurrentRates] = useState<CurrentFuelRate[]>([]);
  const [fuelTypes, setFuelTypes] = useState<FuelType[]>([]);
  const [selectedFuelType, setSelectedFuelType] = useState<string | null>(null);
  const [rateHistory, setRateHistory] = useState<FuelRate[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({
    fuelTypeId: '',
    rate: '',
    effectiveFrom: new Date().toISOString().slice(0, 16),
  });

  const fetchCurrentRates = async () => {
    try {
      setLoading(true);
      const response = await api.getCurrentRates();
      if (response.success && response.data) {
        setCurrentRates(response.data);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to fetch current rates');
    } finally {
      setLoading(false);
    }
  };

  const fetchFuelTypes = async () => {
    try {
      const response = await api.getFuelTypes();
      if (response.success && response.data) {
        setFuelTypes(response.data.filter(ft => ft.isActive));
      }
    } catch (err: any) {
      console.error('Failed to fetch fuel types:', err);
    }
  };

  const fetchRateHistory = async (fuelTypeId: string) => {
    try {
      const response = await api.getRateHistory(fuelTypeId);
      if (response.success && response.data) {
        setRateHistory(response.data);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to fetch rate history');
    }
  };

  useEffect(() => {
    fetchCurrentRates();
    fetchFuelTypes();
  }, []);

  useEffect(() => {
    if (selectedFuelType) {
      fetchRateHistory(selectedFuelType);
    } else {
      setRateHistory([]);
    }
  }, [selectedFuelType]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    try {
      const dto: CreateFuelRateDto = {
        fuelTypeId: formData.fuelTypeId,
        rate: parseFloat(formData.rate),
        effectiveFrom: new Date(formData.effectiveFrom).toISOString(),
      };
      await api.createFuelRate(dto);

      setShowForm(false);
      setFormData({
        fuelTypeId: '',
        rate: '',
        effectiveFrom: new Date().toISOString().slice(0, 16),
      });
      fetchCurrentRates();
      if (selectedFuelType) {
        fetchRateHistory(selectedFuelType);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Operation failed');
    }
  };

  const handleCancel = () => {
    setShowForm(false);
    setFormData({
      fuelTypeId: '',
      rate: '',
      effectiveFrom: new Date().toISOString().slice(0, 16),
    });
    setError('');
  };

  if (loading) {
    return <div className="text-center py-8">Loading...</div>;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Fuel Rates</h1>
        {!showForm && (
          <button
            onClick={() => setShowForm(true)}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
          >
            Update Rate
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
          <h2 className="text-lg font-semibold mb-4">Update Fuel Rate</h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Fuel Type
              </label>
              <select
                required
                value={formData.fuelTypeId}
                onChange={(e) => setFormData({ ...formData, fuelTypeId: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Select fuel type</option>
                {fuelTypes.map((ft) => (
                  <option key={ft.fuelTypeId} value={ft.fuelTypeId}>
                    {ft.fuelName} ({ft.fuelCode})
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                New Rate (per {fuelTypes.find(ft => ft.fuelTypeId === formData.fuelTypeId)?.unit || 'unit'})
              </label>
              <input
                type="number"
                step="0.01"
                required
                value={formData.rate}
                onChange={(e) => setFormData({ ...formData, rate: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="0.00"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Effective From
              </label>
              <input
                type="datetime-local"
                required
                value={formData.effectiveFrom}
                onChange={(e) => setFormData({ ...formData, effectiveFrom: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div className="flex gap-2">
              <button
                type="submit"
                className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
              >
                Update Rate
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

      {/* Current Rates */}
      <div className="bg-white rounded-lg shadow mb-6">
        <div className="px-6 py-4 border-b border-gray-200">
          <h2 className="text-lg font-semibold">Current Rates</h2>
        </div>
        <div className="p-6">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {currentRates.length === 0 ? (
              <div className="col-span-3 text-center text-gray-500 py-8">
                No current rates found. Update a rate to get started.
              </div>
            ) : (
              currentRates.map((rate) => (
                <div
                  key={rate.fuelTypeId}
                  className="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
                  onClick={() => setSelectedFuelType(rate.fuelTypeId)}
                >
                  <div className="flex justify-between items-start mb-2">
                    <div>
                      <h3 className="font-semibold text-gray-900">{rate.fuelName}</h3>
                      <p className="text-xs text-gray-500">{rate.fuelCode}</p>
                    </div>
                    <span className="text-xs bg-green-100 text-green-800 px-2 py-1 rounded">
                      Current
                    </span>
                  </div>
                  <div className="text-2xl font-bold text-blue-600 mb-1">
                    ₹{rate.currentRate.toFixed(2)}
                  </div>
                  <div className="text-xs text-gray-500">
                    per {rate.unit}
                  </div>
                  <div className="text-xs text-gray-400 mt-2">
                    Since {format(new Date(rate.effectiveFrom), 'MMM d, yyyy h:mm a')}
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>

      {/* Rate History */}
      {selectedFuelType && (
        <div className="bg-white rounded-lg shadow">
          <div className="px-6 py-4 border-b border-gray-200 flex justify-between items-center">
            <h2 className="text-lg font-semibold">
              Rate History - {currentRates.find(r => r.fuelTypeId === selectedFuelType)?.fuelName}
            </h2>
            <button
              onClick={() => setSelectedFuelType(null)}
              className="text-sm text-gray-600 hover:text-gray-900"
            >
              Close
            </button>
          </div>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Rate
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Effective From
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Effective To
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Updated By
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {rateHistory.map((rate) => (
                  <tr key={rate.fuelRateId}>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      ₹{rate.rate.toFixed(2)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {format(new Date(rate.effectiveFrom), 'MMM d, yyyy h:mm a')}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {rate.effectiveTo ? format(new Date(rate.effectiveTo), 'MMM d, yyyy h:mm a') : '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {rate.updatedByName || 'N/A'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                          !rate.effectiveTo
                            ? 'bg-green-100 text-green-800'
                            : 'bg-gray-100 text-gray-800'
                        }`}
                      >
                        {!rate.effectiveTo ? 'Current' : 'Past'}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
