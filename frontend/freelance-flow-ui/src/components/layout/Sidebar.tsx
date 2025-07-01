import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { 
  LayoutDashboard, 
  Users, 
  FolderOpen, 
  FileText, 
  Settings, 
  Menu, 
  X,
  LogOut 
} from 'lucide-react';
import { cn } from '../../utils';
import { useAuth } from '../../hooks/useAuth';

interface SidebarProps {
  className?: string;
}

const Sidebar: React.FC<SidebarProps> = ({ className }) => {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const location = useLocation();
  const { logout, user } = useAuth();

  const menuItems = [
    {
      title: 'Dashboard',
      href: '/dashboard',
      icon: LayoutDashboard,
    },
    {
      title: 'Müşteriler',
      href: '/clients',
      icon: Users,
    },
    {
      title: 'Projeler',
      href: '/projects',
      icon: FolderOpen,
    },
    {
      title: 'Faturalar',
      href: '/invoices',
      icon: FileText,
    },
    {
      title: 'Ayarlar',
      href: '/settings',
      icon: Settings,
    },
  ];

  const handleLogout = () => {
    logout();
  };

  return (
    <div className={cn(
      'flex flex-col h-full bg-white border-r border-secondary-200 transition-all duration-300',
      isCollapsed ? 'w-16' : 'w-64',
      className
    )}>
      {/* Header */}
      <div className="flex items-center justify-between p-4 border-b border-secondary-200">
        {!isCollapsed && (
          <div className="flex items-center space-x-2">
            <div className="w-8 h-8 bg-primary-600 rounded-lg flex items-center justify-center">
              <span className="text-white font-bold text-sm">FF</span>
            </div>
            <span className="font-semibold text-secondary-900">FreelanceFlow</span>
          </div>
        )}
        <button
          onClick={() => setIsCollapsed(!isCollapsed)}
          className="p-1 rounded-md hover:bg-secondary-100 transition-colors"
        >
          {isCollapsed ? <Menu size={20} /> : <X size={20} />}
        </button>
      </div>

      {/* Navigation */}
      <nav className="flex-1 py-4">
        <ul className="space-y-1 px-3">
          {menuItems.map((item) => {
            const isActive = location.pathname === item.href;
            const Icon = item.icon;
            
            return (
              <li key={item.href}>
                <Link
                  to={item.href}
                  className={cn(
                    'flex items-center px-3 py-2 rounded-md text-sm font-medium transition-colors',
                    isActive
                      ? 'bg-primary-100 text-primary-700'
                      : 'text-secondary-600 hover:bg-secondary-100 hover:text-secondary-900'
                  )}
                  title={isCollapsed ? item.title : undefined}
                >
                  <Icon size={20} className="shrink-0" />
                  {!isCollapsed && <span className="ml-3">{item.title}</span>}
                </Link>
              </li>
            );
          })}
        </ul>
      </nav>

      {/* User Info & Logout */}
      <div className="border-t border-secondary-200 p-4">
        {!isCollapsed && user && (
          <div className="mb-3">
            <div className="flex items-center space-x-3">
              <div className="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center">
                <span className="text-primary-700 font-medium text-sm">
                  {user.username?.charAt(0).toUpperCase()}
                </span>
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm font-medium text-secondary-900 truncate">
                  {user.username}
                </p>
                <p className="text-xs text-secondary-500 truncate">
                  {user.email}
                </p>
              </div>
            </div>
          </div>
        )}
        
        <button
          onClick={handleLogout}
          className={cn(
            'flex items-center w-full px-3 py-2 text-sm font-medium text-secondary-600 rounded-md hover:bg-secondary-100 hover:text-secondary-900 transition-colors',
            isCollapsed && 'justify-center'
          )}
          title={isCollapsed ? 'Çıkış Yap' : undefined}
        >
          <LogOut size={20} className="shrink-0" />
          {!isCollapsed && <span className="ml-3">Çıkış Yap</span>}
        </button>
      </div>
    </div>
  );
};

export { Sidebar };