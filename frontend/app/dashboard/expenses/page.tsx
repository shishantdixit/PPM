'use client';

import { useState, useEffect } from 'react';
import { api } from '@/lib/api';
import type { Expense, ExpenseSummary, CreateExpenseDto, UpdateExpenseDto, ExpenseCategory, PaymentMethod } from '@/types';

const EXPENSE_CATEGORIES = [
  { value: 0, name: 'Electricity' },
  { value: 1, name: 'Rent' },
  { value: 2, name: 'Salary' },
  { value: 3, name: 'Maintenance' },
  { value: 4, name: 'Equipment Repair' },
  { value: 5, name: 'Office Supplies' },
  { value: 6, name: 'Transportation' },
  { value: 7, name: 'Taxes' },
  { value: 8, name: 'Insurance' },
  { value: 9, name: 'Marketing' },
  { value: 10, name: 'Utilities' },
  { value: 11, name: 'Communication' },
  { value: 12, name: 'Bank Charges' },
  { value: 13, name: 'License Fees' },
  { value: 99, name: 'Other' },
];

const PAYMENT_METHODS = [
  { value: 0, name: 'Cash' },
  { value: 1, name: 'Credit' },
  { value: 2, name: 'Digital' },
  { value: 3, name: 'Mixed' },
];

