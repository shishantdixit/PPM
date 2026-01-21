'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { api } from '@/lib/api';

interface LicenseKey {
  licenseKeyId: string;
  key: string;
  subscriptionPlan: string;
  durationMonths: number;
  maxMachines: number;
  maxWorkers: number;
  maxMonthlyBills: number;
  status: string;
  activatedByTenantId?: string;
  activatedByTenantCode?: string;
  activatedByCompanyName?: string;
  activatedAt?: string;
  generatedByUsername: string;
  notes?: string;
  createdAt: string;
}

interface LicenseKeyStats {
  totalKeys: number;
  availableKeys: number;
  usedKeys: number;
  revokedKeys: number;
  keysByPlan: {
    plan: string;
    count: number;
  }[];
}

export default function LicenseKeysPage() {
  const [licenseKeys, setLicenseKeys] = useState<LicenseKey[]>([]);
  const [stats, setStats] = useState<LicenseKeyStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [planFilter, setPlanFilter] = useState<string>('');
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [revokeModalOpen, setRevokeModalOpen] = useState(false);
  const [selectedKey, setSelectedKey] = useState<LicenseKey | null>(null);
  const [revokeReason, setRevokeReason] = useState('');
  const [revoking, setRevoking] = useState(false);

  const limit = 20;

  const fetchLicenseKeys = async () => {
    try {
      setLoading(true);
      const response = await api.getLicenseKeys({
        page,
        limit,
        status: statusFilter || undefined,
        subscriptionPlan: planFilter || undefined,
        search: search || undefined
      });
      if (response.success && response.data) {
        setLicenseKeys(response.data.keys);
        setTotalCount(response.data.totalCount);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load license keys');
    } finally {
      setLoading(false);
    }
  };

  const fetchStats = async () => {
    try {
      const response = await api.getLicenseKeyStats();
      if (response.success && response.data) {
        setStats(response.data);
      }
    } catch (err: any) {
      console.error('Failed to load stats:', err);
    }
  };

  useEffect(() => {
    fetchLicenseKeys();
  }, [page, statusFilter, planFilter, search]);

  useEffect(() => {
    fetchStats();
  }, []);

  const handleRevoke = async () => {
    if (!selectedKey) return;
    setRevoking(true);
    try {
      const response = await api.revokeLicenseKey(selectedKey.licenseKeyId, revokeReason);
      if (response.success) {
        setRevokeModalOpen(false);
        setSelectedKey(null);
        setRevokeReason('');
        fetchLicenseKeys();
        fetchStats();
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to revoke license key');
    } finally {
      setRevoking(false);
    }
  };

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Available':
        return 'bg-green-100 text-green-800';
      case 'Used':
        return 'bg-blue-100 text-blue-800';
      case 'Revoked':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getPlanBadge = (plan: string) => {
    switch (plan) {
      case 'Basic':
        return 'bg-gray-100 text-gray-800';
      case 'Premium':
        return 'bg-purple-100 text-purple-800';
      case 'Enterprise':
        return 'bg-amber-100 text-amber-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const totalPages = Math.ceil(totalCount / limit);

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">License Keys</h1>
          <p className="text-gray-600">Generate and manage license keys for tenant subscriptions</p>
        </div>
        <Link
          href="/admin/license-keys/new"
          className="inline-flex items-center px-4 py-2 bg-purple-600 text-white font-medium rounded-lg hover:bg-purple-700 transition-colors"
        >
          <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          Generate Keys
        </Link>
      </div>

      {/* Stats Cards */}
      {stats && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="bg-white rounded-lg shadow p-4">
            <div className="text-sm text-gray-500">Total Keys</div>
            <div className="text-2xl font-bold text-gray-900">{stats.totalKeys}</div>
          </div>
          <div className="bg-white rounded-lg shadow p-4">
            <div className="text-sm text-gray-500">Available</div>
            <div className="text-2xl font-bold text-green-600">{stats.availableKeys}</div>
          </div>
          <div className="bg-white rounded-lg shadow p-4">
            <div className="text-sm text-gray-500">Used</div>
            <div className="text-2xl font-bold text-blue-600">{stats.usedKeys}</div>
          </div>
          <div className="bg-white rounded-lg shadow p-4">
            <div className="text-sm text-gray-500">Revoked</div>
            <div className="text-2xl font-bold text-red-600">{stats.revokedKeys}</div>
          </div>
        </div>
      )}

      {/* Filters */}
      <div className="bg-white rounded-lg shadow p-4">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Search</label>
            <input
              type="text"
              value={search}
              onChange={(e) => { setSearch(e.target.value); setPage(1); }}
              placeholder="Key or tenant code..."
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Status</label>
            <select
              value={statusFilter}
              onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            >
              <option value="">All Status</option>
              <option value="Available">Available</option>
              <option value="Used">Used</option>
              <option value="Revoked">Revoked</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Plan</label>
            <select
              value={planFilter}
              onChange={(e) => { setPlanFilter(e.target.value); setPage(1); }}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            >
              <option value="">All Plans</option>
              <option value="Basic">Basic</option>
              <option value="Premium">Premium</option>
              <option value="Enterprise">Enterprise</option>
            </select>
          </div>
          <div className="flex items-end">
            <button
              onClick={() => { setSearch(''); setStatusFilter(''); setPlanFilter(''); setPage(1); }}
              className="px-4 py-2 text-gray-600 hover:text-gray-900 border border-gray-300 rounded-lg hover:bg-gray-50"
            >
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700">
          {error}
        </div>
      )}

      {/* License Keys Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Key</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Plan</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Duration</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Activated By</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Created</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {loading ? (
                <tr>
                  <td colSpan={7} className="px-6 py-12 text-center text-gray-500">Loading...</td>
                </tr>
              ) : licenseKeys.length === 0 ? (
                <tr>
                  <td colSpan={7} className="px-6 py-12 text-center text-gray-500">No license keys found</td>
                </tr>
              ) : (
                licenseKeys.map((key) => (
                  <tr key={key.licenseKeyId} className="hover:bg-gray-50">
                    <td className="px-6 py-4">
                      <div className="font-mono text-sm text-gray-900">{key.key}</div>
                      {key.notes && (
                        <div className="text-xs text-gray-500 mt-1">{key.notes}</div>
                      )}
                    </td>
                    <td className="px-6 py-4">
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${getPlanBadge(key.subscriptionPlan)}`}>
                        {key.subscriptionPlan}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500">
                      {key.durationMonths} months
                    </td>
                    <td className="px-6 py-4">
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusBadge(key.status)}`}>
                        {key.status}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      {key.activatedByTenantCode ? (
                        <div>
                          <div className="text-sm text-gray-900">{key.activatedByTenantCode}</div>
                          <div className="text-xs text-gray-500">{key.activatedByCompanyName}</div>
                        </div>
                      ) : (
                        <span className="text-sm text-gray-400">-</span>
                      )}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500">
                      {new Date(key.createdAt).toLocaleDateString()}
                    </td>
                    <td className="px-6 py-4 text-right">
                      {key.status === 'Available' && (
                        <button
                          onClick={() => { setSelectedKey(key); setRevokeModalOpen(true); }}
                          className="text-red-600 hover:text-red-800 text-sm font-medium"
                        >
                          Revoke
                        </button>
                      )}
                      <button
                        onClick={() => navigator.clipboard.writeText(key.key)}
                        className="ml-3 text-gray-600 hover:text-gray-800 text-sm"
                        title="Copy key"
                      >
                        <svg className="w-4 h-4 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                        </svg>
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="px-6 py-4 border-t border-gray-200 flex items-center justify-between">
            <div className="text-sm text-gray-500">
              Showing {((page - 1) * limit) + 1} to {Math.min(page * limit, totalCount)} of {totalCount} keys
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={page === 1}
                className="px-3 py-1 border border-gray-300 rounded text-sm disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
              >
                Previous
              </button>
              <button
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
                className="px-3 py-1 border border-gray-300 rounded text-sm disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
              >
                Next
              </button>
            </div>
          </div>
        )}
      </div>

      {/* Revoke Modal */}
      {revokeModalOpen && selectedKey && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full mx-4 p-6">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Revoke License Key</h3>
            <p className="text-gray-600 mb-4">
              Are you sure you want to revoke this license key? This action cannot be undone.
            </p>
            <div className="bg-gray-50 rounded p-3 mb-4 font-mono text-sm">
              {selectedKey.key}
            </div>
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-1">Reason (optional)</label>
              <textarea
                value={revokeReason}
                onChange={(e) => setRevokeReason(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-red-500 focus:border-transparent"
                rows={3}
                placeholder="Enter reason for revoking..."
              />
            </div>
            <div className="flex justify-end gap-3">
              <button
                onClick={() => { setRevokeModalOpen(false); setSelectedKey(null); setRevokeReason(''); }}
                className="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200"
              >
                Cancel
              </button>
              <button
                onClick={handleRevoke}
                disabled={revoking}
                className="px-4 py-2 text-white bg-red-600 rounded-lg hover:bg-red-700 disabled:opacity-50"
              >
                {revoking ? 'Revoking...' : 'Revoke Key'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
