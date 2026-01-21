'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { api } from '@/lib/api';

interface GeneratedKey {
  key: string;
  subscriptionPlan: string;
  durationMonths: number;
}

const planDefaults = {
  Basic: { maxMachines: 5, maxWorkers: 20, maxMonthlyBills: 10000 },
  Premium: { maxMachines: 20, maxWorkers: 100, maxMonthlyBills: 100000 },
  Enterprise: { maxMachines: 100, maxWorkers: 500, maxMonthlyBills: 1000000 },
};

export default function GenerateLicenseKeyPage() {
  const router = useRouter();
  const [mode, setMode] = useState<'single' | 'batch'>('single');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [generatedKeys, setGeneratedKeys] = useState<GeneratedKey[]>([]);

  const [formData, setFormData] = useState({
    subscriptionPlan: 'Basic',
    durationMonths: 12,
    maxMachines: 5,
    maxWorkers: 20,
    maxMonthlyBills: 10000,
    notes: '',
    count: 1,
  });

  const handlePlanChange = (plan: string) => {
    const defaults = planDefaults[plan as keyof typeof planDefaults];
    setFormData(prev => ({
      ...prev,
      subscriptionPlan: plan,
      maxMachines: defaults.maxMachines,
      maxWorkers: defaults.maxWorkers,
      maxMonthlyBills: defaults.maxMonthlyBills,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setGeneratedKeys([]);
    setLoading(true);

    try {
      if (mode === 'single') {
        const response = await api.generateLicenseKey({
          subscriptionPlan: formData.subscriptionPlan,
          durationMonths: formData.durationMonths,
          maxMachines: formData.maxMachines,
          maxWorkers: formData.maxWorkers,
          maxMonthlyBills: formData.maxMonthlyBills,
          notes: formData.notes || undefined,
        });
        if (response.success && response.data) {
          setGeneratedKeys([{
            key: response.data.key,
            subscriptionPlan: response.data.subscriptionPlan,
            durationMonths: response.data.durationMonths,
          }]);
        } else {
          setError(response.message || 'Failed to generate license key');
        }
      } else {
        const response = await api.generateBatchLicenseKeys({
          count: formData.count,
          subscriptionPlan: formData.subscriptionPlan,
          durationMonths: formData.durationMonths,
          maxMachines: formData.maxMachines,
          maxWorkers: formData.maxWorkers,
          maxMonthlyBills: formData.maxMonthlyBills,
          notes: formData.notes || undefined,
        });
        if (response.success && response.data) {
          setGeneratedKeys(response.data.map((k: any) => ({
            key: k.key,
            subscriptionPlan: k.subscriptionPlan,
            durationMonths: k.durationMonths,
          })));
        } else {
          setError(response.message || 'Failed to generate license keys');
        }
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to generate license key(s)');
    } finally {
      setLoading(false);
    }
  };

  const copyAllKeys = () => {
    const keysText = generatedKeys.map(k => k.key).join('\n');
    navigator.clipboard.writeText(keysText);
  };

  const copyKey = (key: string) => {
    navigator.clipboard.writeText(key);
  };

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div className="flex items-center gap-4">
        <Link
          href="/admin/license-keys"
          className="text-gray-600 hover:text-gray-900"
        >
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Generate License Keys</h1>
          <p className="text-gray-600">Create new license keys for tenant subscriptions</p>
        </div>
      </div>

      {/* Mode Toggle */}
      <div className="bg-white rounded-lg shadow p-4">
        <div className="flex gap-4">
          <button
            onClick={() => setMode('single')}
            className={`flex-1 py-3 px-4 rounded-lg font-medium transition-colors ${
              mode === 'single'
                ? 'bg-purple-100 text-purple-700 border-2 border-purple-500'
                : 'bg-gray-100 text-gray-600 hover:bg-gray-200 border-2 border-transparent'
            }`}
          >
            Single Key
          </button>
          <button
            onClick={() => setMode('batch')}
            className={`flex-1 py-3 px-4 rounded-lg font-medium transition-colors ${
              mode === 'batch'
                ? 'bg-purple-100 text-purple-700 border-2 border-purple-500'
                : 'bg-gray-100 text-gray-600 hover:bg-gray-200 border-2 border-transparent'
            }`}
          >
            Batch Generate
          </button>
        </div>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow p-6 space-y-6">
        {/* Plan Selection */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-3">Subscription Plan</label>
          <div className="grid grid-cols-3 gap-4">
            {Object.keys(planDefaults).map((plan) => (
              <button
                key={plan}
                type="button"
                onClick={() => handlePlanChange(plan)}
                className={`p-4 rounded-lg border-2 text-left transition-colors ${
                  formData.subscriptionPlan === plan
                    ? 'border-purple-500 bg-purple-50'
                    : 'border-gray-200 hover:border-gray-300'
                }`}
              >
                <div className="font-semibold text-gray-900">{plan}</div>
                <div className="text-xs text-gray-500 mt-1">
                  {planDefaults[plan as keyof typeof planDefaults].maxMachines} machines,{' '}
                  {planDefaults[plan as keyof typeof planDefaults].maxWorkers} workers
                </div>
              </button>
            ))}
          </div>
        </div>

        {/* Duration */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Duration (months)</label>
          <select
            value={formData.durationMonths}
            onChange={(e) => setFormData(prev => ({ ...prev, durationMonths: parseInt(e.target.value) }))}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
          >
            <option value={1}>1 month</option>
            <option value={3}>3 months</option>
            <option value={6}>6 months</option>
            <option value={12}>12 months</option>
            <option value={24}>24 months</option>
            <option value={36}>36 months</option>
          </select>
        </div>

        {/* Batch Count */}
        {mode === 'batch' && (
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Number of Keys</label>
            <input
              type="number"
              min={1}
              max={100}
              value={formData.count}
              onChange={(e) => setFormData(prev => ({ ...prev, count: parseInt(e.target.value) || 1 }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            />
            <p className="text-xs text-gray-500 mt-1">Maximum 100 keys per batch</p>
          </div>
        )}

        {/* Custom Limits */}
        <details className="group">
          <summary className="cursor-pointer text-sm font-medium text-purple-600 hover:text-purple-700">
            + Customize Limits (optional)
          </summary>
          <div className="mt-4 grid grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Max Machines</label>
              <input
                type="number"
                min={1}
                max={100}
                value={formData.maxMachines}
                onChange={(e) => setFormData(prev => ({ ...prev, maxMachines: parseInt(e.target.value) || 1 }))}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Max Workers</label>
              <input
                type="number"
                min={1}
                max={500}
                value={formData.maxWorkers}
                onChange={(e) => setFormData(prev => ({ ...prev, maxWorkers: parseInt(e.target.value) || 1 }))}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Max Monthly Bills</label>
              <input
                type="number"
                min={100}
                max={1000000}
                value={formData.maxMonthlyBills}
                onChange={(e) => setFormData(prev => ({ ...prev, maxMonthlyBills: parseInt(e.target.value) || 100 }))}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
              />
            </div>
          </div>
        </details>

        {/* Notes */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Notes (optional)</label>
          <textarea
            value={formData.notes}
            onChange={(e) => setFormData(prev => ({ ...prev, notes: e.target.value }))}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
            rows={2}
            placeholder="Internal notes about this key..."
          />
        </div>

        {error && (
          <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700">
            {error}
          </div>
        )}

        <button
          type="submit"
          disabled={loading}
          className="w-full py-3 bg-purple-600 text-white font-semibold rounded-lg hover:bg-purple-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
        >
          {loading ? 'Generating...' : mode === 'single' ? 'Generate Key' : `Generate ${formData.count} Keys`}
        </button>
      </form>

      {/* Generated Keys */}
      {generatedKeys.length > 0 && (
        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-900">
              Generated {generatedKeys.length === 1 ? 'Key' : 'Keys'}
            </h2>
            {generatedKeys.length > 1 && (
              <button
                onClick={copyAllKeys}
                className="text-purple-600 hover:text-purple-700 text-sm font-medium flex items-center gap-1"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                </svg>
                Copy All
              </button>
            )}
          </div>

          <div className="space-y-2">
            {generatedKeys.map((key, index) => (
              <div
                key={index}
                className="flex items-center justify-between p-3 bg-green-50 border border-green-200 rounded-lg"
              >
                <div className="font-mono text-lg text-gray-900">{key.key}</div>
                <button
                  onClick={() => copyKey(key.key)}
                  className="text-gray-600 hover:text-gray-900 p-2"
                  title="Copy key"
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                  </svg>
                </button>
              </div>
            ))}
          </div>

          <div className="mt-4 pt-4 border-t border-gray-200">
            <div className="text-sm text-gray-500">
              <span className="font-medium">Plan:</span> {generatedKeys[0].subscriptionPlan} |{' '}
              <span className="font-medium">Duration:</span> {generatedKeys[0].durationMonths} months
            </div>
          </div>

          <div className="mt-4 flex gap-3">
            <button
              onClick={() => setGeneratedKeys([])}
              className="flex-1 py-2 bg-gray-100 text-gray-700 font-medium rounded-lg hover:bg-gray-200 transition-colors"
            >
              Generate More
            </button>
            <Link
              href="/admin/license-keys"
              className="flex-1 py-2 bg-purple-600 text-white font-medium rounded-lg hover:bg-purple-700 transition-colors text-center"
            >
              View All Keys
            </Link>
          </div>
        </div>
      )}
    </div>
  );
}
