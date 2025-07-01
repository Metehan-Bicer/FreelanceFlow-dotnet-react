import React, { useState, useEffect } from 'react';
import { BarChart3, Users, FolderOpen, FileText, TrendingUp, Clock, DollarSign, AlertCircle } from 'lucide-react';
import { DashboardLayout } from '../components/layout/DashboardLayout';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import { Badge } from '../components/ui/Badge';
import { Loading } from '../components/ui/Loading';
import { dashboardService, DashboardStats, RecentActivity } from '../services/dashboardService';
import { formatCurrency, formatDate, getStatusColor, getStatusText } from '../utils';

const DashboardPage: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [recentActivities, setRecentActivities] = useState<RecentActivity[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      setError('');
      
      const [statsResponse, activitiesResponse] = await Promise.all([
        dashboardService.getStats(),
        dashboardService.getRecentActivities(5)
      ]);

      if (statsResponse.success && statsResponse.data) {
        setStats(statsResponse.data);
      }

      if (activitiesResponse.success && activitiesResponse.data) {
        setRecentActivities(activitiesResponse.data);
      }
    } catch (err) {
      setError('Dashboard verileri yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <DashboardLayout title="Dashboard">
        <div className="flex items-center justify-center h-64">
          <Loading size="lg" text="Dashboard yükleniyor..." />
        </div>
      </DashboardLayout>
    );
  }

  if (error) {
    return (
      <DashboardLayout title="Dashboard">
        <div className="bg-red-50 border border-red-200 rounded-md p-4">
          <p className="text-sm text-red-600">{error}</p>
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout title="Dashboard">
      <div className="space-y-6">
        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Toplam Müşteriler</CardTitle>
              <Users className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{stats?.totalClients || 0}</div>
              <p className="text-xs text-muted-foreground">
                Aktif müşteri sayısı
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Aktif Projeler</CardTitle>
              <FolderOpen className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{stats?.activeProjects || 0}</div>
              <p className="text-xs text-muted-foreground">
                Devam eden projeler
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Bekleyen Faturalar</CardTitle>
              <FileText className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{stats?.pendingInvoices || 0}</div>
              <p className="text-xs text-muted-foreground">
                Ödeme bekleyen
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Toplam Gelir</CardTitle>
              <DollarSign className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{formatCurrency(stats?.totalRevenue || 0)}</div>
              <p className="text-xs text-muted-foreground">
                Bu ay: {formatCurrency(stats?.monthlyRevenue || 0)}
              </p>
            </CardContent>
          </Card>
        </div>

        {/* Secondary Stats */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Geciken Faturalar</CardTitle>
              <AlertCircle className="h-4 w-4 text-red-500" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-red-600">{stats?.overdueInvoices || 0}</div>
              <p className="text-xs text-muted-foreground">
                Vade tarihi geçmiş
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Bekleyen Ödemeler</CardTitle>
              <Clock className="h-4 w-4 text-yellow-500" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-yellow-600">{formatCurrency(stats?.pendingPayments || 0)}</div>
              <p className="text-xs text-muted-foreground">
                Tahsil edilecek tutar
              </p>
            </CardContent>
          </Card>
        </div>

        {/* Recent Activities */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <TrendingUp className="mr-2 h-5 w-5" />
              Son Aktiviteler
            </CardTitle>
          </CardHeader>
          <CardContent>
            {recentActivities.length > 0 ? (
              <div className="space-y-4">
                {recentActivities.map((activity, index) => (
                  <div key={activity.id || index} className="flex items-start space-x-4 p-3 rounded-lg bg-secondary-50">
                    <div className="flex-1">
                      <div className="flex items-center justify-between mb-1">
                        <h4 className="text-sm font-medium">{activity.title}</h4>
                        <Badge 
                          variant={activity.status.toLowerCase() === 'completed' || activity.status.toLowerCase() === 'paid' ? 'success' : 'default'}
                          size="sm"
                        >
                          {getStatusText(activity.status)}
                        </Badge>
                      </div>
                      <p className="text-sm text-secondary-600 mb-2">{activity.description}</p>
                      <div className="flex items-center text-xs text-secondary-500">
                        <span className="capitalize">{getStatusText(activity.type)}</span>
                        <span className="mx-2">•</span>
                        <span>{formatDate(activity.date, 'short')}</span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-8">
                <p className="text-secondary-500">Henüz aktivite bulunmuyor.</p>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Quick Actions */}
        <Card>
          <CardHeader>
            <CardTitle>Hızlı İşlemler</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <button className="p-4 bg-primary-50 hover:bg-primary-100 rounded-lg text-left transition-colors">
                <Users className="h-6 w-6 text-primary-600 mb-2" />
                <h3 className="font-medium text-primary-900">Yeni Müşteri</h3>
                <p className="text-sm text-primary-600">Müşteri ekle</p>
              </button>
              
              <button className="p-4 bg-blue-50 hover:bg-blue-100 rounded-lg text-left transition-colors">
                <FolderOpen className="h-6 w-6 text-blue-600 mb-2" />
                <h3 className="font-medium text-blue-900">Yeni Proje</h3>
                <p className="text-sm text-blue-600">Proje oluştur</p>
              </button>
              
              <button className="p-4 bg-green-50 hover:bg-green-100 rounded-lg text-left transition-colors">
                <FileText className="h-6 w-6 text-green-600 mb-2" />
                <h3 className="font-medium text-green-900">Yeni Fatura</h3>
                <p className="text-sm text-green-600">Fatura düzenle</p>
              </button>
            </div>
          </CardContent>
        </Card>
      </div>
    </DashboardLayout>
  );
};

export default DashboardPage;