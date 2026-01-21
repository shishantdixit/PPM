'use client';

import { useSubscription } from '@/context/FeatureContext';
import Link from 'next/link';

export default function TrialBanner() {
  const { subscription, loading } = useSubscription();

  if (loading || !subscription) {
    return null;
  }

  // Don't show banner for non-trial users or if subscription is active
  if (!subscription.isTrial || subscription.subscriptionStatus === 'Active') {
    return null;
  }

  const daysRemaining = subscription.trialDaysRemaining ?? 0;
  const isExpired = daysRemaining <= 0;
  const isUrgent = daysRemaining <= 2;

  // Banner colors based on urgency
  const bannerClass = isExpired
    ? 'bg-red-600 text-white'
    : isUrgent
    ? 'bg-orange-500 text-white'
    : 'bg-blue-500 text-white';

  const getMessage = () => {
    if (isExpired) {
      return 'Your free trial has expired. Activate a license key to continue using PPM.';
    }
    if (daysRemaining === 1) {
      return 'Your free trial expires tomorrow! Activate a license key to continue using PPM.';
    }
    return `Your free trial ends in ${daysRemaining} days. Activate a license key to unlock all features.`;
  };

  return (
    <div className={`${bannerClass} px-4 py-2 text-sm`}>
      <div className="max-w-7xl mx-auto flex items-center justify-between">
        <div className="flex items-center gap-2">
          {isExpired ? (
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
          ) : isUrgent ? (
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          ) : (
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          )}
          <span>{getMessage()}</span>
        </div>
        <Link
          href="/dashboard/subscription"
          className={`px-3 py-1 rounded text-sm font-medium transition-colors ${
            isExpired
              ? 'bg-white text-red-600 hover:bg-red-50'
              : isUrgent
              ? 'bg-white text-orange-600 hover:bg-orange-50'
              : 'bg-white text-blue-600 hover:bg-blue-50'
          }`}
        >
          {isExpired ? 'Activate Now' : 'Upgrade'}
        </Link>
      </div>
    </div>
  );
}
