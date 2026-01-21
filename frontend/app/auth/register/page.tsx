'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { api } from '@/lib/api';

export default function RegisterPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<{
    tenantCode: string;
    username: string;
    trialDaysRemaining: number;
  } | null>(null);

  const [formData, setFormData] = useState({
    companyName: '',
    ownerName: '',
    email: '',
    phone: '',
    tenantCode: '',
    address: '',
    city: '',
    state: '',
    pinCode: '',
    username: '',
    password: '',
    confirmPassword: '',
  });

  const [validation, setValidation] = useState({
    emailAvailable: true,
    emailChecking: false,
    tenantCodeAvailable: true,
    tenantCodeChecking: false,
  });

  // Check email availability
  useEffect(() => {
    if (!formData.email || !formData.email.includes('@')) return;

    const timer = setTimeout(async () => {
      setValidation(v => ({ ...v, emailChecking: true }));
      try {
        const response = await api.checkEmailAvailability(formData.email);
        setValidation(v => ({
          ...v,
          emailAvailable: response.data?.isAvailable ?? true,
          emailChecking: false,
        }));
      } catch {
        setValidation(v => ({ ...v, emailChecking: false }));
      }
    }, 500);

    return () => clearTimeout(timer);
  }, [formData.email]);

  // Check tenant code availability
  useEffect(() => {
    if (!formData.tenantCode || formData.tenantCode.length < 3) return;

    const timer = setTimeout(async () => {
      setValidation(v => ({ ...v, tenantCodeChecking: true }));
      try {
        const response = await api.checkTenantCodeAvailability(formData.tenantCode);
        setValidation(v => ({
          ...v,
          tenantCodeAvailable: response.data?.isAvailable ?? true,
          tenantCodeChecking: false,
        }));
      } catch {
        setValidation(v => ({ ...v, tenantCodeChecking: false }));
      }
    }, 500);

    return () => clearTimeout(timer);
  }, [formData.tenantCode]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    setError(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    // Validation
    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (formData.password.length < 6) {
      setError('Password must be at least 6 characters');
      return;
    }

    if (!validation.emailAvailable) {
      setError('Email is already registered');
      return;
    }

    if (formData.tenantCode && !validation.tenantCodeAvailable) {
      setError('Tenant code is already taken');
      return;
    }

    setLoading(true);

    try {
      const response = await api.register({
        companyName: formData.companyName,
        ownerName: formData.ownerName,
        email: formData.email,
        phone: formData.phone,
        tenantCode: formData.tenantCode || undefined,
        address: formData.address || undefined,
        city: formData.city || undefined,
        state: formData.state || undefined,
        pinCode: formData.pinCode || undefined,
        username: formData.username,
        password: formData.password,
      });

      if (response.success && response.data) {
        setSuccess({
          tenantCode: response.data.tenantCode,
          username: response.data.username,
          trialDaysRemaining: response.data.trialDaysRemaining,
        });
      } else {
        setError(response.message || 'Registration failed');
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  // Success screen
  if (success) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-green-50 to-emerald-100 p-4">
        <div className="max-w-md w-full bg-white rounded-xl shadow-2xl p-8 text-center">
          <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
            </svg>
          </div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Registration Successful!</h2>
          <p className="text-gray-600 mb-6">
            Your 7-day free trial has started. You have {success.trialDaysRemaining} days to explore all features.
          </p>

          <div className="bg-gray-50 rounded-lg p-4 mb-6 text-left">
            <h3 className="font-semibold text-gray-900 mb-2">Your Login Credentials:</h3>
            <p className="text-sm text-gray-600">
              <span className="font-medium">Tenant Code:</span>{' '}
              <span className="font-mono bg-blue-100 px-2 py-0.5 rounded">{success.tenantCode}</span>
            </p>
            <p className="text-sm text-gray-600 mt-1">
              <span className="font-medium">Username:</span>{' '}
              <span className="font-mono bg-blue-100 px-2 py-0.5 rounded">{success.username}</span>
            </p>
          </div>

          <Link
            href="/auth/login"
            className="w-full inline-flex justify-center items-center px-6 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition-colors"
          >
            Continue to Login
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 p-4">
      <div className="max-w-2xl w-full bg-white rounded-xl shadow-2xl p-8">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Start Your Free Trial</h1>
          <p className="text-gray-600 mt-2">
            7 days free, no credit card required
          </p>
        </div>

        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Company Information */}
          <div>
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Company Information</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Company Name <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="companyName"
                  value={formData.companyName}
                  onChange={handleChange}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Your Petrol Pump Name"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Owner Name <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="ownerName"
                  value={formData.ownerName}
                  onChange={handleChange}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Full Name"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Email <span className="text-red-500">*</span>
                </label>
                <div className="relative">
                  <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleChange}
                    required
                    className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                      formData.email && !validation.emailAvailable ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="email@example.com"
                  />
                  {validation.emailChecking && (
                    <span className="absolute right-3 top-2.5 text-gray-400 text-sm">Checking...</span>
                  )}
                </div>
                {formData.email && !validation.emailAvailable && (
                  <p className="text-red-500 text-sm mt-1">Email is already registered</p>
                )}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Phone <span className="text-red-500">*</span>
                </label>
                <input
                  type="tel"
                  name="phone"
                  value={formData.phone}
                  onChange={handleChange}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="9876543210"
                />
              </div>
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Tenant Code <span className="text-gray-400">(optional)</span>
                </label>
                <div className="relative">
                  <input
                    type="text"
                    name="tenantCode"
                    value={formData.tenantCode}
                    onChange={(e) => setFormData(prev => ({ ...prev, tenantCode: e.target.value.toUpperCase() }))}
                    className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent font-mono ${
                      formData.tenantCode && !validation.tenantCodeAvailable ? 'border-red-500' : 'border-gray-300'
                    }`}
                    placeholder="Auto-generated if left empty"
                  />
                  {validation.tenantCodeChecking && (
                    <span className="absolute right-3 top-2.5 text-gray-400 text-sm">Checking...</span>
                  )}
                </div>
                {formData.tenantCode && !validation.tenantCodeAvailable && (
                  <p className="text-red-500 text-sm mt-1">Tenant code is already taken</p>
                )}
                <p className="text-gray-500 text-xs mt-1">This will be your unique login identifier</p>
              </div>
            </div>
          </div>

          {/* Address (collapsible) */}
          <details className="group">
            <summary className="cursor-pointer text-sm font-medium text-blue-600 hover:text-blue-700">
              + Add Address (optional)
            </summary>
            <div className="mt-4 grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Address</label>
                <input
                  type="text"
                  name="address"
                  value={formData.address}
                  onChange={handleChange}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Street address"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">City</label>
                <input
                  type="text"
                  name="city"
                  value={formData.city}
                  onChange={handleChange}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">State</label>
                <input
                  type="text"
                  name="state"
                  value={formData.state}
                  onChange={handleChange}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">PIN Code</label>
                <input
                  type="text"
                  name="pinCode"
                  value={formData.pinCode}
                  onChange={handleChange}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            </div>
          </details>

          {/* Login Credentials */}
          <div>
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Login Credentials</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Username <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="username"
                  value={formData.username}
                  onChange={(e) => setFormData(prev => ({ ...prev, username: e.target.value.toLowerCase() }))}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Choose a username"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Password <span className="text-red-500">*</span>
                </label>
                <input
                  type="password"
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  required
                  minLength={6}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="At least 6 characters"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Confirm Password <span className="text-red-500">*</span>
                </label>
                <input
                  type="password"
                  name="confirmPassword"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  required
                  className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                    formData.confirmPassword && formData.password !== formData.confirmPassword
                      ? 'border-red-500'
                      : 'border-gray-300'
                  }`}
                  placeholder="Confirm your password"
                />
                {formData.confirmPassword && formData.password !== formData.confirmPassword && (
                  <p className="text-red-500 text-sm mt-1">Passwords do not match</p>
                )}
              </div>
            </div>
          </div>

          {/* Submit */}
          <button
            type="submit"
            disabled={loading || !validation.emailAvailable || (formData.tenantCode.length > 0 && !validation.tenantCodeAvailable)}
            className="w-full py-3 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
          >
            {loading ? 'Creating Account...' : 'Start Free Trial'}
          </button>
        </form>

        <p className="mt-6 text-center text-gray-600">
          Already have an account?{' '}
          <Link href="/auth/login" className="text-blue-600 hover:text-blue-700 font-medium">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  );
}
