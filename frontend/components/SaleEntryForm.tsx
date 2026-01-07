'use client';

import { useState, useEffect } from 'react';
import { api } from '@/lib/api';
import type { CreateFuelSaleDto, PaymentMethod, Shift } from '@/types';

interface SaleEntryFormProps {
  shift: Shift;
  onSaleCreated?: () => void;
}

export default function SaleEntryForm({ shift, onSaleCreated }: SaleEntryFormProps) {
  // Form state
  const [formData, setFormData] = useState<CreateFuelSaleDto>({
    nozzleId: '',
    quantity: 0,
    paymentMethod: 0, // Cash default
    customerName: '',
    customerPhone: '',
    vehicleNumber: '',
    notes: '',
  });

  // UI state
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [calculatedAmount, setCalculatedAmount] = useState(0);

  // Calculate amount when nozzle or quantity changes
  useEffect(() => {
    if (formData.nozzleId && formData.quantity > 0) {
      const nozzleReading = shift.nozzleReadings.find(nr => nr.nozzleId === formData.nozzleId);
      if (nozzleReading) {
        setCalculatedAmount(formData.quantity * nozzleReading.rateAtShift);
      }
    } else {
      setCalculatedAmount(0);
    }
  }, [formData.nozzleId, formData.quantity, shift.nozzleReadings]);

  // Handle form submission
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!formData.nozzleId) {
      setError('Please select a nozzle');
      return;
    }

    if (formData.quantity <= 0) {
      setError('Quantity must be greater than zero');
      return;
    }

    try {
      setLoading(true);
      const response = await api.createFuelSale(formData);

      if (response.success) {
        // Reset form
        setFormData({
          nozzleId: formData.nozzleId, // Keep nozzle selected
          quantity: 0,
          paymentMethod: 0,
          customerName: '',
          customerPhone: '',
          vehicleNumber: '',
          notes: '',
        });

        if (onSaleCreated) onSaleCreated();
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to record sale');
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow p-6 space-y-4">
      <h3 className="text-lg font-semibold">Record Fuel Sale</h3>

      {error && (
        <div className="p-3 bg-red-50 border border-red-200 text-red-700 rounded text-sm">
          {error}
        </div>
      )}

      {/* Nozzle Selection */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Nozzle *
        </label>
        <select
          value={formData.nozzleId}
          onChange={(e) => setFormData(prev => ({ ...prev, nozzleId: e.target.value }))}
          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          required
        >
          <option value="">-- Select Nozzle --</option>
          {shift.nozzleReadings.map((nr) => (
            <option key={nr.nozzleId} value={nr.nozzleId}>
              {nr.nozzleNumber} - {nr.fuelName} ({nr.machineName}) - ₹{nr.rateAtShift.toFixed(2)}/L
            </option>
          ))}
        </select>
      </div>

      {/* Quantity */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Quantity (Liters) *
        </label>
        <input
          type="number"
          step="0.01"
          min="0"
          value={formData.quantity || ''}
          onChange={(e) => setFormData(prev => ({ ...prev, quantity: parseFloat(e.target.value) || 0 }))}
          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          required
        />
      </div>

      {/* Calculated Amount Display */}
      {calculatedAmount > 0 && (
        <div className="p-3 bg-blue-50 rounded-lg">
          <p className="text-sm text-gray-600">Calculated Amount</p>
          <p className="text-2xl font-bold text-blue-600">₹{calculatedAmount.toFixed(2)}</p>
        </div>
      )}

      {/* Payment Method */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Payment Method *
        </label>
        <select
          value={formData.paymentMethod}
          onChange={(e) => setFormData(prev => ({ ...prev, paymentMethod: parseInt(e.target.value) as PaymentMethod }))}
          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          required
        >
          <option value="0">Cash</option>
          <option value="1">Credit</option>
          <option value="2">Digital (UPI/Card)</option>
          <option value="3">Mixed</option>
        </select>
      </div>

      {/* Optional Customer Details */}
      <div className="border-t pt-4">
        <p className="text-sm font-medium text-gray-700 mb-3">Customer Details (Optional)</p>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-xs text-gray-600 mb-1">Customer Name</label>
            <input
              type="text"
              value={formData.customerName || ''}
              onChange={(e) => setFormData(prev => ({ ...prev, customerName: e.target.value }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              maxLength={200}
            />
          </div>

          <div>
            <label className="block text-xs text-gray-600 mb-1">Phone Number</label>
            <input
              type="tel"
              value={formData.customerPhone || ''}
              onChange={(e) => setFormData(prev => ({ ...prev, customerPhone: e.target.value }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              maxLength={20}
            />
          </div>
        </div>

        <div className="mt-3">
          <label className="block text-xs text-gray-600 mb-1">Vehicle Number</label>
          <input
            type="text"
            value={formData.vehicleNumber || ''}
            onChange={(e) => setFormData(prev => ({ ...prev, vehicleNumber: e.target.value.toUpperCase() }))}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            placeholder="MH01AB1234"
            maxLength={50}
          />
        </div>
      </div>

      {/* Notes */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Notes (Optional)</label>
        <textarea
          value={formData.notes || ''}
          onChange={(e) => setFormData(prev => ({ ...prev, notes: e.target.value }))}
          rows={2}
          className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
        />
      </div>

      {/* Submit Button */}
      <button
        type="submit"
        disabled={loading || !formData.nozzleId || formData.quantity <= 0}
        className="w-full py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 font-medium disabled:bg-gray-300 disabled:cursor-not-allowed"
      >
        {loading ? 'Recording...' : 'Record Sale'}
      </button>
    </form>
  );
}
