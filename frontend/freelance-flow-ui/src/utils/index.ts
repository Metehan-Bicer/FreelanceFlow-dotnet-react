import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('tr-TR', {
    style: 'currency',
    currency: 'TRY',
    minimumFractionDigits: 2
  }).format(amount);
}

export function formatDate(date: string | Date, format: 'short' | 'long' = 'long'): string {
  const dateObj = typeof date === 'string' ? new Date(date) : date;
  
  if (format === 'short') {
    return new Intl.DateTimeFormat('tr-TR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).format(dateObj);
  }
  
  return new Intl.DateTimeFormat('tr-TR', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  }).format(dateObj);
}

export function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

export function truncateText(text: string, maxLength: number): string {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + '...';
}

export function generateInvoiceNumber(): string {
  const now = new Date();
  const year = now.getFullYear();
  const month = String(now.getMonth() + 1).padStart(2, '0');
  const random = Math.floor(Math.random() * 1000).toString().padStart(3, '0');
  return `INV-${year}${month}-${random}`;
}

// Status utility functions
export function getStatusColor(status: string): string {
  switch (status?.toLowerCase()) {
    case 'active':
    case 'completed':
    case 'paid':
    case 'success':
      return 'text-green-600 bg-green-50';
    case 'pending':
    case 'inprogress':
    case 'in progress':
      return 'text-blue-600 bg-blue-50';
    case 'overdue':
    case 'cancelled':
    case 'error':
      return 'text-red-600 bg-red-50';
    case 'onhold':
    case 'on hold':
    case 'warning':
      return 'text-yellow-600 bg-yellow-50';
    case 'draft':
    case 'planning':
      return 'text-gray-600 bg-gray-50';
    default:
      return 'text-gray-600 bg-gray-50';
  }
}

export function getStatusText(status: string): string {
  switch (status?.toLowerCase()) {
    case 'active':
      return 'Aktif';
    case 'inactive':
      return 'Pasif';
    case 'completed':
      return 'Tamamlandı';
    case 'inprogress':
    case 'in progress':
      return 'Devam Ediyor';
    case 'planning':
      return 'Planlama';
    case 'onhold':
    case 'on hold':
      return 'Beklemede';
    case 'cancelled':
      return 'İptal Edildi';
    case 'paid':
      return 'Ödendi';
    case 'pending':
      return 'Beklemede';
    case 'overdue':
      return 'Gecikmiş';
    case 'draft':
      return 'Taslak';
    case 'sent':
      return 'Gönderildi';
    default:
      return status || 'Bilinmiyor';
  }
}