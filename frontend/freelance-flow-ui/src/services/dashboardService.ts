import { apiClient } from './apiClient';
import { ApiResponse } from '../types';

export interface DashboardStats {
  totalClients: number;
  activeProjects: number;
  totalInvoices: number;
  totalRevenue: number;
  monthlyRevenue: number;
  pendingInvoices: number;
  overdueInvoices: number;
  pendingPayments: number;
}

export interface RecentActivity {
  id: string;
  type: string;
  title: string;
  description: string;
  date: string;
  status: string;
}

export class DashboardService {
  async getStats(): Promise<ApiResponse<DashboardStats>> {
    return await apiClient.get<DashboardStats>('/dashboard/stats');
  }

  async getRecentActivities(count?: number): Promise<ApiResponse<RecentActivity[]>> {
    const params = count ? `?count=${count}` : '';
    return await apiClient.get<RecentActivity[]>(`/dashboard/recent-activities${params}`);
  }
}

export const dashboardService = new DashboardService();