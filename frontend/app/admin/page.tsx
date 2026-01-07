'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { api } from '@/lib/api';
import { SystemDashboard, TenantSummary } from '@/types';

export default function AdminDashboardPage() {
  const [dashboard, setDashboard] = useState<SystemDashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      const response = await api.getAdminDashboard();
      if (response.success && response.data) {
        setDashboard(response.data);
      } else {
        setError(response.message || 'Failed to load dashboard');
      }
    } catch (err) {
      setError('Failed to load dashboard');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
        <p className="text-red-600">{error}</p>
        <button
          onClick={loadDashboard}
          className="mt-4 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
        >
          Retry
        </button>
      </div>
    );
  }

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      maximumFractionDigits: 0,
    }).format(amount);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">System Dashboard</h1>
        <button
          onClick={loadDashboard}
          className="px-4 py-2 text-sm text-purple-600 hover:text-purple-700 flex items-center space-x-2"
        >
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          <span>Refresh</span>
        </button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-500">Total Tenants</p>
              <p className="text-3xl font-bold text-gray-900 mt-1">{dashboard?.totalTenants || 0}</p>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
              <svg className="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
            </div>
          </div>
          <div className="mt-4 flex items-center space-x-4 text-sm">
            <span className="text-green-600">{dashboard?.activeTenants || 0} active</span>
            <span className="text-yellow-600">{dashboard?.suspendedTenants || 0} suspended</span>
            <span className="text-red-600">{dashboard?.expiredTenants || 0} expired</span>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-500">Total Users</p>
              <p className="text-3xl font-bold text-gray-900 mt-1">{dashboard?.totalUsers || 0}</p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
            </div>
          </div>
          <p className="text-sm text-gray-500 mt-4">Across all tenants</p>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-500">This Month Sales</p>
              <p className="text-3xl font-bold text-gray-900 mt-1">
                {formatCurrency(dashboard?.totalSalesThisMonth || 0)}
              </p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
              <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
          <p className="text-sm text-gray-500 mt-4">System-wide revenue</p>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-500">All Time Sales</p>
              <p className="text-3xl font-bold text-gray-900 mt-1">
                {formatCurrency(dashboard?.totalSalesAllTime || 0)}
              </p>
            </div>
            <div className="w-12 h-12 bg-indigo-100 rounded-lg flex items-center justify-center">
              <svg className="w-6 h-6 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
            </div>
          </div>
          <p className="text-sm text-gray-500 mt-4">Total system revenue</p>
        </div>
      </div>

      {/* Subscription Breakdown */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Subscription Plans</h2>
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-3 h-3 bg-gray-400 rounded-full"></div>
                <span className="text-gray-600">Basic</span>
              </div>
              <span className="font-semibold">{dashboard?.subscriptionBreakdown?.basic || 0}</span>
            </div>
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-3 h-3 bg-blue-500 rounded-full"></div>
                <span className="text-gray-600">Premium</span>
              </div>
              <span className="font-semibold">{dashboard?.subscriptionBreakdown?.premium || 0}</span>
            </div>
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-3 h-3 bg-purple-500 rounded-full"></div>
                <span className="text-gray-600">Enterprise</span>
              </div>
              <span className="font-semibold">{dashboard?.subscriptionBreakdown?.enterprise || 0}</span>
            </div>
          </div>
        </div>

        {/* Top Tenants */}
        <div className="lg:col-span-2 bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-900">Top Performing Tenants</h2>
            <Link href="/admin/tenants" className="text-sm text-purple-600 hover:text-purple-700">
              View all
            </Link>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="text-left text-sm text-gray-500 border-b">
                  <th className="pb-3 font-medium">Tenant</th>
                  <th className="pb-3 font-medium">Location</th>
                  <th className="pb-3 font-medium">Plan</th>
                  <th className="pb-3 font-medium text-right">This Month</th>
                </tr>
              </thead>
              <tbody className="divide-y">
                {dashboard?.topTenants?.map((tenant: TenantSummary) => (
                  <tr key={tenant.tenantId} className="text-sm">
                    <td className="py-3">
                      <div>
                        <p className="font-medium text-gray-900">{tenant.companyName}</p>
                        <p className="text-gray-500">{tenant.tenantCode}</p>
                      </div>
                    </td>
                    <td className="py-3 text-gray-600">
                      {tenant.city}, {tenant.state}
                    </td>
                    <td className="py-3">
                      <span
                        className={`px-2 py-1 text-xs rounded-full ${
                          tenant.subscriptionPlan === 'Enterprise'
                            ? 'bg-purple-100 text-purple-700'
                            : tenant.subscriptionPlan === 'Premium'
                            ? 'bg-blue-100 text-blue-700'
                            : 'bg-gray-100 text-gray-700'
                        }`}
                      >
                        {tenant.subscriptionPlan}
                      </span>
                    </td>
                    <td className="py-3 text-right font-medium text-gray-900">
                      {formatCurrency(tenant.thisMonthSales || 0)}
                    </td>
                  </tr>
                ))}
                {(!dashboard?.topTenants || dashboard.topTenants.length === 0) && (
                  <tr>
                    <td colSpan={4} className="py-6 text-center text-gray-500">
                      No tenant data available
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* Monthly Growth */}
      <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Monthly Growth (Last 6 Months)</h2>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="text-left text-sm text-gray-500 border-b">
                <th className="pb-3 font-medium">Month</th>
                <th className="pb-3 font-medium text-right">New Tenants</th>
                <th className="pb-3 font-medium text-right">Total Sales</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {dashboard?.monthlyGrowth?.map((month) => (
                <tr key={month.month} className="text-sm">
                  <td className="py-3 font-medium text-gray-900">{month.month}</td>
                  <td className="py-3 text-right text-gray-600">{month.newTenants}</td>
                  <td className="py-3 text-right font-medium text-gray-900">
                    {formatCurrency(month.totalSales || 0)}
                  </td>
                </tr>
              ))}
              {(!dashboard?.monthlyGrowth || dashboard.monthlyGrowth.length === 0) && (
                <tr>
                  <td colSpan={3} className="py-6 text-center text-gray-500">
                    No growth data available
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
