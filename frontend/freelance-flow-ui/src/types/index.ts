// Auth Types
export interface User {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: 'Admin' | 'User';
  isActive: boolean;
  createdAt: string;
  lastLoginAt?: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  email: string;
  role: string;
  expiresAt: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phone?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

// Client Types
export interface Client {
  id: string;
  name: string;
  email: string;
  phone?: string;
  company?: string;
  address?: string;
  taxNumber?: string;
  notes?: string;
  isActive: boolean;
  projectCount: number;
  totalRevenue: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateClientRequest {
  name: string;
  email: string;
  phone?: string;
  company?: string;
  address?: string;
  taxNumber?: string;
  notes?: string;
}

export interface UpdateClientRequest {
  name: string;
  email: string;
  phone?: string;
  company?: string;
  address?: string;
  taxNumber?: string;
  notes?: string;
}

// Project Types
export interface Project {
  id: string;
  name: string;
  description: string;
  clientId: string;
  clientName: string;
  startDate: string;
  endDate?: string;
  deadlineDate?: string;
  budget: number;
  hourlyRate?: number;
  status: ProjectStatus;
  priority: Priority;
  progressPercentage: number;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateProjectRequest {
  name: string;
  description: string;
  clientId: string;
  startDate: string;
  deadlineDate?: string;
  budget?: number;
  priority: number; // Backend enum ile uyumlu olması için number
}

export interface UpdateProjectRequest {
  name: string;
  description: string;
  startDate: string;
  deadlineDate?: string;
  budget: number;
  priority: number; // Backend enum ile uyumlu olması için number
  progressPercentage: number;
}

export type ProjectStatus = 'Planning' | 'InProgress' | 'OnHold' | 'Completed' | 'Cancelled';
export type Priority = 'Low' | 'Medium' | 'High' | 'Critical'; // Display için string tut

// Invoice Types
export interface Invoice {
  id: string;
  invoiceNumber: string;
  clientId: string;
  clientName: string;
  projectId?: string;
  projectName?: string;
  issueDate: string;
  dueDate: string;
  subTotal: number;
  taxAmount: number;
  totalAmount: number;
  status: InvoiceStatus;
  paymentStatus: PaymentStatus;
  notes?: string;
  createdAt: string;
  paidAt?: string;
  items: InvoiceItem[];
}

export interface InvoiceItem {
  id: string;
  description: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface CreateInvoiceRequest {
  clientId: string;
  projectId?: string;
  issueDate: string;
  dueDate: string;
  taxRate: number;
  notes?: string;
  items: CreateInvoiceItemRequest[];
}

export interface CreateInvoiceItemRequest {
  description: string;
  quantity: number;
  unitPrice: number;
}

export interface UpdateInvoiceRequest {
  issueDate: string;
  dueDate: string;
  taxRate: number;
  notes?: string;
  items: CreateInvoiceItemRequest[];
}

export type InvoiceStatus = 'Draft' | 'Sent' | 'Paid' | 'Overdue' | 'Cancelled';
export type PaymentStatus = 'Pending' | 'Paid' | 'PartiallyPaid' | 'Overdue';

// Dashboard Types
export interface DashboardStats {
  totalClients: number;
  activeProjects: number;
  pendingInvoices: number;
  overdueInvoices: number;
  totalRevenue: number;
  monthlyRevenue: number;
  pendingPayments: number;
}

export interface RecentActivity {
  type: string;
  title: string;
  description: string;
  date: string;
  status: string;
}

export interface MonthlyRevenueItem {
  month: string;
  revenue: number;
  invoiceCount: number;
}

export interface MonthlyRevenue {
  data: MonthlyRevenueItem[];
}

export interface ProjectStatusStats {
  planning: number;
  inProgress: number;
  onHold: number;
  completed: number;
  cancelled: number;
}

// API Response Types
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  error?: string;
}