'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { api } from '@/lib/api';
import type {
  CreditCustomer,
  CreditTransaction,
  UpdateCreditCustomerDto,
  PaymentModeCredit,
  CreditTransactionType
} from '@/types';

export default function CreditCustomerDetailPage() {
  const params = useParams();
  const router = useRouter();
  const customerId = params.id as string;

  const [customer, setCustomer] = useState<CreditCustomer | null>(null);
  const [transactions, setTransactions] = useState<CreditTransaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'details' | 'transactions' | 'statement'>('details');
  const [showEditModal, setShowEditModal] = useState(false);
  const [showPaymentModal, setShowPaymentModal] = useState(false);
  const [showAdjustModal, setShowAdjustModal] = useState(false);
  const [exporting, setExporting] = useState(false);
  const [showExportMenu, setShowExportMenu] = useState(false);

  useEffect(() => {
    fetchCustomer();
    fetchTransactions();
  }, [customerId]);

  const fetchCustomer = async () => {
    try {
      const res = await api.getCreditCustomer(customerId);
      if (res.success && res.data) {
        setCustomer(res.data);
      } else {
        setError(res.message || 'Customer not found');
      }
    } catch (err: any) {
      setError(err.message || 'Failed to fetch customer');
    } finally {
      setLoading(false);
    }
  };

  const fetchTransactions = async () => {
    try {
      const res = await api.getCreditTransactions(customerId);
      if (res.success && res.data) {
        setTransactions(res.data);
      }
    } catch (err: any) {
      console.error('Failed to fetch transactions:', err);
    }
  };

  const handleBlockUnblock = async () => {
    if (!customer) return;

    try {
      if (customer.isBlocked) {
        await api.unblockCreditCustomer(customer.creditCustomerId);
      } else {
        const reason = prompt('Enter reason for blocking:');
        if (!reason) return;
        await api.blockCreditCustomer(customer.creditCustomerId, reason);
      }
      fetchCustomer();
    } catch (err: any) {
      alert(err.message || 'Failed to update status');
    }
  };

  const handleDelete = async () => {
    if (!customer) return;
    if (!confirm('Are you sure you want to delete this customer?')) return;

    try {
      const res = await api.deleteCreditCustomer(customer.creditCustomerId);
      if (res.success) {
        router.push('/dashboard/credit-customers');
      } else {
        alert(res.message || 'Failed to delete customer');
      }
    } catch (err: any) {
      alert(err.message || 'Failed to delete customer');
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 2
    }).format(value);
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('en-IN', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const getTransactionTypeColor = (type: CreditTransactionType) => {
    switch (type) {
      case CreditTransactionType.Sale: return 'text-red-600';
      case CreditTransactionType.Payment: return 'text-green-600';
      case CreditTransactionType.Adjustment: return 'text-orange-600';
      case CreditTransactionType.Refund: return 'text-blue-600';
      default: return 'text-gray-600';
    }
  };

  const handleExportStatement = async (format: string) => {
    setExporting(true);
    setShowExportMenu(false);
    try {
      await api.exportCustomerStatement(customerId, undefined, undefined, format);
    } catch (err: any) {
      alert(err.message || 'Failed to export statement');
    } finally {
      setExporting(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg">Loading...</div>
      </div>
    );
  }

  if (error || !customer) {
    return (
      <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
        {error || 'Customer not found'}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-start">
        <div>
          <Link
            href="/dashboard/credit-customers"
            className="text-blue-600 hover:text-blue-800 text-sm mb-2 inline-block"
          >
            &larr; Back to Customers
          </Link>
          <h1 className="text-2xl font-bold text-gray-900">{customer.customerName}</h1>
          <p className="text-gray-500">{customer.customerCode}</p>
        </div>
        <div className="flex gap-2">
          <div className="relative">
            <button
              onClick={() => setShowExportMenu(!showExportMenu)}
              disabled={exporting}
              className="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 disabled:opacity-50 flex items-center gap-2"
            >
              {exporting ? 'Exporting...' : 'Statement'}
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
              </svg>
            </button>
            {showExportMenu && (
              <div className="absolute right-0 mt-2 w-40 bg-white rounded-lg shadow-lg border z-10">
                <button
                  onClick={() => handleExportStatement('pdf')}
                  className="w-full px-4 py-2 text-left hover:bg-gray-100 rounded-t-lg"
                >
                  PDF
                </button>
                <button
                  onClick={() => handleExportStatement('excel')}
                  className="w-full px-4 py-2 text-left hover:bg-gray-100"
                >
                  Excel
                </button>
                <button
                  onClick={() => handleExportStatement('csv')}
                  className="w-full px-4 py-2 text-left hover:bg-gray-100 rounded-b-lg"
                >
                  CSV
                </button>
              </div>
            )}
          </div>
          <button
            onClick={() => setShowPaymentModal(true)}
            className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700"
          >
            Record Payment
          </button>
          <button
            onClick={() => setShowEditModal(true)}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
          >
            Edit
          </button>
          <button
            onClick={handleBlockUnblock}
            className={`px-4 py-2 rounded-lg ${
              customer.isBlocked
                ? 'bg-green-100 text-green-700 hover:bg-green-200'
                : 'bg-red-100 text-red-700 hover:bg-red-200'
            }`}
          >
            {customer.isBlocked ? 'Unblock' : 'Block'}
          </button>
        </div>
      </div>

      {/* Status Banner */}
      {customer.isBlocked && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
          <strong>Blocked:</strong> {customer.blockReason || 'No reason provided'}
        </div>
      )}

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-white rounded-lg shadow p-4">
          <div className="text-sm text-gray-500">Credit Limit</div>
          <div className="text-2xl font-bold text-blue-600">{formatCurrency(customer.creditLimit)}</div>
        </div>
        <div className="bg-white rounded-lg shadow p-4">
          <div className="text-sm text-gray-500">Current Balance</div>
          <div className={`text-2xl font-bold ${customer.currentBalance > 0 ? 'text-red-600' : 'text-green-600'}`}>
            {formatCurrency(customer.currentBalance)}
          </div>
        </div>
        <div className="bg-white rounded-lg shadow p-4">
          <div className="text-sm text-gray-500">Available Credit</div>
          <div className="text-2xl font-bold text-green-600">{formatCurrency(customer.availableCredit)}</div>
        </div>
        <div className="bg-white rounded-lg shadow p-4">
          <div className="text-sm text-gray-500">Payment Terms</div>
          <div className="text-2xl font-bold text-gray-900">{customer.paymentTermDays} days</div>
        </div>
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200">
        <nav className="-mb-px flex space-x-8">
          <button
            onClick={() => setActiveTab('details')}
            className={`py-4 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'details'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Details
          </button>
          <button
            onClick={() => setActiveTab('transactions')}
            className={`py-4 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'transactions'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Transactions ({transactions.length})
          </button>
        </nav>
      </div>

      {/* Tab Content */}
      {activeTab === 'details' && (
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Customer Information</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-500">Contact Person</label>
              <p className="mt-1 text-gray-900">{customer.contactPerson || '-'}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500">Phone</label>
              <p className="mt-1 text-gray-900">{customer.phone}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500">Email</label>
              <p className="mt-1 text-gray-900">{customer.email || '-'}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500">Address</label>
              <p className="mt-1 text-gray-900">{customer.address || '-'}</p>
            </div>
            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-500">Vehicle Numbers</label>
              <p className="mt-1 text-gray-900">{customer.vehicleNumbers || '-'}</p>
            </div>
            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-500">Notes</label>
              <p className="mt-1 text-gray-900">{customer.notes || '-'}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500">Created At</label>
              <p className="mt-1 text-gray-900">{formatDate(customer.createdAt)}</p>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500">Last Updated</label>
              <p className="mt-1 text-gray-900">{formatDate(customer.updatedAt)}</p>
            </div>
          </div>

          <div className="mt-6 pt-6 border-t border-gray-200 flex gap-3">
            <button
              onClick={() => setShowAdjustModal(true)}
              className="px-4 py-2 bg-orange-100 text-orange-700 rounded-lg hover:bg-orange-200"
            >
              Adjust Balance
            </button>
            <button
              onClick={handleDelete}
              className="px-4 py-2 bg-red-100 text-red-700 rounded-lg hover:bg-red-200"
            >
              Delete Customer
            </button>
          </div>
        </div>
      )}

      {activeTab === 'transactions' && (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Details</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Amount</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Balance</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {transactions.map((tx) => (
                <tr key={tx.creditTransactionId} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {formatDate(tx.transactionDate)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`text-sm font-medium ${getTransactionTypeColor(tx.transactionType)}`}>
                      {tx.transactionTypeName}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {tx.saleNumber && <div>Sale: {tx.saleNumber}</div>}
                    {tx.paymentModeName && <div>Mode: {tx.paymentModeName}</div>}
                    {tx.paymentReference && <div>Ref: {tx.paymentReference}</div>}
                    {tx.notes && <div className="text-gray-400">{tx.notes}</div>}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right">
                    <span className={`text-sm font-medium ${
                      tx.transactionType === CreditTransactionType.Sale ? 'text-red-600' : 'text-green-600'
                    }`}>
                      {tx.transactionType === CreditTransactionType.Sale ? '+' : '-'}
                      {formatCurrency(tx.amount)}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm text-gray-900">
                    {formatCurrency(tx.balanceAfter)}
                  </td>
                </tr>
              ))}
              {transactions.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-6 py-8 text-center text-gray-500">
                    No transactions yet
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* Edit Modal */}
      {showEditModal && (
        <EditCustomerModal
          customer={customer}
          onClose={() => setShowEditModal(false)}
          onSuccess={() => {
            setShowEditModal(false);
            fetchCustomer();
          }}
        />
      )}

      {/* Payment Modal */}
      {showPaymentModal && (
        <RecordPaymentModal
          customer={customer}
          onClose={() => setShowPaymentModal(false)}
          onSuccess={() => {
            setShowPaymentModal(false);
            fetchCustomer();
            fetchTransactions();
          }}
        />
      )}

      {/* Adjust Balance Modal */}
      {showAdjustModal && (
        <AdjustBalanceModal
          customer={customer}
          onClose={() => setShowAdjustModal(false)}
          onSuccess={() => {
            setShowAdjustModal(false);
            fetchCustomer();
            fetchTransactions();
          }}
        />
      )}
    </div>
  );
}

function EditCustomerModal({
  customer,
  onClose,
  onSuccess
}: {
  customer: CreditCustomer;
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [formData, setFormData] = useState<UpdateCreditCustomerDto>({
    customerName: customer.customerName,
    contactPerson: customer.contactPerson || undefined,
    phone: customer.phone,
    email: customer.email || undefined,
    address: customer.address || undefined,
    creditLimit: customer.creditLimit,
    paymentTermDays: customer.paymentTermDays,
    vehicleNumbers: customer.vehicleNumbers || undefined,
    notes: customer.notes || undefined
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const res = await api.updateCreditCustomer(customer.creditCustomerId, formData);
      if (res.success) {
        onSuccess();
      } else {
        setError(res.message || 'Failed to update customer');
      }
    } catch (err: any) {
      setError(err.message || 'Failed to update customer');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-lg w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <h2 className="text-xl font-bold text-gray-900 mb-4">Edit Customer</h2>

          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Customer Name *</label>
              <input
                type="text"
                required
                value={formData.customerName || ''}
                onChange={(e) => setFormData({ ...formData, customerName: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Phone *</label>
                <input
                  type="tel"
                  required
                  value={formData.phone || ''}
                  onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
                <input
                  type="email"
                  value={formData.email || ''}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Contact Person</label>
              <input
                type="text"
                value={formData.contactPerson || ''}
                onChange={(e) => setFormData({ ...formData, contactPerson: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Address</label>
              <textarea
                value={formData.address || ''}
                onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                rows={2}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Credit Limit</label>
                <input
                  type="number"
                  min="0"
                  step="100"
                  value={formData.creditLimit || 0}
                  onChange={(e) => setFormData({ ...formData, creditLimit: parseFloat(e.target.value) })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Payment Terms (Days)</label>
                <input
                  type="number"
                  min="1"
                  value={formData.paymentTermDays || 30}
                  onChange={(e) => setFormData({ ...formData, paymentTermDays: parseInt(e.target.value) })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Vehicle Numbers</label>
              <input
                type="text"
                value={formData.vehicleNumbers || ''}
                onChange={(e) => setFormData({ ...formData, vehicleNumbers: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                placeholder="e.g., MH01AB1234, MH02CD5678"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Notes</label>
              <textarea
                value={formData.notes || ''}
                onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                rows={2}
              />
            </div>

            <div className="flex justify-end gap-3 pt-4">
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={loading}
                className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
              >
                {loading ? 'Saving...' : 'Save Changes'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

function RecordPaymentModal({
  customer,
  onClose,
  onSuccess
}: {
  customer: CreditCustomer;
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [amount, setAmount] = useState('');
  const [paymentMode, setPaymentMode] = useState<PaymentModeCredit>(0);
  const [paymentReference, setPaymentReference] = useState('');
  const [notes, setNotes] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const paymentModeOptions = [
    { value: 0, label: 'Cash' },
    { value: 1, label: 'Cheque' },
    { value: 2, label: 'Bank Transfer' },
    { value: 3, label: 'UPI' },
    { value: 4, label: 'Card' },
    { value: 5, label: 'Other' }
  ];

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const res = await api.recordPayment(customer.creditCustomerId, {
        amount: parseFloat(amount),
        paymentMode,
        paymentReference: paymentReference || undefined,
        notes: notes || undefined
      });

      if (res.success) {
        onSuccess();
      } else {
        setError(res.message || 'Failed to record payment');
      }
    } catch (err: any) {
      setError(err.message || 'Failed to record payment');
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 2
    }).format(value);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
        <div className="p-6">
          <h2 className="text-xl font-bold text-gray-900 mb-4">Record Payment</h2>

          <div className="bg-gray-50 rounded-lg p-4 mb-4">
            <div className="font-medium text-gray-900">{customer.customerName}</div>
            <div className="text-sm text-gray-500">{customer.customerCode}</div>
            <div className="mt-2 text-sm">
              <span className="text-gray-500">Current Balance: </span>
              <span className="font-medium text-red-600">{formatCurrency(customer.currentBalance)}</span>
            </div>
          </div>

          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Payment Amount *</label>
              <input
                type="number"
                required
                min="0.01"
                step="0.01"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Payment Mode *</label>
              <select
                value={paymentMode}
                onChange={(e) => setPaymentMode(parseInt(e.target.value) as PaymentModeCredit)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
              >
                {paymentModeOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Reference</label>
              <input
                type="text"
                value={paymentReference}
                onChange={(e) => setPaymentReference(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Notes</label>
              <textarea
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                rows={2}
              />
            </div>

            <div className="flex justify-end gap-3 pt-4">
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={loading}
                className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50"
              >
                {loading ? 'Recording...' : 'Record Payment'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

function AdjustBalanceModal({
  customer,
  onClose,
  onSuccess
}: {
  customer: CreditCustomer;
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [amount, setAmount] = useState('');
  const [adjustmentType, setAdjustmentType] = useState<'increase' | 'decrease'>('decrease');
  const [notes, setNotes] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const adjustedAmount = adjustmentType === 'increase'
        ? parseFloat(amount)
        : -parseFloat(amount);

      const res = await api.adjustBalance(customer.creditCustomerId, {
        amount: adjustedAmount,
        notes
      });

      if (res.success) {
        onSuccess();
      } else {
        setError(res.message || 'Failed to adjust balance');
      }
    } catch (err: any) {
      setError(err.message || 'Failed to adjust balance');
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 2
    }).format(value);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
        <div className="p-6">
          <h2 className="text-xl font-bold text-gray-900 mb-4">Adjust Balance</h2>

          <div className="bg-gray-50 rounded-lg p-4 mb-4">
            <div className="font-medium text-gray-900">{customer.customerName}</div>
            <div className="mt-2 text-sm">
              <span className="text-gray-500">Current Balance: </span>
              <span className="font-medium text-red-600">{formatCurrency(customer.currentBalance)}</span>
            </div>
          </div>

          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Adjustment Type *</label>
              <div className="flex gap-4">
                <label className="flex items-center">
                  <input
                    type="radio"
                    checked={adjustmentType === 'increase'}
                    onChange={() => setAdjustmentType('increase')}
                    className="mr-2"
                  />
                  Increase Balance (add charge)
                </label>
                <label className="flex items-center">
                  <input
                    type="radio"
                    checked={adjustmentType === 'decrease'}
                    onChange={() => setAdjustmentType('decrease')}
                    className="mr-2"
                  />
                  Decrease Balance (credit)
                </label>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Amount *</label>
              <input
                type="number"
                required
                min="0.01"
                step="0.01"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Reason *</label>
              <textarea
                required
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                rows={3}
                placeholder="Enter reason for adjustment"
              />
            </div>

            <div className="flex justify-end gap-3 pt-4">
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={loading}
                className="px-4 py-2 bg-orange-600 text-white rounded-lg hover:bg-orange-700 disabled:opacity-50"
              >
                {loading ? 'Adjusting...' : 'Adjust Balance'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
