'use client';

import { useState, useEffect } from 'react';
import { api } from '@/lib/api';
import { useAuth } from '@/context/AuthContext';
import type { FuelSale, PaymentMethod } from '@/types';

interface SalesListProps {
  shiftId: string;
  refreshTrigger?: number;
  shiftStatus?: number; // 0: Pending, 1: Active, 2: Closed
}

export default function SalesList({ shiftId, refreshTrigger, shiftStatus }: SalesListProps) {
  const { user } = useAuth();
  const [sales, setSales] = useState<FuelSale[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [voidingId, setVoidingId] = useState<string | null>(null);
  const [showVoidModal, setShowVoidModal] = useState(false);
  const [saleToVoid, setSaleToVoid] = useState<FuelSale | null>(null);
  const [voidReason, setVoidReason] = useState('');

  const canVoid = user?.role === 'Manager' || user?.role === 'Owner';
  const isShiftClosed = shiftStatus === 2;

  useEffect(() => {
    loadSales();
  }, [shiftId, refreshTrigger]);

  const loadSales = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await api.getFuelSalesByShift(shiftId);
      if (response.success && response.data) {
        setSales(response.data);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load sales');
    } finally {
      setLoading(false);
    }
  };

  const handleVoidClick = (sale: FuelSale) => {
    setSaleToVoid(sale);
    setVoidReason('');
    setShowVoidModal(true);
  };

  const handleVoidSubmit = async () => {
    if (!saleToVoid || !voidReason.trim()) return;

    try {
      setVoidingId(saleToVoid.fuelSaleId);
      const response = await api.voidFuelSale(saleToVoid.fuelSaleId, voidReason.trim());
      if (response.success) {
        // Update the sale in the list
        setSales(sales.map(s =>
          s.fuelSaleId === saleToVoid.fuelSaleId ? { ...s, isVoided: true, voidReason: voidReason.trim() } : s
        ));
        setShowVoidModal(false);
        setSaleToVoid(null);
        setVoidReason('');
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to void sale');
    } finally {
      setVoidingId(null);
    }
  };

  const getPaymentMethodLabel = (method: PaymentMethod): string => {
    const labels = ['Cash', 'Credit', 'Digital', 'Mixed'];
    return labels[method];
  };

  const getPaymentMethodBadge = (method: PaymentMethod): string => {
    const colors = [
      'bg-green-100 text-green-800', // Cash
      'bg-yellow-100 text-yellow-800', // Credit
      'bg-blue-100 text-blue-800', // Digital
      'bg-purple-100 text-purple-800', // Mixed
    ];
    return colors[method];
  };

  // Calculate summary (excluding voided sales)
  const activeSales = sales.filter(s => !s.isVoided);
  const voidedSales = sales.filter(s => s.isVoided);

  const summary = activeSales.reduce(
    (acc, sale) => {
      acc.totalQuantity += sale.quantity;
      acc.totalAmount += sale.amount;

      switch (sale.paymentMethod) {
        case 0:
          acc.cash += sale.amount;
          break;
        case 1:
          acc.credit += sale.amount;
          break;
        case 2:
          acc.digital += sale.amount;
          break;
        case 3:
          acc.mixed += sale.amount;
          break;
      }

      return acc;
    },
    {
      totalQuantity: 0,
      totalAmount: 0,
      cash: 0,
      credit: 0,
      digital: 0,
      mixed: 0,
    }
  );

  if (loading) {
    return (
      <div className="bg-white rounded-lg shadow p-6">
        <div className="text-gray-500">Loading sales...</div>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* Summary Cards */}
      <div className="grid grid-cols-4 gap-4">
        <div className="bg-blue-50 p-4 rounded-lg">
          <p className="text-sm text-gray-600">Active Sales</p>
          <p className="text-2xl font-bold text-blue-600">{activeSales.length}</p>
          {voidedSales.length > 0 && (
            <p className="text-xs text-red-500">{voidedSales.length} voided</p>
          )}
        </div>
        <div className="bg-green-50 p-4 rounded-lg">
          <p className="text-sm text-gray-600">Total Quantity</p>
          <p className="text-xl font-bold text-green-600">{summary.totalQuantity.toFixed(2)} L</p>
        </div>
        <div className="bg-purple-50 p-4 rounded-lg">
          <p className="text-sm text-gray-600">Total Amount</p>
          <p className="text-2xl font-bold text-purple-600">₹{summary.totalAmount.toFixed(2)}</p>
        </div>
        <div className="bg-gray-50 p-4 rounded-lg">
          <p className="text-xs text-gray-600 mb-1">Payment Breakdown</p>
          <div className="space-y-1 text-xs">
            {summary.cash > 0 && <p>Cash: ₹{summary.cash.toFixed(2)}</p>}
            {summary.credit > 0 && <p>Credit: ₹{summary.credit.toFixed(2)}</p>}
            {summary.digital > 0 && <p>Digital: ₹{summary.digital.toFixed(2)}</p>}
            {summary.mixed > 0 && <p>Mixed: ₹{summary.mixed.toFixed(2)}</p>}
            {summary.totalAmount === 0 && <p className="text-gray-400">No sales yet</p>}
          </div>
        </div>
      </div>

      {/* Sales Table */}
      <div className="bg-white rounded-lg shadow">
        <div className="p-4 border-b">
          <h3 className="font-semibold">Individual Sales</h3>
        </div>

        {error && (
          <div className="p-4 bg-red-50 border-b border-red-200 text-red-700 text-sm">
            {error}
          </div>
        )}

        {sales.length === 0 ? (
          <div className="p-8 text-center text-gray-500">No sales recorded yet</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Bill No.
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Time
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Nozzle
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Fuel
                  </th>
                  <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Quantity
                  </th>
                  <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Rate
                  </th>
                  <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    Amount
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Payment
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Status
                  </th>
                  {canVoid && !isShiftClosed && (
                    <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      Actions
                    </th>
                  )}
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {sales.map((sale) => (
                  <tr
                    key={sale.fuelSaleId}
                    className={`hover:bg-gray-50 ${sale.isVoided ? 'bg-red-50 opacity-60' : ''}`}
                  >
                    <td className={`px-4 py-3 text-sm font-mono ${sale.isVoided ? 'line-through text-gray-400' : 'text-blue-600'}`}>
                      {sale.saleNumber}
                    </td>
                    <td className={`px-4 py-3 text-sm ${sale.isVoided ? 'line-through text-gray-400' : 'text-gray-500'}`}>
                      {new Date(sale.saleTime).toLocaleTimeString()}
                    </td>
                    <td className={`px-4 py-3 text-sm ${sale.isVoided ? 'line-through text-gray-400' : 'font-medium'}`}>
                      {sale.nozzleNumber}
                    </td>
                    <td className={`px-4 py-3 text-sm ${sale.isVoided ? 'line-through text-gray-400' : ''}`}>
                      {sale.fuelName}
                    </td>
                    <td className={`px-4 py-3 text-sm text-right ${sale.isVoided ? 'line-through text-gray-400' : ''}`}>
                      {sale.quantity.toFixed(2)}
                    </td>
                    <td className={`px-4 py-3 text-sm text-right ${sale.isVoided ? 'line-through text-gray-400' : ''}`}>
                      ₹{sale.rate.toFixed(2)}
                    </td>
                    <td className={`px-4 py-3 text-sm text-right ${sale.isVoided ? 'line-through text-gray-400' : 'font-bold'}`}>
                      ₹{sale.amount.toFixed(2)}
                    </td>
                    <td className="px-4 py-3 text-sm">
                      <span
                        className={`px-2 py-1 rounded text-xs font-medium ${
                          sale.isVoided ? 'bg-gray-100 text-gray-500' : getPaymentMethodBadge(sale.paymentMethod)
                        }`}
                      >
                        {getPaymentMethodLabel(sale.paymentMethod)}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-sm">
                      {sale.isVoided ? (
                        <span
                          className="px-2 py-1 rounded text-xs font-medium bg-red-100 text-red-800 cursor-help"
                          title={`Voided: ${sale.voidReason || 'No reason'}`}
                        >
                          VOIDED
                        </span>
                      ) : (
                        <span className="px-2 py-1 rounded text-xs font-medium bg-green-100 text-green-800">
                          Active
                        </span>
                      )}
                    </td>
                    {canVoid && !isShiftClosed && (
                      <td className="px-4 py-3 text-sm text-right">
                        {!sale.isVoided && (
                          <button
                            onClick={() => handleVoidClick(sale)}
                            disabled={voidingId === sale.fuelSaleId}
                            className="text-red-600 hover:text-red-800 text-xs font-medium disabled:opacity-50"
                          >
                            {voidingId === sale.fuelSaleId ? 'Voiding...' : 'Void'}
                          </button>
                        )}
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Void Modal */}
      {showVoidModal && saleToVoid && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h3 className="text-lg font-semibold mb-4">Void Sale</h3>
            <div className="mb-4 p-3 bg-gray-50 rounded">
              <p className="text-sm text-gray-600">Sale: <span className="font-mono">{saleToVoid.saleNumber}</span></p>
              <p className="text-sm text-gray-600">Amount: <span className="font-bold">₹{saleToVoid.amount.toFixed(2)}</span></p>
            </div>
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Reason for voiding *
              </label>
              <textarea
                value={voidReason}
                onChange={(e) => setVoidReason(e.target.value)}
                className="w-full border rounded-lg p-2 text-sm"
                rows={3}
                placeholder="Enter reason for voiding this sale..."
              />
            </div>
            <div className="flex justify-end gap-3">
              <button
                onClick={() => {
                  setShowVoidModal(false);
                  setSaleToVoid(null);
                  setVoidReason('');
                }}
                className="px-4 py-2 text-gray-600 hover:text-gray-800"
              >
                Cancel
              </button>
              <button
                onClick={handleVoidSubmit}
                disabled={!voidReason.trim() || voidingId !== null}
                className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 disabled:opacity-50"
              >
                {voidingId ? 'Voiding...' : 'Void Sale'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
