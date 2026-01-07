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
      case 'Expired':
        return 'bg-red-100 text-red-800';
      case 'Suspended':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getDaysRemaining = () => {
    if (!subscription.subscriptionEndDate) return null;
    const endDate = new Date(subscription.subscriptionEndDate);
    const today = new Date();
    const diffTime = endDate.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  };

  const daysRemaining = getDaysRemaining();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Subscription</h1>
        <p className="text-gray-600">View your current subscription plan and features</p>
      </div>

      {/* Current Plan Card */}
      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex items-start justify-between">
          <div>
            <h2 className="text-lg font-semibold text-gray-900">Current Plan</h2>
            <div className="mt-2 flex items-center gap-3">
              <span className="text-3xl font-bold text-blue-600">{subscription.subscriptionPlan}</span>
              <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(subscription.subscriptionStatus)}`}>
                {subscription.subscriptionStatus}
              </span>
            </div>
          </div>
          {daysRemaining !== null && daysRemaining > 0 && (
            <div className="text-right">
              <div className="text-sm text-gray-500">Days Remaining</div>
              <div className={`text-2xl font-bold ${daysRemaining <= 7 ? 'text-red-600' : daysRemaining <= 30 ? 'text-yellow-600' : 'text-green-600'}`}>
                {daysRemaining}
              </div>
            </div>
          )}
        </div>

        <div className="mt-6 grid grid-cols-2 gap-4 border-t pt-4">
          <div>
            <div className="text-sm text-gray-500">Start Date</div>
            <div className="font-medium">{formatDate(subscription.subscriptionStartDate)}</div>
          </div>
          <div>
            <div className="text-sm text-gray-500">End Date</div>
            <div className="font-medium">{formatDate(subscription.subscriptionEndDate)}</div>
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
        <h2 className="text-lg font-semibold mb-2">Need More Features?</h2>
        <p className="text-blue-100 mb-4">
          Contact our sales team to upgrade your subscription and unlock additional features.
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