export default function ExpensesPage() {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [summary, setSummary] = useState<ExpenseSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterCategory, setFilterCategory] = useState<ExpenseCategory | undefined>(undefined);
  const [fromDate, setFromDate] = useState<string>('');
  const [toDate, setToDate] = useState<string>('');
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [selectedExpense, setSelectedExpense] = useState<Expense | null>(null);
  const [exporting, setExporting] = useState(false);
  const [showExportMenu, setShowExportMenu] = useState(false);

  // Form state
  const [formData, setFormData] = useState<CreateExpenseDto>({
    category: 0,
    description: '',
    amount: 0,
    expenseDate: new Date().toISOString().split('T')[0],
    paymentMode: 0,
    reference: '',
    vendor: '',
    notes: '',
  });

  useEffect(() => {
    // Set default date range to last 30 days
    const today = new Date();
    const thirtyDaysAgo = new Date(today);
    thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
    setFromDate(thirtyDaysAgo.toISOString().split('T')[0]);
    setToDate(today.toISOString().split('T')[0]);
  }, []);

  useEffect(() => {
    if (fromDate && toDate) {
      fetchData();
    }
  }, [fromDate, toDate, filterCategory]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [expensesRes, summaryRes] = await Promise.all([
        api.getExpenses(fromDate, toDate, filterCategory, undefined, searchTerm || undefined),
        api.getExpenseSummary(fromDate, toDate)
      ]);

      if (expensesRes.success && expensesRes.data) {
        setExpenses(expensesRes.data);
      }
      if (summaryRes.success && summaryRes.data) {
        setSummary(summaryRes.data);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to fetch data');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    fetchData();
  };

  const handleCreateExpense = async () => {
    try {
      if (!formData.description || formData.amount <= 0) {
        setError('Please fill in all required fields');
        return;
      }

      const result = await api.createExpense(formData);
      if (result.success) {
        setSuccess('Expense created successfully');
        setShowCreateModal(false);
        resetForm();
        fetchData();
      } else {
        setError(result.message || 'Failed to create expense');
      }
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Failed to create expense');
    }
  };

  const handleUpdateExpense = async () => {
    if (!selectedExpense) return;

    try {
      if (!formData.description || formData.amount <= 0) {
        setError('Please fill in all required fields');
        return;
      }

      const result = await api.updateExpense(selectedExpense.expenseId, formData as UpdateExpenseDto);
      if (result.success) {
        setSuccess('Expense updated successfully');
        setShowEditModal(false);
        setSelectedExpense(null);
        resetForm();
        fetchData();
      } else {
        setError(result.message || 'Failed to update expense');
      }
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Failed to update expense');
    }
  };

  const handleDeleteExpense = async (expense: Expense) => {
    if (!confirm('Are you sure you want to delete this expense?')) return;

    try {
      const result = await api.deleteExpense(expense.expenseId);
      if (result.success) {
        setSuccess('Expense deleted successfully');
        fetchData();
      } else {
        setError(result.message || 'Failed to delete expense');
      }
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Failed to delete expense');
    }
  };

  const openEditModal = (expense: Expense) => {
    setSelectedExpense(expense);
    setFormData({
      category: expense.category,
      description: expense.description,
      amount: expense.amount,
      expenseDate: expense.expenseDate,
      paymentMode: expense.paymentMode,
      reference: expense.reference || '',
      vendor: expense.vendor || '',
      notes: expense.notes || '',
    });
    setShowEditModal(true);
  };

  const resetForm = () => {
    setFormData({
      category: 0,
      description: '',
      amount: 0,
      expenseDate: new Date().toISOString().split('T')[0],
      paymentMode: 0,
      reference: '',
      vendor: '',
      notes: '',
    });
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 2
    }).format(value);
  };

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  };

  const handleExport = async (format: string) => {
    setExporting(true);
    setShowExportMenu(false);
    try {
      await api.exportExpenses(fromDate, toDate, filterCategory, format);
    } catch (err: any) {
      setError(err.message || 'Failed to export expenses');
    } finally {
      setExporting(false);
    }
  };

  // Clear messages after 5 seconds
  useEffect(() => {
    if (error || success) {
      const timer = setTimeout(() => {
        setError(null);
        setSuccess(null);
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [error, success]);

  if (loading && !expenses.length) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg">Loading...</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Expense Management</h1>
        <div className="flex items-center gap-2">
          <div className="relative">
            <button
              onClick={() => setShowExportMenu(!showExportMenu)}
              disabled={exporting}
              className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 flex items-center gap-2"
            >
              {exporting ? 'Exporting...' : 'Export'}
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
              </svg>
            </button>
            {showExportMenu && (
              <div className="absolute right-0 mt-2 w-40 bg-white rounded-lg shadow-lg border z-10">
                <button
                  onClick={() => handleExport('pdf')}
                  className="w-full px-4 py-2 text-left hover:bg-gray-100 rounded-t-lg"
                >
                  PDF
                </button>
                <button
                  onClick={() => handleExport('excel')}
                  className="w-full px-4 py-2 text-left hover:bg-gray-100"
                >
                  Excel
                </button>
                <button
                  onClick={() => handleExport('csv')}
                  className="w-full px-4 py-2 text-left hover:bg-gray-100 rounded-b-lg"
                >
                  CSV
                </button>
              </div>
            )}
          </div>
          <button
            onClick={() => {
              resetForm();
              setShowCreateModal(true);
            }}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
          >
            + Add Expense
          </button>
        </div>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      )}

      {success && (
        <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded">
          {success}
        </div>
      )}

      {/* Summary Cards */}
      {summary && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="bg-white p-4 rounded-lg shadow border">
            <h3 className="text-sm font-medium text-gray-500">Total Expenses</h3>
            <p className="text-2xl font-bold text-red-600">{formatCurrency(summary.totalExpenses)}</p>
          </div>
          <div className="bg-white p-4 rounded-lg shadow border">
            <h3 className="text-sm font-medium text-gray-500">Total Entries</h3>
            <p className="text-2xl font-bold text-gray-900">{expenses.length}</p>
          </div>
          <div className="bg-white p-4 rounded-lg shadow border">
            <h3 className="text-sm font-medium text-gray-500">Categories Used</h3>
            <p className="text-2xl font-bold text-gray-900">{summary.byCategory.length}</p>
          </div>
          <div className="bg-white p-4 rounded-lg shadow border">
            <h3 className="text-sm font-medium text-gray-500">Date Range</h3>
            <p className="text-sm font-medium text-gray-900">
              {fromDate && formatDate(fromDate)} - {toDate && formatDate(toDate)}
            </p>
          </div>
        </div>
      )}

      {/* Category Breakdown */}
      {summary && summary.byCategory.length > 0 && (
        <div className="bg-white p-4 rounded-lg shadow border">
          <h3 className="text-lg font-semibold mb-4">Expenses by Category</h3>
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-5 gap-4">
            {summary.byCategory.map((cat) => (
              <div key={cat.category} className="bg-gray-50 p-3 rounded-lg">
                <p className="text-sm text-gray-500">{cat.categoryName}</p>
                <p className="text-lg font-semibold text-gray-900">{formatCurrency(cat.amount)}</p>
                <p className="text-xs text-gray-400">{cat.count} entries</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Filters */}
      <div className="bg-white p-4 rounded-lg shadow border">
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">From Date</label>
            <input
              type="date"
              value={fromDate}
              onChange={(e) => setFromDate(e.target.value)}
              className="w-full border rounded-lg px-3 py-2"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">To Date</label>
            <input
              type="date"
              value={toDate}
              onChange={(e) => setToDate(e.target.value)}
              className="w-full border rounded-lg px-3 py-2"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Category</label>
            <select
              value={filterCategory ?? ''}
              onChange={(e) => setFilterCategory(e.target.value ? Number(e.target.value) as ExpenseCategory : undefined)}
              className="w-full border rounded-lg px-3 py-2"
            >
              <option value="">All Categories</option>
              {EXPENSE_CATEGORIES.map((cat) => (
                <option key={cat.value} value={cat.value}>{cat.name}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Search</label>
            <input
              type="text"
              placeholder="Description, vendor..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
              className="w-full border rounded-lg px-3 py-2"
            />
          </div>
          <div className="flex items-end">
            <button
              onClick={handleSearch}
              className="w-full bg-gray-600 text-white px-4 py-2 rounded-lg hover:bg-gray-700"
            >
              Search
            </button>
          </div>
        </div>
      </div>

      {/* Expenses Table */}
      <div className="bg-white rounded-lg shadow border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Category</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Description</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Vendor</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Amount</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Payment</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Recorded By</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {expenses.length === 0 ? (
                <tr>
                  <td colSpan={8} className="px-6 py-8 text-center text-gray-500">
                    No expenses found for the selected period
                  </td>
                </tr>
              ) : (
                expenses.map((expense) => (
                  <tr key={expense.expenseId} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {formatDate(expense.expenseDate)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="px-2 py-1 text-xs font-medium bg-blue-100 text-blue-800 rounded-full">
                        {expense.categoryName}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-900">
                      {expense.description}
                      {expense.reference && (
                        <span className="block text-xs text-gray-500">Ref: {expense.reference}</span>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {expense.vendor || '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-red-600 text-right">
                      {formatCurrency(expense.amount)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {expense.paymentModeName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {expense.recordedByName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm">
                      <button
                        onClick={() => openEditModal(expense)}
                        className="text-blue-600 hover:text-blue-800 mr-3"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteExpense(expense)}
                        className="text-red-600 hover:text-red-800"
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

      {/* Create/Edit Modal */}
      {(showCreateModal || showEditModal) && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-lg mx-4">
            <h2 className="text-xl font-bold mb-4">
              {showCreateModal ? 'Add New Expense' : 'Edit Expense'}
            </h2>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Category <span className="text-red-500">*</span>
                </label>
                <select
                  value={formData.category}
                  onChange={(e) => setFormData({ ...formData, category: Number(e.target.value) as ExpenseCategory })}
                  className="w-full border rounded-lg px-3 py-2"
                >
                  {EXPENSE_CATEGORIES.map((cat) => (
                    <option key={cat.value} value={cat.value}>{cat.name}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Description <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  className="w-full border rounded-lg px-3 py-2"
                  placeholder="Enter expense description"
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Amount <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    min="0"
                    value={formData.amount}
                    onChange={(e) => setFormData({ ...formData, amount: parseFloat(e.target.value) || 0 })}
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Date <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="date"
                    value={formData.expenseDate}
                    onChange={(e) => setFormData({ ...formData, expenseDate: e.target.value })}
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Payment Mode
                  </label>
                  <select
                    value={formData.paymentMode}
                    onChange={(e) => setFormData({ ...formData, paymentMode: Number(e.target.value) as PaymentMethod })}
                    className="w-full border rounded-lg px-3 py-2"
                  >
                    {PAYMENT_METHODS.map((method) => (
                      <option key={method.value} value={method.value}>{method.name}</option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Reference/Bill No.
                  </label>
                  <input
                    type="text"
                    value={formData.reference}
                    onChange={(e) => setFormData({ ...formData, reference: e.target.value })}
                    className="w-full border rounded-lg px-3 py-2"
                    placeholder="Invoice/Bill number"
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Vendor/Payee
                </label>
                <input
                  type="text"
                  value={formData.vendor}
                  onChange={(e) => setFormData({ ...formData, vendor: e.target.value })}
                  className="w-full border rounded-lg px-3 py-2"
                  placeholder="Vendor or payee name"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Notes
                </label>
                <textarea
                  value={formData.notes}
                  onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                  className="w-full border rounded-lg px-3 py-2"
                  rows={2}
                  placeholder="Additional notes (optional)"
                />
              </div>
            </div>
            <div className="flex justify-end gap-3 mt-6">
              <button
                onClick={() => {
                  setShowCreateModal(false);
                  setShowEditModal(false);
                  setSelectedExpense(null);
                  resetForm();
                }}
                className="px-4 py-2 text-gray-700 border rounded-lg hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                onClick={showCreateModal ? handleCreateExpense : handleUpdateExpense}
                className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
              >
                {showCreateModal ? 'Create Expense' : 'Update Expense'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
