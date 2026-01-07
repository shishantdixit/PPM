'use client';

import { Fragment } from 'react';

interface UpgradeModalProps {
  isOpen: boolean;
  onClose: () => void;
  featureName: string;
  currentPlan: string;
  requiredPlan?: string;
}

export function UpgradeModal({
  isOpen,
  onClose,
  featureName,
  currentPlan,
  requiredPlan
}: UpgradeModalProps) {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex min-h-screen items-center justify-center p-4">
        {/* Backdrop */}
        <div
          className="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
          onClick={onClose}
        />

        {/* Modal */}
        <div className="relative bg-white rounded-lg shadow-xl max-w-md w-full p-6 transform transition-all">
          {/* Icon */}
          <div className="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-yellow-100 mb-4">
            <svg
              className="h-6 w-6 text-yellow-600"
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
          </div>

          {/* Content */}
          <div className="text-center">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">
              Upgrade Required
            </h3>
            <p className="text-sm text-gray-600 mb-4">
              <span className="font-medium">{featureName}</span> is not available in your current plan.
            </p>

            {/* Plan info */}
            <div className="bg-gray-50 rounded-lg p-4 mb-4">
              <div className="flex justify-between items-center text-sm">
                <span className="text-gray-500">Current Plan:</span>
                <span className="font-medium text-gray-900">{currentPlan}</span>
              </div>
              {requiredPlan && (
                <div className="flex justify-between items-center text-sm mt-2">
                  <span className="text-gray-500">Required Plan:</span>
                  <span className="font-medium text-green-600">{requiredPlan}+</span>
                </div>
              )}
            </div>

            <p className="text-xs text-gray-500 mb-6">
              Contact your administrator or our sales team to upgrade your subscription.
            </p>
          </div>

          {/* Actions */}
          <div className="flex gap-3">
            <button
              onClick={onClose}
              className="flex-1 px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
            >
              Maybe Later
            </button>
            <button
              onClick={() => {
                // Navigate to subscription page or show contact info
                window.location.href = '/dashboard/subscription';
              }}
              className="flex-1 px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
            >
              View Plans
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
