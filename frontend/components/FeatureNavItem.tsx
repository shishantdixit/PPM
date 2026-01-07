'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useFeatures } from '@/context/FeatureContext';
import { UpgradeModal } from './UpgradeModal';

interface FeatureNavItemProps {
  href: string;
  label: string;
  icon: React.ReactNode;
  featureCode?: string;
  isActive?: boolean;
  requiredPlan?: string;
}

export function FeatureNavItem({
  href,
  label,
  icon,
  featureCode,
  isActive,
  requiredPlan = 'Premium'
}: FeatureNavItemProps) {
  const { hasFeature, subscription, isSuperAdmin } = useFeatures();
  const [showUpgradeModal, setShowUpgradeModal] = useState(false);

  // Check if feature is accessible
  const isEnabled = !featureCode || hasFeature(featureCode) || isSuperAdmin;
  const currentPlan = subscription?.subscriptionPlan || 'Basic';

  const baseClasses = `flex items-center gap-3 px-3 py-2 rounded-lg transition-colors`;
  const activeClasses = isActive
    ? 'bg-blue-50 text-blue-700'
    : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900';
  const lockedClasses = !isEnabled ? 'opacity-60' : '';

  const handleClick = (e: React.MouseEvent) => {
    if (!isEnabled) {
      e.preventDefault();
      setShowUpgradeModal(true);
    }
  };

  return (
    <>
      <Link
        href={isEnabled ? href : '#'}
        onClick={handleClick}
        className={`${baseClasses} ${activeClasses} ${lockedClasses}`}
      >
        <span className="w-5 h-5">{icon}</span>
        <span className="flex-1">{label}</span>
        {!isEnabled && (
          <svg
            className="w-4 h-4 text-gray-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
            />
          </svg>
        )}
      </Link>

      <UpgradeModal
        isOpen={showUpgradeModal}
        onClose={() => setShowUpgradeModal(false)}
        featureName={label}
        currentPlan={currentPlan}
        requiredPlan={requiredPlan}
      />
    </>
  );
}
