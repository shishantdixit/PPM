'use client';

import { useAuth } from '@/context/AuthContext';
import { useFeatures } from '@/context/FeatureContext';
import { FeatureNavItem } from '@/components/FeatureNavItem';
import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { ReactNode, useEffect, useState } from 'react';
import { UpgradeModal } from '@/components/UpgradeModal';
import TrialBanner from '@/components/TrialBanner';

interface NavItem {
  name: string;
  href: string;
  icon: string;
  roles: string[];
  featureCode?: string;
  requiredPlan?: string;
}

export default function DashboardLayout({ children }: { children: ReactNode }) {
  const { user, loading, isAuthenticated, logout } = useAuth();
  const { hasFeature, subscription } = useFeatures();
  const router = useRouter();
  const pathname = usePathname();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [upgradeModal, setUpgradeModal] = useState<{ isOpen: boolean; featureName: string; requiredPlan: string }>({
    isOpen: false,
    featureName: '',
    requiredPlan: 'Premium'
  });

  // Close sidebar when route changes (mobile)
  useEffect(() => {
    setSidebarOpen(false);
  }, [pathname]);

  useEffect(() => {
    if (!loading && !isAuthenticated) {
      router.push('/auth/login');
    }
  }, [loading, isAuthenticated, router]);

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-lg">Loading...</div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return null;
  }

  const baseNavigation: NavItem[] = [
    { name: 'Dashboard', href: '/dashboard', icon: '📊', roles: ['Worker', 'Manager', 'Owner'] },
    { name: 'My Shift', href: '/dashboard/shifts', icon: '👷', roles: ['Worker', 'Manager', 'Owner'] },
    { name: 'All Shifts', href: '/dashboard/shifts/all', icon: '📋', roles: ['Manager', 'Owner'] },
    { name: 'Reports', href: '/dashboard/reports', icon: '📈', roles: ['Manager', 'Owner'], featureCode: 'REPORTS', requiredPlan: 'Premium' },
    { name: 'Credit Customers', href: '/dashboard/credit-customers', icon: '💳', roles: ['Manager', 'Owner'], featureCode: 'CREDIT_CUSTOMERS', requiredPlan: 'Premium' },
    { name: 'Expenses', href: '/dashboard/expenses', icon: '💸', roles: ['Manager', 'Owner'], featureCode: 'EXPENSES', requiredPlan: 'Premium' },
    { name: 'Users', href: '/dashboard/users', icon: '👥', roles: ['Owner'] },
    { name: 'Fuel Types', href: '/dashboard/fuel-types', icon: '⛽', roles: ['Manager', 'Owner'] },
    { name: 'Fuel Rates', href: '/dashboard/fuel-rates', icon: '💰', roles: ['Manager', 'Owner'] },
    { name: 'Tanks', href: '/dashboard/tanks', icon: '🛢️', roles: ['Manager', 'Owner'] },
    { name: 'Stock', href: '/dashboard/stock', icon: '📦', roles: ['Manager', 'Owner'] },
    { name: 'Machines', href: '/dashboard/machines', icon: '🔧', roles: ['Manager', 'Owner'] },
    { name: 'Nozzles', href: '/dashboard/nozzles', icon: '🚿', roles: ['Manager', 'Owner'] },
    { name: 'Subscription', href: '/dashboard/subscription', icon: '💼', roles: ['Owner'] },
  ];

  const navigation = baseNavigation.filter(item =>
    item.roles.includes(user?.role || '')
  );

  const handleLockedNavClick = (item: NavItem) => {
    setUpgradeModal({
      isOpen: true,
      featureName: item.name,
      requiredPlan: item.requiredPlan || 'Premium'
    });
  };

  const isActive = (href: string) => {
    if (href === '/dashboard') {
      return pathname === href;
    }
    return pathname.startsWith(href);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Trial Banner - shows at the very top */}
      <TrialBanner />

      {/* Mobile sidebar backdrop */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 z-40 bg-black bg-opacity-50 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Top Navigation Bar */}
      <nav className="bg-white shadow-sm border-b border-gray-200 sticky top-0 z-30">
        <div className="mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-14 sm:h-16">
            <div className="flex items-center">
              {/* Mobile menu button */}
              <button
                onClick={() => setSidebarOpen(!sidebarOpen)}
                className="lg:hidden p-2 rounded-md text-gray-600 hover:text-gray-900 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500"
                aria-label="Toggle menu"
              >
                <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  {sidebarOpen ? (
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  ) : (
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                  )}
                </svg>
              </button>

              <h1 className="text-lg sm:text-xl font-bold text-gray-900 ml-2 lg:ml-0">PPM System</h1>

              {/* User info - hidden on mobile, shown on larger screens */}
              {user && (
                <div className="hidden md:flex ml-4 lg:ml-6 text-sm text-gray-600 items-center">
                  <span className="font-medium truncate max-w-[120px] lg:max-w-none">{user.fullName}</span>
                  <span className="mx-2">•</span>
                  <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded whitespace-nowrap">
                    {user.role}
                  </span>
                  {user.tenantCode && (
                    <>
                      <span className="mx-2 hidden lg:inline">•</span>
                      <span className="hidden lg:inline text-xs bg-gray-100 text-gray-800 px-2 py-1 rounded">
                        {user.tenantCode}
                      </span>
                    </>
                  )}
                </div>
              )}
            </div>
            <div className="flex items-center gap-2">
              {/* Show role badge on mobile */}
              {user && (
                <span className="md:hidden text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">
                  {user.role}
                </span>
              )}
              <button
                onClick={logout}
                className="text-sm text-gray-700 hover:text-gray-900 px-2 sm:px-3 py-2 rounded-md hover:bg-gray-100"
              >
                <span className="hidden sm:inline">Logout</span>
                <svg className="sm:hidden h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </nav>

      <div className="flex">
        {/* Sidebar - hidden on mobile, fixed overlay when open */}
        <aside
          className={`
            fixed lg:static inset-y-0 left-0 z-50 lg:z-auto
            w-64 bg-white shadow-lg lg:shadow-sm
            transform transition-transform duration-300 ease-in-out
            ${sidebarOpen ? 'translate-x-0' : '-translate-x-full'}
            lg:translate-x-0
            pt-14 sm:pt-16 lg:pt-0
            min-h-screen lg:min-h-[calc(100vh-4rem)]
            overflow-y-auto
          `}
        >
          {/* Mobile sidebar header */}
          <div className="lg:hidden flex items-center justify-between px-4 py-3 border-b border-gray-200">
            <span className="font-medium text-gray-900">Menu</span>
            <button
              onClick={() => setSidebarOpen(false)}
              className="p-1 rounded-md text-gray-500 hover:text-gray-700 hover:bg-gray-100"
            >
              <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          {/* Mobile user info */}
          {user && (
            <div className="lg:hidden px-4 py-3 border-b border-gray-200 bg-gray-50">
              <p className="font-medium text-gray-900 truncate">{user.fullName}</p>
              <p className="text-xs text-gray-500">{user.tenantCode}</p>
            </div>
          )}

          <nav className="mt-3 lg:mt-5 px-2 space-y-1 pb-4 flex flex-col h-[calc(100%-120px)] lg:h-[calc(100%-60px)]">
            <div className="flex-1">
            {navigation.map((item) => {
              const isFeatureLocked = item.featureCode && !hasFeature(item.featureCode);

              if (isFeatureLocked) {
                return (
                  <button
                    key={item.name}
                    onClick={() => handleLockedNavClick(item)}
                    className="w-full group flex items-center px-3 py-2.5 sm:py-2 text-sm font-medium rounded-md text-gray-400 hover:bg-gray-50 cursor-pointer"
                  >
                    <span className="mr-3 text-xl opacity-50">{item.icon}</span>
                    <span className="flex-1 text-left">{item.name}</span>
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
                  </button>
                );
              }

              return (
                <Link
                  key={item.name}
                  href={item.href}
                  className={`
                    group flex items-center px-3 py-2.5 sm:py-2 text-sm font-medium rounded-md
                    ${
                      isActive(item.href)
                        ? 'bg-blue-50 text-blue-700'
                        : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                    }
                  `}
                >
                  <span className="mr-3 text-xl">{item.icon}</span>
                  {item.name}
                </Link>
              );
            })}
            </div>

            {/* Version info */}
            <div className="mt-auto pt-4 border-t border-gray-200 px-3">
              <p className="text-xs text-gray-400">
                v{process.env.NEXT_PUBLIC_APP_VERSION || '1.0.0'}
              </p>
            </div>
          </nav>
        </aside>

        {/* Upgrade Modal */}
        <UpgradeModal
          isOpen={upgradeModal.isOpen}
          onClose={() => setUpgradeModal({ ...upgradeModal, isOpen: false })}
          featureName={upgradeModal.featureName}
          currentPlan={subscription?.subscriptionPlan || 'Basic'}
          requiredPlan={upgradeModal.requiredPlan}
        />

        {/* Main Content */}
        <main className="flex-1 p-4 sm:p-6 lg:p-8 min-w-0">
          {children}
        </main>
      </div>
    </div>
  );
}
