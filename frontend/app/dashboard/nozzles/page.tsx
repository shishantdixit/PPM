'use client';

import { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import type { Nozzle, CreateNozzleDto, UpdateNozzleDto, Machine, FuelType } from '@/types';

export default function NozzlesPage() {
  const [nozzles, setNozzles] = useState<Nozzle[]>([]);
  const [machines, setMachines] = useState<Machine[]>([]);
  const [fuelTypes, setFuelTypes] = useState<FuelType[]>([]);
  const [selectedMachine, setSelectedMachine] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    machineId: '',
    fuelTypeId: '',
    nozzleNumber: '',
    nozzleName: '',
    currentMeterReading: '',
  });

  const fetchNozzles = async (machineId?: string) => {
    try {
      setLoading(true);
      const response = await api.getNozzles(machineId);
      if (response.success && response.data) {
        setNozzles(response.data);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to fetch nozzles');
    } finally {
      setLoading(false);
    }
  };

  const fetchMachines = async () => {
    try {
      const response = await api.getMachines(true); // Active only
      if (response.success && response.data) {
        setMachines(response.data);
      }
    } catch (err: any) {
      console.error('Failed to fetch machines:', err);
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

  useEffect(() => {
    fetchNozzles();
    fetchMachines();
    fetchFuelTypes();
  }, []);

  useEffect(() => {
    fetchNozzles(selectedMachine || undefined);
  }, [selectedMachine]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    try {
      if (editingId) {
        const dto: UpdateNozzleDto = {
          nozzleName: formData.nozzleName || undefined,
          currentMeterReading: formData.currentMeterReading ? parseFloat(formData.currentMeterReading) : undefined,
        };
        await api.updateNozzle(editingId, dto);
      } else {
        const dto: CreateNozzleDto = {
          machineId: formData.machineId,
          fuelTypeId: formData.fuelTypeId,
          nozzleNumber: formData.nozzleNumber,
          nozzleName: formData.nozzleName || undefined,
          currentMeterReading: formData.currentMeterReading ? parseFloat(formData.currentMeterReading) : 0,
        };
        await api.createNozzle(dto);
      }

      setShowForm(false);
      setEditingId(null);
      setFormData({
        machineId: '',
        fuelTypeId: '',
        nozzleNumber: '',
        nozzleName: '',
        currentMeterReading: '',
      });
      fetchNozzles(selectedMachine || undefined);
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Operation failed');
    }
  };

  const handleEdit = (nozzle: Nozzle) => {
    setEditingId(nozzle.nozzleId);
    setFormData({
      machineId: nozzle.machineId,
      fuelTypeId: nozzle.fuelTypeId,
      nozzleNumber: nozzle.nozzleNumber,
      nozzleName: nozzle.nozzleName || '',
      currentMeterReading: nozzle.currentMeterReading.toString(),
    });
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this nozzle?')) return;

    try {
      await api.deleteNozzle(id);
      fetchNozzles(selectedMachine || undefined);
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Delete failed');
    }
  };

  const handleCancel = () => {
    setShowForm(false);
    setEditingId(null);
    setFormData({
      machineId: '',
      fuelTypeId: '',
      nozzleNumber: '',
      nozzleName: '',
      currentMeterReading: '',
    });
    setError('');
  };

  if (loading) {
    return <div className="text-center py-8">Loading...</div>;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Nozzles</h1>
        <div className="flex gap-2">
          <select
            value={selectedMachine}
            onChange={(e) => setSelectedMachine(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">All Machines</option>
            {machines.map((machine) => (
              <option key={machine.machineId} value={machine.machineId}>
                {machine.machineName} ({machine.machineCode})
              </option>
            ))}
          </select>
          {!showForm && (
            <button
              onClick={() => setShowForm(true)}
              className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
            >
              Add Nozzle
            </button>
          )}
        </div>
      </div>

      {error && (
        <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      )}

      {showForm && (
        <div className="bg-white p-6 rounded-lg shadow mb-6">
          <h2 className="text-lg font-semibold mb-4">
            {editingId ? 'Edit Nozzle' : 'Add New Nozzle'}
          </h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Machine *
                </label>
                <select
                  required
                  disabled={!!editingId}
                  value={formData.machineId}
                  onChange={(e) => setFormData({ ...formData, machineId: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
                >
                  <option value="">Select machine</option>
                  {machines.map((machine) => (
                    <option key={machine.machineId} value={machine.machineId}>
                      {machine.machineName} ({machine.machineCode})
                    </option>
                  ))}
                </select>
                {editingId && (
                  <p className="text-xs text-gray-500 mt-1">Machine cannot be changed</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Fuel Type *
                </label>
                <select
                  required
                  disabled={!!editingId}
                  value={formData.fuelTypeId}
                  onChange={(e) => setFormData({ ...formData, fuelTypeId: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
                >
                  <option value="">Select fuel type</option>
                  {fuelTypes.map((fuelType) => (
                    <option key={fuelType.fuelTypeId} value={fuelType.fuelTypeId}>
                      {fuelType.fuelName} ({fuelType.fuelCode})
                    </option>
                  ))}
                </select>
                {editingId && (
                  <p className="text-xs text-gray-500 mt-1">Fuel type cannot be changed</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Nozzle Number *
                </label>
                <input
                  type="text"
                  required
                  disabled={!!editingId}
                  value={formData.nozzleNumber}
                  onChange={(e) => setFormData({ ...formData, nozzleNumber: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
                  placeholder="e.g., 1, 2, A, B"
                />
                {editingId && (
                  <p className="text-xs text-gray-500 mt-1">Nozzle number cannot be changed</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Nozzle Name
                </label>
                <input
                  type="text"
                  value={formData.nozzleName}
                  onChange={(e) => setFormData({ ...formData, nozzleName: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Optional display name"
                />
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Current Meter Reading
                </label>
                <input
                  type="number"
                  step="0.01"
                  value={formData.currentMeterReading}
                  onChange={(e) => setFormData({ ...formData, currentMeterReading: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="0.00"
                />
                <p className="text-xs text-gray-500 mt-1">
                  {editingId
                    ? 'Note: Meter reading can only be increased, not decreased'
                    : 'Initial meter reading (optional, defaults to 0)'}
                </p>
              </div>
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
                Nozzle
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Machine
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Fuel Type
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Meter Reading
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
            {nozzles.length === 0 ? (
              <tr>
                <td colSpan={6} className="px-6 py-4 text-center text-gray-500">
                  No nozzles found. Add one to get started.
                </td>
              </tr>
            ) : (
              nozzles.map((nozzle) => (
                <tr key={nozzle.nozzleId}>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="text-sm font-medium text-gray-900">
                      Nozzle {nozzle.nozzleNumber}
                    </div>
                    {nozzle.nozzleName && (
                      <div className="text-sm text-gray-500">{nozzle.nozzleName}</div>
                    )}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="text-sm text-gray-900">{nozzle.machineName}</div>
                    <div className="text-sm text-gray-500">{nozzle.machineCode}</div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="text-sm text-gray-900">{nozzle.fuelName}</div>
                    <div className="text-sm text-gray-500">{nozzle.fuelCode}</div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="text-sm font-mono text-gray-900">
                      {nozzle.currentMeterReading.toFixed(2)}
                    </div>
                    <div className="text-xs text-gray-500">{nozzle.fuelUnit}</div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span
                      className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                        nozzle.isActive
                          ? 'bg-green-100 text-green-800'
                          : 'bg-red-100 text-red-800'
                      }`}
                    >
                      {nozzle.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(nozzle)}
                      className="text-blue-600 hover:text-blue-900 mr-4"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(nozzle.nozzleId)}
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
