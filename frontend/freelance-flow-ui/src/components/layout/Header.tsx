import React from 'react';
import { Bell, Search } from 'lucide-react';
import { useAuth } from '../../hooks/useAuth';
import { cn } from '../../utils';

interface HeaderProps {
  className?: string;
  title?: string;
}

const Header: React.FC<HeaderProps> = ({ className, title }) => {
  const { user } = useAuth();

  // Helper function to get role name from role object or string
  const getRoleName = (role: any): string => {
    if (typeof role === 'string') {
      return role;
    }
    if (role && typeof role === 'object' && role.name) {
      return role.name;
    }
    return 'User';
  };

  return (
    <header className={cn(
      'bg-white border-b border-secondary-200 px-6 py-4',
      className
    )}>
      <div className="flex items-center justify-between">
        <div>
          {title && (
            <h1 className="text-2xl font-semibold text-secondary-900">
              {title}
            </h1>
          )}
        </div>

        <div className="flex items-center space-x-4">
          {/* Search */}
          <div className="relative hidden md:block">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-secondary-400" size={16} />
            <input
              type="text"
              placeholder="Ara..."
              className="pl-10 pr-4 py-2 w-64 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            />
          </div>

          {/* Notifications */}
          <button className="relative p-2 text-secondary-600 hover:text-secondary-900 hover:bg-secondary-100 rounded-md transition-colors">
            <Bell size={20} />
            <span className="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
          </button>

          {/* User Avatar */}
          <div className="flex items-center space-x-3">
            <div className="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center">
              <span className="text-primary-700 font-medium text-sm">
                {user?.username?.charAt(0).toUpperCase() || 'U'}
              </span>
            </div>
            <div className="hidden md:block">
              <p className="text-sm font-medium text-secondary-900">
                {user?.username || 'Kullanıcı'}
              </p>
              <p className="text-xs text-secondary-500">
                {getRoleName(user?.role)}
              </p>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
};

export { Header };