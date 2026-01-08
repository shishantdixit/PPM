'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { api } from '@/lib/api';
import { TenantDetail, UpdateSubscriptionDto, CreateOwnerUserDto, TenantFeature, UpdateTenantFeatureDto } from '@/types';

export default function TenantDetailPage() {
  const params = useParams();
  const router = useRouter();
  const tenantId = params.id as string;

  const [tenant, setTenant] = useState<TenantDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState('overview');
  const [showSubscriptionModal, setShowSubscriptionModal] = useState(false);
  const [showOwnerModal, setShowOwnerModal] = useState(false);
  const [showFeaturesModal, setShowFeaturesModal] = useState(false);
  const [tenantFeatures, setTenantFeatures] = useState<TenantFeature[]>([]);

  useEffect(() => {
    loadTenant();
  }, [tenantId]);

  const loadTenant = async () => {
    try {
      const response = await api.getTenantDetails(tenantId);
      if (response.success && response.data) {
        setTenant(response.data);
      } else {
        setError(response.message || 'Failed to load tenant');
      }
    } catch (err) {
      setError('Failed to load tenant');
    } finally {
      setLoading(false);
    }
  };

  const handleStatusChange = async (isActive: boolean) => {
    try {
      const response = await api.updateTenantStatus(tenantId, isActive);
      if (response.success) {
        loadTenant();
      }
    } catch (err) {
      setError('Failed to update tenant status');
    }
  };

  const loadTenantFeatures = async () => {
    try {
      const response = await api.getTenantFeatures(tenantId);
      if (response.success && response.data) {
        setTenantFeatures(response.data);
      }
    } catch (err) {
      console.error('Failed to load tenant features');
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      maximumFractionDigits: 0,
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-IN', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600"></div>
      </div>
    );
  }

  if (error || !tenant) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
        <p className="text-red-600">{error || 'Tenant not found'}</p>
        <Link href="/admin/tenants" className="mt-4 inline-block text-purple-600 hover:text-purple-700">
          Back to Tenants
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <Link
            href="/admin/tenants"
            className="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </Link>
          <div>
            <h1 className="text-2xl font-bold text-gray-900">{tenant.companyName}</h1>
            <p className="text-gray-500">{tenant.tenantCode}</p>
          </div>
        </div>
        <div className="flex items-center space-x-3">
          <button
            onClick={() => setShowSubscriptionModal(true)}
            className="px-4 py-2 border border-purple-600 text-purple-600 rounded-lg hover:bg-purple-50"
          >
            Update Subscription
          </button>
          {tenant.isActive ? (
            <button
              onClick={() => handleStatusChange(false)}
              className="px-4 py-2 bg-yellow-500 text-white rounded-lg hover:bg-yellow-600"
            >
              Suspend Tenant
            </button>
          ) : (
            <button
              onClick={() => handleStatusChange(true)}
              className="px-4 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600"
            >
              Activate Tenant
            </button>
          )}
        </div>
      </div>

      {/* Status Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
          <p className="text-sm text-gray-500">Status</p>
          <span
            className={`inline-block mt-1 px-3 py-1 text-sm font-medium rounded-full ${
              tenant.subscriptionStatus === 'Active'
                ? 'bg-green-100 text-green-700'
                : tenant.subscriptionStatus === 'Suspended'
                ? 'bg-yellow-100 text-yellow-700'
                : 'bg-red-100 text-red-700'
            }`}
          >
            {tenant.subscriptionStatus}
          </span>
        </div>
        <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
          <p className="text-sm text-gray-500">Plan</p>
          <span
            className={`inline-block mt-1 px-3 py-1 text-sm font-medium rounded-full ${
              tenant.subscriptionPlan === 'Enterprise'
                ? 'bg-purple-100 text-purple-700'
                : tenant.subscriptionPlan === 'Premium'
                ? 'bg-blue-100 text-blue-700'
                : 'bg-gray-100 text-gray-700'
            }`}
          >
            {tenant.subscriptionPlan}
          </span>
        </div>
        <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
          <p className="text-sm text-gray-500">This Month Sales</p>
          <p className="text-xl font-bold text-gray-900 mt-1">{formatCurrency(tenant.thisMonthSales || 0)}</p>
        </div>
        <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
          <p className="text-sm text-gray-500">Total Sales</p>
          <p className="text-xl font-bold text-gray-900 mt-1">{formatCurrency(tenant.totalSales || 0)}</p>
        </div>
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200">
        <nav className="flex space-x-8">
          {['overview', 'features', 'users', 'sales'].map((tab) => (
            <button
              key={tab}
              onClick={() => {
                setActiveTab(tab);
                if (tab === 'features' && tenantFeatures.length === 0) {
                  loadTenantFeatures();
                }
              }}
              className={`py-4 px-1 border-b-2 font-medium text-sm capitalize ${
                activeTab === tab
                  ? 'border-purple-600 text-purple-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }`}
            >
              {tab}
            </button>
          ))}
        </nav>
      </div>

      {/* Tab Content */}
      {activeTab === 'overview' && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Company Info */}
          <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Company Information</h3>
            <dl className="space-y-3">
              <div className="flex justify-between">
                <dt className="text-gray-500">Owner</dt>
                <dd className="text-gray-900 font-medium">{tenant.ownerName}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Email</dt>
                <dd className="text-gray-900">{tenant.email}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Phone</dt>
                <dd className="text-gray-900">{tenant.phone}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Address</dt>
                <dd className="text-gray-900 text-right">{tenant.address || '-'}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">City</dt>
                <dd className="text-gray-900">{tenant.city}, {tenant.state}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">PIN Code</dt>
                <dd className="text-gray-900">{tenant.pinCode || '-'}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Created</dt>
                <dd className="text-gray-900">{formatDate(tenant.createdAt.toString())}</dd>
              </div>
            </dl>
          </div>

          {/* Subscription Info */}
          <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Subscription Details</h3>
            <dl className="space-y-3">
              <div className="flex justify-between">
                <dt className="text-gray-500">Plan</dt>
                <dd className="text-gray-900 font-medium">{tenant.subscriptionPlan}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Start Date</dt>
                <dd className="text-gray-900">{formatDate(tenant.subscriptionStartDate.toString())}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">End Date</dt>
                <dd className="text-gray-900">
                  {tenant.subscriptionEndDate ? formatDate(tenant.subscriptionEndDate.toString()) : 'No expiry'}
                </dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Max Machines</dt>
                <dd className="text-gray-900">{tenant.maxMachines}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Max Workers</dt>
                <dd className="text-gray-900">{tenant.maxWorkers}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Max Monthly Bills</dt>
                <dd className="text-gray-900">{tenant.maxMonthlyBills}</dd>
              </div>
            </dl>
          </div>

          {/* Stats */}
          <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Usage Statistics</h3>
            <dl className="space-y-3">
              <div className="flex justify-between">
                <dt className="text-gray-500">Users</dt>
                <dd className="text-gray-900 font-medium">{tenant.userCount}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Machines</dt>
                <dd className="text-gray-900 font-medium">{tenant.machineCount}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Nozzles</dt>
                <dd className="text-gray-900 font-medium">{tenant.nozzleCount}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500">Total Shifts</dt>
                <dd className="text-gray-900 font-medium">{tenant.totalShifts}</dd>
              </div>
            </dl>
          </div>
        </div>
      )}

      {activeTab === 'features' && (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="flex items-center justify-between p-6 border-b border-gray-200">
            <div>
              <h3 className="text-lg font-semibold text-gray-900">Feature Access</h3>
              <p className="text-sm text-gray-500 mt-1">
                Plan: <span className="font-medium">{tenant.subscriptionPlan}</span> - Override features for this tenant
              </p>
            </div>
            <div className="flex space-x-2">
              <button
                onClick={async () => {
                  try {
                    const response = await api.resetTenantFeatures(tenantId);
                    if (response.success && response.data) {
                      setTenantFeatures(response.data);
                    }
                  } catch (err) {
                    console.error('Failed to reset features');
                  }
                }}
                className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 text-sm"
              >
                Reset to Plan Defaults
              </button>
            </div>
          </div>
          <div className="p-6">
            {tenantFeatures.length === 0 ? (
              <div className="text-center py-8 text-gray-500">Loading features...</div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {tenantFeatures.map((feature) => (
                  <div
                    key={feature.featureId}
                    className={`flex items-center justify-between p-4 rounded-lg border ${
                      feature.isEnabled ? 'border-green-200 bg-green-50' : 'border-gray-200 bg-gray-50'
                    }`}
                  >
                    <div className="flex-1">
                      <div className="flex items-center gap-2">
                        <span className="font-medium text-gray-900">{feature.featureName}</span>
                        {feature.isOverridden && (
                          <span className="px-2 py-0.5 text-xs bg-yellow-100 text-yellow-700 rounded">
                            Overridden
                          </span>
                        )}
                      </div>
                      <p className="text-sm text-gray-500 mt-1">{feature.description}</p>
                      <p className="text-xs text-gray-400 mt-1">Code: {feature.featureCode}</p>
                    </div>
                    <label className="relative inline-flex items-center cursor-pointer ml-4">
                      <input
                        type="checkbox"
                        checked={feature.isEnabled}
                        onChange={async (e) => {
                          const newEnabled = e.target.checked;
                          try {
                            const response = await api.updateTenantFeatures(tenantId, [
                              { featureId: feature.featureId, isEnabled: newEnabled }
                            ]);
                            if (response.success && response.data) {
                              setTenantFeatures(response.data);
                            }
                          } catch (err) {
                            console.error('Failed to update feature');
                          }
                        }}
                        className="sr-only peer"
                      />
                      <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-purple-300 rounded-full peer peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-purple-600"></div>
                    </label>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}

      {activeTab === 'users' && (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="flex items-center justify-between p-6 border-b border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900">Users</h3>
            <button
              onClick={() => setShowOwnerModal(true)}
              className="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 text-sm"
            >
              Create Owner User
            </button>
          </div>
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr className="text-left text-sm text-gray-500">
                <th className="px-6 py-3 font-medium">User</th>
                <th className="px-6 py-3 font-medium">Role</th>
                <th className="px-6 py-3 font-medium">Status</th>
                <th className="px-6 py-3 font-medium">Last Login</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {tenant.users?.map((user) => (
                <tr key={user.userId}>
                  <td className="px-6 py-4">
                    <div>
                      <p className="font-medium text-gray-900">{user.fullName}</p>
                      <p className="text-sm text-gray-500">{user.username}</p>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <span
                      className={`px-2 py-1 text-xs font-medium rounded-full ${
                        user.role === 'Owner'
                          ? 'bg-purple-100 text-purple-700'
                          : user.role === 'Manager'
                          ? 'bg-blue-100 text-blue-700'
                          : 'bg-gray-100 text-gray-700'
                      }`}
                    >
                      {user.role}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <span
                      className={`px-2 py-1 text-xs font-medium rounded-full ${
                        user.isActive ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
                      }`}
                    >
                      {user.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-600">
                    {user.lastLoginAt ? formatDate(user.lastLoginAt.toString()) : 'Never'}
                  </td>
                </tr>
              ))}
              {(!tenant.users || tenant.users.length === 0) && (
                <tr>
                  <td colSpan={4} className="px-6 py-8 text-center text-gray-500">
                    No users found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {activeTab === 'sales' && (
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Sales History (Last 12 Months)</h3>
          <table className="w-full">
            <thead className="border-b border-gray-200">
              <tr className="text-left text-sm text-gray-500">
                <th className="pb-3 font-medium">Month</th>
                <th className="pb-3 font-medium text-right">Shifts</th>
                <th className="pb-3 font-medium text-right">Sales</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {tenant.salesHistory?.map((month) => (
                <tr key={month.month} className="text-sm">
                  <td className="py-3 font-medium text-gray-900">{month.month}</td>
                  <td className="py-3 text-right text-gray-600">{month.shiftCount}</td>
                  <td className="py-3 text-right font-medium text-gray-900">{formatCurrency(month.sales)}</td>
                </tr>
              ))}
              {(!tenant.salesHistory || tenant.salesHistory.length === 0) && (
                <tr>
                  <td colSpan={3} className="py-8 text-center text-gray-500">
                    No sales data available
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* Subscription Modal */}
      {showSubscriptionModal && (
        <SubscriptionModal
          tenant={tenant}
          onClose={() => setShowSubscriptionModal(false)}
          onSuccess={() => {
            setShowSubscriptionModal(false);
            loadTenant();
          }}
        />
      )}

      {/* Owner Modal */}
      {showOwnerModal && (
        <CreateOwnerModal
          tenantId={tenantId}
          onClose={() => setShowOwnerModal(false)}
          onSuccess={() => {
            setShowOwnerModal(false);
            loadTenant();
          }}
        />
      )}
    </div>
  );
}

function SubscriptionModal({
  tenant,
  onClose,
  onSuccess,
}: {
  tenant: TenantDetail;
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [formData, setFormData] = useState<UpdateSubscriptionDto>({
    subscriptionPlan: tenant.subscriptionPlan,
    subscriptionEndDate: tenant.subscriptionEndDate ?? undefined,
    maxMachines: tenant.maxMachines,
    maxWorkers: tenant.maxWorkers,
    maxMonthlyBills: tenant.maxMonthlyBills,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const response = await api.updateTenantSubscription(tenant.tenantId, formData);
      if (response.success) {
        onSuccess();
      } else {
        setError(response.message || 'Failed to update subscription');
      }
    } catch (err) {
      setError('Failed to update subscription');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md mx-4 p-6">
        <h2 className="text-xl font-bold text-gray-900 mb-4">Update Subscription</h2>
        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-600 text-sm">
            {error}
          </div>
        )}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Plan</label>
            <select
              value={formData.subscriptionPlan}
              onChange={(e) => setFormData({ ...formData, subscriptionPlan: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
            >
              <option value="Basic">Basic</option>
              <option value="Premium">Premium</option>
              <option value="Enterprise">Enterprise</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">End Date</label>
            <input
              type="date"
              value={formData.subscriptionEndDate?.split('T')[0] || ''}
              onChange={(e) => setFormData({ ...formData, subscriptionEndDate: e.target.value || undefined })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
            />
          </div>
          <div className="grid grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Max Machines</label>
              <input
                type="number"
                value={formData.maxMachines}
                onChange={(e) => setFormData({ ...formData, maxMachines: parseInt(e.target.value) })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Max Workers</label>
              <input
                type="number"
                value={formData.maxWorkers}
                onChange={(e) => setFormData({ ...formData, maxWorkers: parseInt(e.target.value) })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Max Bills</label>
              <input
                type="number"
                value={formData.maxMonthlyBills}
                onChange={(e) => setFormData({ ...formData, maxMonthlyBills: parseInt(e.target.value) })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
              />
            </div>
          </div>
          <div className="flex justify-end space-x-3 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 text-gray-600 hover:text-gray-900"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 disabled:opacity-50"
            >
              {loading ? 'Saving...' : 'Save Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

function CreateOwnerModal({
  tenantId,
  onClose,
  onSuccess,
}: {
  tenantId: string;
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [formData, setFormData] = useState<CreateOwnerUserDto>({
    username: '',
    password: '',
    fullName: '',
    email: '',
    phone: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const response = await api.createTenantOwner(tenantId, formData);
      if (response.success) {
        onSuccess();
      } else {
        setError(response.message || 'Failed to create owner');
      }
    } catch (err) {
      setError('Failed to create owner');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md mx-4 p-6">
        <h2 className="text-xl font-bold text-gray-900 mb-4">Create Owner User</h2>
        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-600 text-sm">
            {error}
          </div>
        )}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Username</label>
            <input
              type="text"
              value={formData.username}
              onChange={(e) => setFormData({ ...formData, username: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Password</label>
            <input
              type="password"
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Full Name</label>
            <input
              type="text"
              value={formData.fullName}
              onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
            <input
              type="email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Phone</label>
            <input
              type="tel"
              value={formData.phone}
              onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
              required
            />
          </div>
          <div className="flex justify-end space-x-3 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 text-gray-600 hover:text-gray-900"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 disabled:opacity-50"
            >
              {loading ? 'Creating...' : 'Create Owner'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
