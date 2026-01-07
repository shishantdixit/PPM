'use client';

import { useEffect, useState } from 'react';
import { api } from '@/lib/api';
import type { Machine, CreateMachineDto, UpdateMachineDto } from '@/types';
import { format } from 'date-fns';

export default function MachinesPage() {
  const [machines, setMachines] = useState<Machine[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    machineName: '',
    machineCode: '',
    serialNumber: '',
    manufacturer: '',
    model: '',
    installationDate: '',
    location: '',
  });

  const fetchMachines = async () => {
    try {
      setLoading(true);
      const response = await api.getMachines();
      if (response.success && response.data) {
        setMachines(response.data);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to fetch machines');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMachines();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    try {
      if (editingId) {
        const dto: UpdateMachineDto = {
          machineName: formData.machineName,
          serialNumber: formData.serialNumber || undefined,
          manufacturer: formData.manufacturer || undefined,
          model: formData.model || undefined,
          installationDate: formData.installationDate || undefined,
          location: formData.location || undefined,
        };
        await api.updateMachine(editingId, dto);
      } else {
        const dto: CreateMachineDto = {
          machineName: formData.machineName,
          machineCode: formData.machineCode,
          serialNumber: formData.serialNumber || undefined,
          manufacturer: formData.manufacturer || undefined,
          model: formData.model || undefined,
          installationDate: formData.installationDate || undefined,
          location: formData.location || undefined,
        };
        await api.createMachine(dto);
      }

      setShowForm(false);
      setEditingId(null);
      setFormData({
        machineName: '',
        machineCode: '',
        serialNumber: '',
        manufacturer: '',
        model: '',
        installationDate: '',
        location: '',
      });
      fetchMachines();
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Operation failed');
    }
  };

  const handleEdit = (machine: Machine) => {
    setEditingId(machine.machineId);
    setFormData({
      machineName: machine.machineName,
      machineCode: machine.machineCode,
      serialNumber: machine.serialNumber || '',
      manufacturer: machine.manufacturer || '',
      model: machine.model || '',
      installationDate: machine.installationDate || '',
      location: machine.location || '',
    });
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this machine?')) return;

    try {
      await api.deleteMachine(id);
      fetchMachines();
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Delete failed');
    }
  };

  const handleCancel = () => {
    setShowForm(false);
    setEditingId(null);
    setFormData({
      machineName: '',
      machineCode: '',
      serialNumber: '',
      manufacturer: '',
      model: '',
      installationDate: '',
      location: '',
    });
    setError('');
  };

  if (loading) {
    return <div className="text-center py-8">Loading...</div>;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Machines</h1>
        {!showForm && (
          <button
            onClick={() => setShowForm(true)}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
          >
            Add Machine
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
            {editingId ? 'Edit Machine' : 'Add New Machine'}
          </h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Machine Name *
                </label>
                <input
                  type="text"
                  required
                  value={formData.machineName}
                  onChange={(e) => setFormData({ ...formData, machineName: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="e.g., Dispenser 1"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Machine Code *
                </label>
                <input
                  type="text"
                  required
                  disabled={!!editingId}
                  value={formData.machineCode}
                  onChange={(e) => setFormData({ ...formData, machineCode: e.target.value.toUpperCase() })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
                  placeholder="e.g., M001"
                />
                {editingId && (
                  <p className="text-xs text-gray-500 mt-1">Machine code cannot be changed</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Serial Number
                </label>
                <input
                  type="text"
                  value={formData.serialNumber}
                  onChange={(e) => setFormData({ ...formData, serialNumber: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="e.g., SN123456"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Manufacturer
                </label>
                <input
                  type="text"
                  value={formData.manufacturer}
                  onChange={(e) => setFormData({ ...formData, manufacturer: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="e.g., Gilbarco, Wayne"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Model
                </label>
                <input
                  type="text"
                  value={formData.model}
                  onChange={(e) => setFormData({ ...formData, model: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="e.g., Encore 700S"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Installation Date
                </label>
                <input
                  type="date"
                  value={formData.installationDate}
                  onChange={(e) => setFormData({ ...formData, installationDate: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Location
                </label>
                <input
                  type="text"
                  value={formData.location}
                  onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="e.g., North Side, Bay 1"
                />
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

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {machines.length === 0 ? (
          <div className="col-span-3 bg-white rounded-lg shadow p-8 text-center text-gray-500">
            No machines found. Add one to get started.
          </div>
        ) : (
          machines.map((machine) => (
            <div key={machine.machineId} className="bg-white rounded-lg shadow hover:shadow-lg transition-shadow">
              <div className="p-6">
                <div className="flex justify-between items-start mb-4">
                  <div>
                    <h3 className="text-lg font-semibold text-gray-900">{machine.machineName}</h3>
                    <p className="text-sm text-gray-500">{machine.machineCode}</p>
                  </div>
                  <span
                    className={`px-2 py-1 text-xs font-semibold rounded-full ${
                      machine.isActive
                        ? 'bg-green-100 text-green-800'
                        : 'bg-red-100 text-red-800'
                    }`}
                  >
                    {machine.isActive ? 'Active' : 'Inactive'}
                  </span>
                </div>

                <div className="space-y-2 text-sm text-gray-600 mb-4">
                  {machine.serialNumber && (
                    <div>
                      <span className="font-medium">Serial:</span> {machine.serialNumber}
                    </div>
                  )}
                  {machine.manufacturer && (
                    <div>
                      <span className="font-medium">Manufacturer:</span> {machine.manufacturer}
                    </div>
                  )}
                  {machine.model && (
                    <div>
                      <span className="font-medium">Model:</span> {machine.model}
                    </div>
                  )}
                  {machine.installationDate && (
                    <div>
                      <span className="font-medium">Installed:</span> {format(new Date(machine.installationDate), 'MMM d, yyyy')}
                    </div>
                  )}
                  {machine.location && (
                    <div>
                      <span className="font-medium">Location:</span> {machine.location}
                    </div>
                  )}
                  <div>
                    <span className="font-medium">Nozzles:</span> {machine.nozzleCount || 0}
                  </div>
                </div>

                <div className="flex gap-2 pt-4 border-t border-gray-200">
                  <button
                    onClick={() => handleEdit(machine)}
                    className="flex-1 bg-blue-50 hover:bg-blue-100 text-blue-700 px-3 py-2 rounded-md text-sm font-medium"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(machine.machineId)}
                    className="flex-1 bg-red-50 hover:bg-red-100 text-red-700 px-3 py-2 rounded-md text-sm font-medium"
                  >
                    Delete
                  </button>
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
