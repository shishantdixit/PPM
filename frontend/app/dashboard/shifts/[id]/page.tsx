'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { api } from '@/lib/api';
import type { Shift, ShiftStatus } from '@/types';
import SalesList from '@/components/SalesList';

export default function ShiftDetailsPage() {
  const params = useParams();
  const router = useRouter();
  const [shift, setShift] = useState<Shift | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (params.id) {
      loadShift(params.id as string);
    }
  }, [params.id]);

  const loadShift = async (id: string) => {
    try {
      setLoading(true);
      setError(null);

      const response = await api.getShift(id);
      if (response.success && response.data) {
        setShift(response.data);
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load shift');
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (status: ShiftStatus) => {
    const styles = {
      0: 'bg-gray-100 text-gray-800',
      1: 'bg-green-100 text-green-800',
      2: 'bg-blue-100 text-blue-800',
    };
    const labels = ['Pending', 'Active', 'Closed'];
    return (
      <span className={`px-3 py-1 rounded text-sm font-medium ${styles[status]}`}>
        {labels[status]}
      </span>
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-gray-500">Loading shift details...</div>
      </div>
    );
  }

  if (error || !shift) {
    return (
      <div className="space-y-6">
        <button
          onClick={() => router.push('/dashboard/shifts/all')}
          className="text-blue-600 hover:text-blue-800"
        >
          ← Back to All Shifts
        </button>
        <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-lg">
          {error || 'Shift not found'}
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <button
          onClick={() => router.push('/dashboard/shifts/all')}
          className="text-blue-600 hover:text-blue-800"
        >
          ← Back to All Shifts
        </button>
        {shift.status === 1 && (
          <button
            onClick={() => router.push('/dashboard/shifts/all')}
            className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
          >
            Close Shift
          </button>
        )}
      </div>

      <div className="bg-white rounded-lg shadow p-6 space-y-6">
        <div className="flex items-center justify-between">
          <h1 className="text-2xl font-bold">Shift Details</h1>
          {getStatusBadge(shift.status)}
        </div>

        {/* Shift Info */}
        <div className="grid grid-cols-2 gap-6">
          <div>
            <h3 className="text-sm font-medium text-gray-500 mb-1">Worker</h3>
            <p className="text-lg">{shift.workerName}</p>
          </div>
          <div>
            <h3 className="text-sm font-medium text-gray-500 mb-1">Shift Date</h3>
            <p className="text-lg">{shift.shiftDate}</p>
          </div>
          <div>
            <h3 className="text-sm font-medium text-gray-500 mb-1">Start Time</h3>
            <p className="text-lg">{shift.startTime}</p>
          </div>
          <div>
            <h3 className="text-sm font-medium text-gray-500 mb-1">End Time</h3>
            <p className="text-lg">{shift.endTime || 'Ongoing'}</p>
          </div>
        </div>

        {/* Settlement Summary */}
        {shift.status === 2 && (
          <div className="border-t pt-6">
            <h3 className="text-lg font-semibold mb-4">Settlement Summary</h3>
            <div className="grid grid-cols-3 gap-4">
              <div className="bg-blue-50 p-4 rounded-lg">
                <p className="text-sm text-gray-600">Total Sales</p>
                <p className="text-2xl font-bold text-blue-600">₹{shift.totalSales.toFixed(2)}</p>
              </div>
              <div className="bg-green-50 p-4 rounded-lg">
                <p className="text-sm text-gray-600">Cash Collected</p>
                <p className="text-xl font-bold text-green-600">₹{shift.cashCollected.toFixed(2)}</p>
              </div>
              <div className="bg-purple-50 p-4 rounded-lg">
                <p className="text-sm text-gray-600">Credit Sales</p>
                <p className="text-xl font-bold text-purple-600">₹{shift.creditSales.toFixed(2)}</p>
              </div>
              <div className="bg-indigo-50 p-4 rounded-lg">
                <p className="text-sm text-gray-600">Digital Payments</p>
                <p className="text-xl font-bold text-indigo-600">₹{shift.digitalPayments.toFixed(2)}</p>
              </div>
              <div className="bg-yellow-50 p-4 rounded-lg">
                <p className="text-sm text-gray-600">Borrowing</p>
                <p className="text-xl font-bold text-yellow-600">₹{shift.borrowing.toFixed(2)}</p>
              </div>
              <div className={`p-4 rounded-lg ${shift.variance < 0 ? 'bg-red-50' : 'bg-green-50'}`}>
                <p className="text-sm text-gray-600">Variance</p>
                <p className={`text-2xl font-bold ${shift.variance < 0 ? 'text-red-600' : 'text-green-600'}`}>
                  ₹{shift.variance.toFixed(2)}
                </p>
              </div>
            </div>
          </div>
        )}

        {/* Nozzle Readings */}
        <div className="border-t pt-6">
          <h3 className="text-lg font-semibold mb-4">Nozzle Readings</h3>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Nozzle</th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Fuel</th>
                  <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Opening</th>
                  <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Closing</th>
                  <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Quantity</th>
                  <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Rate</th>
                  <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Amount</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {shift.nozzleReadings.map((reading) => (
                  <tr key={reading.shiftNozzleReadingId}>
                    <td className="px-4 py-3 text-sm">
                      <div>
                        <p className="font-medium">{reading.nozzleNumber}</p>
                        <p className="text-xs text-gray-500">{reading.machineName}</p>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-sm">
                      <div>
                        <p className="font-medium">{reading.fuelName}</p>
                        <p className="text-xs text-gray-500">{reading.fuelCode}</p>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-sm text-right">{reading.openingReading.toFixed(2)}</td>
                    <td className="px-4 py-3 text-sm text-right">
                      {reading.closingReading?.toFixed(2) || '-'}
                    </td>
                    <td className="px-4 py-3 text-sm text-right font-medium">
                      {reading.quantitySold.toFixed(2)}
                    </td>
                    <td className="px-4 py-3 text-sm text-right">₹{reading.rateAtShift.toFixed(2)}</td>
                    <td className="px-4 py-3 text-sm text-right font-bold">
                      ₹{reading.expectedAmount.toFixed(2)}
                    </td>
                  </tr>
                ))}
                <tr className="bg-gray-50 font-bold">
                  <td colSpan={6} className="px-4 py-3 text-sm text-right">Total:</td>
                  <td className="px-4 py-3 text-sm text-right">
                    ₹{shift.nozzleReadings.reduce((sum, r) => sum + r.expectedAmount, 0).toFixed(2)}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        {/* Individual Sales Section */}
        {shift.fuelSales && shift.fuelSales.length > 0 && (
          <div className="border-t pt-6">
            <h3 className="text-lg font-semibold mb-4">Individual Sales</h3>
            <SalesList shiftId={shift.shiftId} />
          </div>
        )}

        {/* Notes */}
        {shift.notes && (
          <div className="border-t pt-6">
            <h3 className="text-lg font-semibold mb-2">Notes</h3>
            <p className="text-gray-700">{shift.notes}</p>
          </div>
        )}

        {/* Metadata */}
        <div className="border-t pt-6 text-sm text-gray-500">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <p>Created: {new Date(shift.createdAt).toLocaleString()}</p>
            </div>
            <div>
              <p>Last Updated: {new Date(shift.updatedAt).toLocaleString()}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
