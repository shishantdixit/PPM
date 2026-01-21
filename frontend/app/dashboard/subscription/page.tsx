'use client';

import { useEffect, useState } from 'react';
import { useAuth } from '@/context/AuthContext';
import { useFeatures } from '@/context/FeatureContext';
import { api } from '@/lib/api';
import type { TenantSubscription, TenantFeature } from '@/types';

const planFeatures: Record<string, string[]> = {
  Basic: ['Dashboard', 'My Shift', 'Fuel Management', 'Machine & Nozzle Setup'],
  Premium: ['Everything in Basic', 'Reports & Analytics', 'Credit Customer Management', 'Expense Tracking', 'Multiple Shifts Per Day', 'Data Export (PDF/Excel)'],
  Enterprise: ['Everything in Premium', 'API Access', 'Advanced Analytics', 'Bulk Import/Export', 'Priority Support']
};

export default function SubscriptionPage() {
  const { user } = useAuth();
  const { subscription: contextSubscription, refreshFeatures } = useFeatures();
  const [subscription, setSubscription] = useState<TenantSubscription | null>(null);
  const [features, setFeatures] = useState<TenantFeature[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // License key activation state
  const [licenseKey, setLicenseKey] = useState('');
  const [activating, setActivating] = useState(false);
  const [activationError, setActivationError] = useState<string | null>(null);
  const [activationSuccess, setActivationSuccess] = useState<string | null>(null);

  useEffect(() => {
    const fetchSubscription = async () => {
      try {
        setLoading(true);
        const response = await api.getMySubscription();
        if (response.success && response.data) {
          setSubscription(response.data);
          setFeatures(response.data.features);
        }
      } catch (err: any) {
        setError(err.response?.data?.message || 'Failed to load subscription details');
      } finally {
        setLoading(false);
      }
    };

    if (user?.role === 'Owner') {
      fetchSubscription();
    }
  }, [user]);

  // Format license key as user types
  const handleLicenseKeyChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let value = e.target.value.toUpperCase().replace(/[^A-Z0-9]/g, '');

    // Add PPM- prefix if not present
    if (!value.startsWith('PPM')) {
      if (value.length > 0 && !['P', 'PP', 'PPM'].includes(value.substring(0, Math.min(3, value.length)))) {
        value = 'PPM' + value;
      }
    }

    // Format with dashes
    let formatted = '';
    const segments = value.replace('PPM', '').match(/.{1,4}/g) || [];
    if (value.startsWith('PPM')) {
      formatted = 'PPM';
      segments.forEach((segment, index) => {
        formatted += '-' + segment;
      });
    } else {
      formatted = value;
    }

    // Limit to PPM-XXXX-XXXX-XXXX-XXXX format (23 chars)
    setLicenseKey(formatted.substring(0, 23));
    setActivationError(null);
    setActivationSuccess(null);
  };

  const handleActivateLicense = async (e: React.FormEvent) => {
    e.preventDefault();
    setActivationError(null);
    setActivationSuccess(null);

    // Validate format
    const keyPattern = /^PPM-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$/;
    if (!keyPattern.test(licenseKey)) {
      setActivationError('Invalid license key format. Expected: PPM-XXXX-XXXX-XXXX-XXXX');
      return;
    }

    setActivating(true);
    try {
      const response = await api.activateLicenseKey(licenseKey);
      if (response.success && response.data) {
        setActivationSuccess(`License activated! Your subscription is now ${response.data.newSubscriptionPlan} until ${new Date(response.data.newSubscriptionEndDate!).toLocaleDateString()}`);
        setLicenseKey('');
        // Refresh subscription data
        const subResponse = await api.getMySubscription();
        if (subResponse.success && subResponse.data) {
          setSubscription(subResponse.data);
          setFeatures(subResponse.data.features);
        }
        // Refresh features context
        refreshFeatures();
      } else {
        setActivationError(response.message || 'Failed to activate license key');
      }
    } catch (err: any) {
      setActivationError(err.response?.data?.message || 'Failed to activate license key');
    } finally {
      setActivating(false);
    }
  };

  if (user?.role !== 'Owner') {
    return (
      <div className="p-6">
        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
          <p className="text-yellow-800">Only owners can view subscription details.</p>
        </div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-gray-500">Loading subscription details...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-6">
        <div className="bg-red-50 border border-red-200 rounded-lg p-4">
          <p className="text-red-800">{error}</p>
        </div>
      </div>
    );
  }

  if (!subscription) {
    return (
      <div className="p-6">
        <div className="bg-gray-50 border border-gray-200 rounded-lg p-4">
          <p className="text-gray-800">No subscription information available.</p>
        </div>
      </div>
    );
  }

  const formatDate = (dateString: string | null) => {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Active':
        return 'bg-green-100 text-green-800';
      case 'Trial':
        return 'bg-blue-100 text-blue-800';
      case 'Expired':
        return 'bg-red-100 text-red-800';
      case 'Suspended':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getDaysRemaining = () => {
    if (subscription.isTrial && subscription.trialDaysRemaining !== null) {
      return subscription.trialDaysRemaining;
    }
    if (!subscription.subscriptionEndDate) return null;
    const endDate = new Date(subscription.subscriptionEndDate);
    const today = new Date();
    const diffTime = endDate.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  };

  const daysRemaining = getDaysRemaining();
  const isTrialOrExpired = subscription.isTrial || subscription.subscriptionStatus === 'Expired' || subscription.subscriptionStatus === 'Trial';

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Subscription</h1>
        <p className="text-gray-600">View your current subscription plan and features</p>
      </div>

      {/* Trial Banner */}
      {subscription.isTrial && (
        <div className={`rounded-lg p-4 ${daysRemaining && daysRemaining <= 2 ? 'bg-red-50 border border-red-200' : 'bg-blue-50 border border-blue-200'}`}>
          <div className="flex items-center gap-3">
            <div className={`p-2 rounded-full ${daysRemaining && daysRemaining <= 2 ? 'bg-red-100' : 'bg-blue-100'}`}>
              <svg className={`w-6 h-6 ${daysRemaining && daysRemaining <= 2 ? 'text-red-600' : 'text-blue-600'}`} fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div>
              <h3 className={`font-semibold ${daysRemaining && daysRemaining <= 2 ? 'text-red-800' : 'text-blue-800'}`}>
                {daysRemaining && daysRemaining > 0
                  ? `Free Trial - ${daysRemaining} ${daysRemaining === 1 ? 'day' : 'days'} remaining`
                  : 'Free Trial Expired'}
              </h3>
              <p className={`text-sm ${daysRemaining && daysRemaining <= 2 ? 'text-red-600' : 'text-blue-600'}`}>
                {daysRemaining && daysRemaining > 0
                  ? 'Activate a license key below to continue using all features after your trial ends.'
                  : 'Please activate a license key to continue using PPM.'}
              </p>
            </div>
          </div>
        </div>
      )}

      {/* License Key Activation Section */}
      {isTrialOrExpired && (
        <div className="bg-white rounded-lg shadow p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Activate License Key</h2>
          <p className="text-gray-600 mb-4">
            Enter your license key to activate or upgrade your subscription. Contact sales to purchase a license key.
          </p>

          <form onSubmit={handleActivateLicense} className="space-y-4">
            <div>
              <label htmlFor="licenseKey" className="block text-sm font-medium text-gray-700 mb-1">
                License Key
              </label>
              <div className="flex gap-3">
                <input
                  type="text"
                  id="licenseKey"
                  value={licenseKey}
                  onChange={handleLicenseKeyChange}
                  placeholder="PPM-XXXX-XXXX-XXXX-XXXX"
                  className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent font-mono text-lg tracking-wider"
                  maxLength={23}
                />
                <button
                  type="submit"
                  disabled={activating || licenseKey.length < 23}
                  className="px-6 py-2 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
                >
                  {activating ? 'Activating...' : 'Activate'}
                </button>
              </div>
            </div>

            {activationError && (
              <div className="p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
                {activationError}
              </div>
            )}

            {activationSuccess && (
              <div className="p-3 bg-green-50 border border-green-200 rounded-lg text-green-700 text-sm">
                {activationSuccess}
              </div>
            )}
          </form>
        </div>
      )}

      {/* Current Plan Card */}
      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex items-start justify-between">
          <div>
            <h2 className="text-lg font-semibold text-gray-900">Current Plan</h2>
            <div className="mt-2 flex items-center gap-3">
              <span className="text-3xl font-bold text-blue-600">{subscription.subscriptionPlan}</span>
              <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(subscription.subscriptionStatus)}`}>
                {subscription.isTrial ? 'Trial' : subscription.subscriptionStatus}
              </span>
            </div>
          </div>
          {daysRemaining != null && daysRemaining > 0 && (
            <div className="text-right">
              <div className="text-sm text-gray-500">{subscription.isTrial ? 'Trial' : ''} Days Remaining</div>
              <div className={`text-2xl font-bold ${daysRemaining <= 2 ? 'text-red-600' : daysRemaining <= 7 ? 'text-yellow-600' : 'text-green-600'}`}>
                {daysRemaining}
              </div>
            </div>
          )}
        </div>

        <div className="mt-6 grid grid-cols-2 gap-4 border-t pt-4">
          <div>
            <div className="text-sm text-gray-500">{subscription.isTrial ? 'Trial Start Date' : 'Start Date'}</div>
            <div className="font-medium">{formatDate(subscription.isTrial ? subscription.trialStartDate ?? null : subscription.subscriptionStartDate)}</div>
          </div>
          <div>
            <div className="text-sm text-gray-500">{subscription.isTrial ? 'Trial End Date' : 'End Date'}</div>
            <div className="font-medium">{formatDate(subscription.isTrial ? subscription.trialEndDate ?? null : subscription.subscriptionEndDate)}</div>
          </div>
        </div>

        {/* Usage Limits */}
        <div className="mt-4 border-t pt-4">
          <h3 className="text-sm font-medium text-gray-700 mb-3">Usage Limits</h3>
          <div className="grid grid-cols-3 gap-4">
            <div className="bg-gray-50 rounded-lg p-3 text-center">
              <div className="text-2xl font-bold text-gray-900">{subscription.maxMachines}</div>
              <div className="text-xs text-gray-500">Max Machines</div>
            </div>
            <div className="bg-gray-50 rounded-lg p-3 text-center">
              <div className="text-2xl font-bold text-gray-900">{subscription.maxWorkers}</div>
              <div className="text-xs text-gray-500">Max Workers</div>
            </div>
            <div className="bg-gray-50 rounded-lg p-3 text-center">
              <div className="text-2xl font-bold text-gray-900">{subscription.maxMonthlyBills.toLocaleString()}</div>
              <div className="text-xs text-gray-500">Monthly Bills</div>
            </div>
          </div>
        </div>
      </div>

      {/* Your Features */}
      <div className="bg-white rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Your Features</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          {features.map((feature) => (
            <div
              key={feature.featureId}
              className={`flex items-center gap-3 p-3 rounded-lg ${feature.isEnabled ? 'bg-green-50' : 'bg-gray-50'}`}
            >
              {feature.isEnabled ? (
                <svg className="w-5 h-5 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                </svg>
              ) : (
                <svg className="w-5 h-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                </svg>
              )}
              <div>
                <div className={`font-medium ${feature.isEnabled ? 'text-gray-900' : 'text-gray-500'}`}>
                  {feature.featureName}
                </div>
                <div className="text-xs text-gray-500">{feature.description}</div>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Plan Comparison */}
      <div className="bg-white rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Plan Comparison</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {Object.entries(planFeatures).map(([plan, featureList]) => (
            <div
              key={plan}
              className={`border rounded-lg p-4 ${plan === subscription.subscriptionPlan ? 'border-blue-500 bg-blue-50' : 'border-gray-200'}`}
            >
              <div className="flex items-center justify-between mb-3">
                <h3 className="font-semibold text-lg">{plan}</h3>
                {plan === subscription.subscriptionPlan && (
                  <span className="text-xs bg-blue-600 text-white px-2 py-1 rounded">Current</span>
                )}
              </div>
              <ul className="space-y-2">
                {featureList.map((feature, index) => (
                  <li key={index} className="flex items-start gap-2 text-sm">
                    <svg className="w-4 h-4 text-green-600 mt-0.5 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                    </svg>
                    <span className="text-gray-600">{feature}</span>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>
      </div>

      {/* Contact for Upgrade */}
      <div className="bg-gradient-to-r from-blue-600 to-blue-700 rounded-lg shadow p-6 text-white">
        <h2 className="text-lg font-semibold mb-2">Need a License Key?</h2>
        <p className="text-blue-100 mb-4">
          Contact our sales team to purchase a license key and unlock all features.
        </p>
        <div className="flex flex-wrap gap-4 text-sm">
          <div className="flex items-center gap-2">
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
            </svg>
            <span>sales@ppm.example.com</span>
          </div>
          <div className="flex items-center gap-2">
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
            </svg>
            <span>+1 (555) 123-4567</span>
          </div>
        </div>
      </div>
    </div>
  );
}
