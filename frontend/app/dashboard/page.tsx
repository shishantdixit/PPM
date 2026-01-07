'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/context/AuthContext';
import { api } from '@/lib/api';
import type { DashboardSummary, SalesTrend } from '@/types';

export default function DashboardPage() {
  const { user } = useAuth();
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await api.getDashboardSummary();
      if (response.success && response.data) {
        setSummary(response.data);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load dashboard');
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(value);
  };

  const formatNumber = (value: number) => {
    return new Intl.NumberFormat('en-IN', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value);
  };

  const getChangeColor = (percent: number) => {
    if (percent > 0) return 'text-green-600';
    if (percent < 0) return 'text-red-600';
    return 'text-gray-600';
  };

  const getChangeArrow = (percent: number) => {
    if (percent > 0) return '↑';
    if (percent < 0) return '↓';
    return '→';
  };

  // Simple bar chart component - responsive
  const SimpleBarChart = ({ data }: { data: SalesTrend[] }) => {
    const maxAmount = Math.max(...data.map(d => d.totalAmount), 1);

    // Format currency compactly for mobile
    const formatCompact = (value: number) => {
      if (value >= 100000) return `${(value / 100000).toFixed(1)}L`;
      if (value >= 1000) return `${(value / 1000).toFixed(1)}K`;
      return value.toFixed(0);
    };

    return (
      <div className="flex items-end justify-between gap-0.5 sm:gap-1 h-28 sm:h-32">
        {data.map((day, index) => {
          const height = (day.totalAmount / maxAmount) * 100;
          const date = new Date(day.date);
          const dayName = date.toLocaleDateString('en-US', { weekday: 'short' });
          const dayLetter = dayName.charAt(0);

          return (
            <div key={index} className="flex flex-col items-center flex-1 min-w-0">
              <div className="text-[10px] sm:text-xs text-gray-500 mb-1 truncate w-full text-center">
                <span className="hidden sm:inline">{formatCurrency(day.totalAmount)}</span>
                <span className="sm:hidden">{formatCompact(day.totalAmount)}</span>
              </div>
              <div
                className="w-full bg-blue-500 rounded-t transition-all duration-300 hover:bg-blue-600"
                style={{ height: `${Math.max(height, 2)}%` }}
                title={`${dayName}: ${formatCurrency(day.totalAmount)}`}
              />
              <div className="text-[10px] sm:text-xs text-gray-500 mt-1">
                <span className="hidden sm:inline">{dayName}</span>
                <span className="sm:hidden">{dayLetter}</span>
              </div>
            </div>
          );
        })}
      </div>
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-gray-500">Loading dashboard...</div>
      </div>
    );
  }

  return (
    <div className="space-y-4 sm:space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2">
        <h1 className="text-xl sm:text-2xl font-bold text-gray-900">Dashboard</h1>
        <div className="text-xs sm:text-sm text-gray-500">
          {new Date().toLocaleDateString('en-IN', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
        </div>
      </div>

      {error && (
        <div className="bg-red-50 text-red-700 p-3 sm:p-4 rounded-lg text-sm">{error}</div>
      )}

      {/* Welcome Card */}
      <div className="bg-gradient-to-r from-blue-600 to-indigo-600 rounded-lg shadow p-4 sm:p-6 text-white">
        <h2 className="text-lg sm:text-xl font-semibold">Welcome back, {user?.fullName}!</h2>
        <p className="text-blue-100 mt-1 text-sm sm:text-base">
          {user?.role} at {user?.tenantCode || 'Your Organization'}
        </p>
        {summary && summary.activeShifts > 0 && (
          <div className="mt-3 sm:mt-4 bg-white/20 rounded-lg p-2 sm:p-3 inline-block text-sm sm:text-base">
            <span className="font-semibold">{summary.activeShifts}</span> active shift(s) right now
          </div>
        )}
      </div>

      {summary && (
        <>
          {/* Today's Stats */}
          <div className="grid grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4">
            <div className="bg-white rounded-lg shadow p-3 sm:p-6">
              <div className="flex items-center justify-between">
                <div className="min-w-0 flex-1">
                  <p className="text-xs sm:text-sm text-gray-500 font-medium">Today's Sales</p>
                  <p className="text-lg sm:text-2xl font-bold text-gray-900 mt-1 truncate">{formatCurrency(summary.todaySales)}</p>
                </div>
                <div className="text-2xl sm:text-3xl ml-2">💰</div>
              </div>
              <div className={`mt-1 sm:mt-2 text-xs sm:text-sm ${getChangeColor(summary.salesChangePercent)}`}>
                {getChangeArrow(summary.salesChangePercent)} {Math.abs(summary.salesChangePercent).toFixed(1)}%
                <span className="hidden sm:inline"> vs yesterday</span>
              </div>
            </div>

            <div className="bg-white rounded-lg shadow p-3 sm:p-6">
              <div className="flex items-center justify-between">
                <div className="min-w-0 flex-1">
                  <p className="text-xs sm:text-sm text-gray-500 font-medium">Today's Volume</p>
                  <p className="text-lg sm:text-2xl font-bold text-gray-900 mt-1 truncate">{formatNumber(summary.todayQuantity)} L</p>
                </div>
                <div className="text-2xl sm:text-3xl ml-2">⛽</div>
              </div>
              <div className="mt-1 sm:mt-2 text-xs sm:text-sm text-gray-500">
                {summary.todaySalesCount} <span className="hidden sm:inline">transactions</span><span className="sm:hidden">txn</span>
              </div>
            </div>

            <div className="bg-white rounded-lg shadow p-3 sm:p-6">
              <div className="flex items-center justify-between">
                <div className="min-w-0 flex-1">
                  <p className="text-xs sm:text-sm text-gray-500 font-medium">This Week</p>
                  <p className="text-lg sm:text-2xl font-bold text-gray-900 mt-1 truncate">{formatCurrency(summary.weekSales)}</p>
                </div>
                <div className="text-2xl sm:text-3xl ml-2">📊</div>
              </div>
              <div className={`mt-1 sm:mt-2 text-xs sm:text-sm ${getChangeColor(summary.weekChangePercent)}`}>
                {getChangeArrow(summary.weekChangePercent)} {Math.abs(summary.weekChangePercent).toFixed(1)}%
                <span className="hidden sm:inline"> vs last week</span>
              </div>
            </div>

            <div className="bg-white rounded-lg shadow p-3 sm:p-6">
              <div className="flex items-center justify-between">
                <div className="min-w-0 flex-1">
                  <p className="text-xs sm:text-sm text-gray-500 font-medium">This Month</p>
                  <p className="text-lg sm:text-2xl font-bold text-gray-900 mt-1 truncate">{formatCurrency(summary.monthSales)}</p>
                </div>
                <div className="text-2xl sm:text-3xl ml-2">📅</div>
              </div>
              <div className={`mt-1 sm:mt-2 text-xs sm:text-sm ${getChangeColor(summary.monthChangePercent)}`}>
                {getChangeArrow(summary.monthChangePercent)} {Math.abs(summary.monthChangePercent).toFixed(1)}%
                <span className="hidden sm:inline"> vs last month</span>
              </div>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
            {/* Sales Trend Chart */}
            <div className="bg-white rounded-lg shadow p-4 sm:p-6">
              <h3 className="text-base sm:text-lg font-semibold mb-3 sm:mb-4">Last 7 Days Sales</h3>
              {summary.last7DaysTrend && summary.last7DaysTrend.length > 0 ? (
                <SimpleBarChart data={summary.last7DaysTrend} />
              ) : (
                <div className="text-gray-500 text-center py-8">No sales data available</div>
              )}
            </div>

            {/* Top Fuel Types */}
            <div className="bg-white rounded-lg shadow p-4 sm:p-6">
              <h3 className="text-base sm:text-lg font-semibold mb-3 sm:mb-4">Top Fuel Types (This Month)</h3>
              {summary.topFuelTypes && summary.topFuelTypes.length > 0 ? (
                <div className="space-y-4">
                  {summary.topFuelTypes.map((fuel, index) => (
                    <div key={fuel.fuelTypeId} className="flex items-center">
                      <div className="w-8 h-8 rounded-full bg-blue-100 text-blue-600 flex items-center justify-center font-bold text-sm">
                        {index + 1}
                      </div>
                      <div className="ml-3 flex-1">
                        <div className="flex items-center justify-between">
                          <span className="font-medium">{fuel.fuelName}</span>
                          <span className="text-gray-900 font-semibold">{formatCurrency(fuel.totalAmount)}</span>
                        </div>
                        <div className="flex items-center justify-between text-sm text-gray-500">
                          <span>{formatNumber(fuel.totalQuantity)} L</span>
                          <span>{fuel.salesCount} sales</span>
                        </div>
                        <div className="mt-1 h-2 bg-gray-100 rounded-full overflow-hidden">
                          <div
                            className="h-full bg-blue-500 rounded-full"
                            style={{ width: `${fuel.percentageOfTotal}%` }}
                          />
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="text-gray-500 text-center py-8">No fuel sales data available</div>
              )}
            </div>
          </div>

          {/* Quick Stats */}
          <div className="bg-white rounded-lg shadow p-4 sm:p-6">
            <h3 className="text-base sm:text-lg font-semibold mb-3 sm:mb-4">Quick Stats</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-2 sm:gap-4">
              <div className="text-center p-2 sm:p-4 bg-green-50 rounded-lg">
                <p className="text-xl sm:text-3xl font-bold text-green-600">{formatNumber(summary.weekQuantity)}</p>
                <p className="text-xs sm:text-sm text-gray-600">Liters This Week</p>
              </div>
              <div className="text-center p-2 sm:p-4 bg-purple-50 rounded-lg">
                <p className="text-xl sm:text-3xl font-bold text-purple-600">{formatNumber(summary.monthQuantity)}</p>
                <p className="text-xs sm:text-sm text-gray-600">Liters This Month</p>
              </div>
              <div className="text-center p-2 sm:p-4 bg-orange-50 rounded-lg">
                <p className="text-xl sm:text-3xl font-bold text-orange-600">{summary.activeShifts}</p>
                <p className="text-xs sm:text-sm text-gray-600">Active Shifts</p>
              </div>
              <div className="text-center p-2 sm:p-4 bg-blue-50 rounded-lg">
                <p className="text-xl sm:text-3xl font-bold text-blue-600">{summary.topFuelTypes?.length || 0}</p>
                <p className="text-xs sm:text-sm text-gray-600">Fuel Types Sold</p>
              </div>
            </div>
          </div>
        </>
      )}

      {/* Quick Links */}
      <div className="bg-white rounded-lg shadow p-4 sm:p-6">
        <h3 className="text-base sm:text-lg font-semibold mb-3 sm:mb-4">Quick Actions</h3>
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-2 sm:gap-4">
          <a
            href="/dashboard/shifts"
            className="p-3 sm:p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors text-center"
          >
            <div className="text-xl sm:text-2xl mb-1 sm:mb-2">👷</div>
            <div className="font-medium text-sm sm:text-base">My Shift</div>
          </a>
          {(user?.role === 'Manager' || user?.role === 'Owner') && (
            <>
              <a
                href="/dashboard/shifts/all"
                className="p-3 sm:p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors text-center"
              >
                <div className="text-xl sm:text-2xl mb-1 sm:mb-2">📋</div>
                <div className="font-medium text-sm sm:text-base">All Shifts</div>
              </a>
              <a
                href="/dashboard/reports"
                className="p-3 sm:p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors text-center"
              >
                <div className="text-xl sm:text-2xl mb-1 sm:mb-2">📊</div>
                <div className="font-medium text-sm sm:text-base">Reports</div>
              </a>
            </>
          )}
          {user?.role === 'Owner' && (
            <a
              href="/dashboard/users"
              className="p-3 sm:p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors text-center"
            >
              <div className="text-xl sm:text-2xl mb-1 sm:mb-2">👥</div>
              <div className="font-medium text-sm sm:text-base">Users</div>
            </a>
          )}
        </div>
      </div>
    </div>
  );
}
