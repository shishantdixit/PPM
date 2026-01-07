'use client';

import { useAuth } from '@/context/AuthContext';

export default function DashboardPage() {
  const { user } = useAuth();

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Dashboard</h1>

      <div className="bg-white rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold mb-4">Welcome, {user?.fullName}!</h2>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div className="bg-blue-50 p-4 rounded-lg">
            <div className="text-sm text-blue-600 font-medium">Role</div>
            <div className="text-2xl font-bold text-blue-900">{user?.role}</div>
          </div>

          <div className="bg-green-50 p-4 rounded-lg">
            <div className="text-sm text-green-600 font-medium">Tenant</div>
            <div className="text-2xl font-bold text-green-900">{user?.tenantCode || 'N/A'}</div>
          </div>

          <div className="bg-purple-50 p-4 rounded-lg">
            <div className="text-sm text-purple-600 font-medium">Username</div>
            <div className="text-xl font-bold text-purple-900">{user?.username}</div>
          </div>

          <div className="bg-orange-50 p-4 rounded-lg">
            <div className="text-sm text-orange-600 font-medium">Status</div>
            <div className="text-2xl font-bold text-orange-900">Active</div>
          </div>
        </div>

        <div className="mt-6 p-4 bg-gray-50 rounded-lg">
          <h3 className="font-semibold mb-2">Quick Links</h3>
          <ul className="space-y-2 text-sm text-gray-600">
            <li>• Manage fuel types and current rates</li>
            <li>• Configure machines and nozzles</li>
            <li>• Track fuel dispensing and sales</li>
            <li>• Generate reports and analytics</li>
          </ul>
        </div>
      </div>
    </div>
  );
}
