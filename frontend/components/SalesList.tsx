'use client';

import { useState, useEffect } from 'react';
import { api } from '@/lib/api';
import type { FuelSale, PaymentMethod } from '@/types';

interface SalesListProps {
  shiftId: string;
  refreshTrigger?: number;
}

export default function SalesList({ shiftId, refreshTrigger }: SalesListProps) {
  const [sales, setSales] = useState<FuelSale[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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

  // Calculate summary
  const summary = sales.reduce(
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
          <p className="text-sm text-gray-600">Total Sales</p>
          <p className="text-2xl font-bold text-blue-600">{sales.length}</p>
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
                    Customer
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {sales.map((sale) => (
                  <tr key={sale.fuelSaleId} className="hover:bg-gray-50">
                    <td className="px-4 py-3 text-sm text-gray-500">
                      {new Date(sale.saleTime).toLocaleTimeString()}
                    </td>
                    <td className="px-4 py-3 text-sm font-medium">{sale.nozzleNumber}</td>
                    <td className="px-4 py-3 text-sm">{sale.fuelName}</td>
                    <td className="px-4 py-3 text-sm text-right">{sale.quantity.toFixed(2)}</td>
                    <td className="px-4 py-3 text-sm text-right">₹{sale.rate.toFixed(2)}</td>
                    <td className="px-4 py-3 text-sm text-right font-bold">
                      ₹{sale.amount.toFixed(2)}
                    </td>
                    <td className="px-4 py-3 text-sm">
                      <span
                        className={`px-2 py-1 rounded text-xs font-medium ${getPaymentMethodBadge(
                          sale.paymentMethod
                        )}`}
                      >
                        {getPaymentMethodLabel(sale.paymentMethod)}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-sm">
                      {sale.customerName || sale.vehicleNumber || '-'}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
