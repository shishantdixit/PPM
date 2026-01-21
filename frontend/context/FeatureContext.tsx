'use client';

import React, { createContext, useContext, useState, useEffect, useCallback, ReactNode } from 'react';
import { api } from '@/lib/api';
import { useAuth } from './AuthContext';
import type { FeatureAccess, TenantSubscription } from '@/types';

interface FeatureContextType {
  features: Record<string, boolean>;
  subscription: TenantSubscription | null;
  loading: boolean;
  error: string | null;
  hasFeature: (featureCode: string) => boolean;
  refreshFeatures: () => Promise<void>;
  isSuperAdmin: boolean;
}

const FeatureContext = createContext<FeatureContextType | undefined>(undefined);

export function FeatureProvider({ children }: { children: ReactNode }) {
  const { user, isAuthenticated } = useAuth();
  const [features, setFeatures] = useState<Record<string, boolean>>({});
  const [subscription, setSubscription] = useState<TenantSubscription | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const isSuperAdmin = user?.isSuperAdmin === true;

  const loadFeatures = useCallback(async () => {
    if (!isAuthenticated || isSuperAdmin) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const response = await api.getMyFeatureAccess();
      if (response.success && response.data) {
        setFeatures(response.data.features);
      }
    } catch (err: any) {
      // Ignore 401 errors - they occur during auth transitions
      if (err.response?.status === 401) {
        setLoading(false);
        return;
      }
      console.error('Error loading features:', err);
      setError(err.message || 'Failed to load features');
    } finally {
      setLoading(false);
    }
  }, [isAuthenticated, isSuperAdmin]);

  const loadSubscription = useCallback(async () => {
    if (!isAuthenticated || isSuperAdmin || user?.role !== 'Owner') {
      return;
    }

    try {
      const response = await api.getMySubscription();
      if (response.success && response.data) {
        setSubscription(response.data);
      }
    } catch (err: any) {
      // Ignore 401 errors - they occur during auth transitions
      if (err.response?.status === 401) {
        return;
      }
      console.error('Error loading subscription:', err);
    }
  }, [isAuthenticated, isSuperAdmin, user?.role]);

  useEffect(() => {
    if (isAuthenticated && !isSuperAdmin) {
      loadFeatures();
      loadSubscription();
    } else {
      setFeatures({});
      setSubscription(null);
      setLoading(false);
    }
  }, [isAuthenticated, isSuperAdmin, loadFeatures, loadSubscription]);

  const hasFeature = useCallback((featureCode: string): boolean => {
    // Super admin has access to all features
    if (isSuperAdmin) {
      return true;
    }
    return features[featureCode] === true;
  }, [features, isSuperAdmin]);

  const refreshFeatures = useCallback(async () => {
    await loadFeatures();
    await loadSubscription();
  }, [loadFeatures, loadSubscription]);

  const value = {
    features,
    subscription,
    loading,
    error,
    hasFeature,
    refreshFeatures,
    isSuperAdmin,
  };

  return <FeatureContext.Provider value={value}>{children}</FeatureContext.Provider>;
}

export function useFeatures() {
  const context = useContext(FeatureContext);
  if (context === undefined) {
    throw new Error('useFeatures must be used within a FeatureProvider');
  }
  return context;
}

// Convenience hook for subscription/trial info
export function useSubscription() {
  const { subscription, loading, refreshFeatures } = useFeatures();
  return { subscription, loading, refreshSubscription: refreshFeatures };
}
