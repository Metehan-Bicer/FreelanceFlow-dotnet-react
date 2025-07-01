import React, { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { Button } from './ui/Button';
import { Input } from './ui/Input';
import { projectService } from '../services/projectService';
import { clientService } from '../services/clientService';
import { CreateProjectRequest, UpdateProjectRequest, Project, Client } from '../types';

// Priority enum backend ile uyumlu olacak şekilde
const PRIORITY_OPTIONS = [
  { value: 1, label: 'Düşük' },
  { value: 2, label: 'Orta' },
  { value: 3, label: 'Yüksek' },
  { value: 4, label: 'Kritik' }
];

interface ProjectModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  project?: Project; // Edit mode için
  mode?: 'create' | 'edit';
}

const ProjectModal: React.FC<ProjectModalProps> = ({ 
  isOpen, 
  onClose, 
  onSuccess, 
  project, 
  mode = 'create' 
}) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [clients, setClients] = useState<Client[]>([]);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    clientId: '',
    startDate: '',
    deadlineDate: '',
    budget: 0,
    priority: 2, // Medium as default (integer)
  });

  // Müşterileri yükle
  useEffect(() => {
    if (isOpen) {
      loadClients();
    }
  }, [isOpen]);

  // Edit mode için form data'yı doldur
  useEffect(() => {
    if (project && mode === 'edit') {
      setFormData({
        name: project.name || '',
        description: project.description || '',
        clientId: project.clientId || '',
        startDate: project.startDate ? new Date(project.startDate).toISOString().split('T')[0] : '',
        deadlineDate: project.deadlineDate ? new Date(project.deadlineDate).toISOString().split('T')[0] : '',
        budget: project.budget || 0,
        priority: getPriorityValue(project.priority), // String'i integer'a çevir
      });
    }
  }, [project, mode]);

  // Priority string'ini integer'a çeviren helper function
  const getPriorityValue = (priorityString: string): number => {
    switch (priorityString) {
      case 'Low': return 1;
      case 'Medium': return 2;
      case 'High': return 3;
      case 'Critical': return 4;
      default: return 2; // Default Medium
    }
  };

  const loadClients = async () => {
    try {
      // Sadece aktif müşterileri yükle
      const response = await clientService.getActive();
      if (response.success && response.data) {
        setClients(Array.isArray(response.data) ? response.data : []);
      }
    } catch (error) {
      console.error('Müşteriler yüklenemedi:', error);
    }
  };

  const resetForm = () => {
    setFormData({
      name: '',
      description: '',
      clientId: '',
      startDate: '',
      deadlineDate: '',
      budget: 0,
      priority: 2, // Medium as default
    });
    setError('');
    setSuccess('');
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess('');

    try {
      console.log(`Proje ${mode === 'edit' ? 'güncelleme' : 'ekleme'} isteği gönderiliyor:`, formData);
      
      // Backend'e gönderilecek veri formatı
      const requestData = {
        name: formData.name,
        description: formData.description,
        clientId: formData.clientId,
        startDate: formData.startDate,
        deadlineDate: formData.deadlineDate || undefined,
        budget: formData.budget || 0,
        priority: formData.priority // Integer değer
      };

      let response;
      if (mode === 'edit' && project) {
        // Edit mode için UpdateProjectRequest formatına çevir
        const updateData = {
          ...requestData,
          progressPercentage: project.progressPercentage || 0 // Mevcut progress değerini koru
        };
        response = await projectService.update(project.id, updateData);
      } else {
        response = await projectService.create(requestData);
      }
      
      console.log('Backend response:', response);
      
      if (response.success) {
        console.log(`Proje başarıyla ${mode === 'edit' ? 'güncellendi' : 'eklendi'}:`, response.data);
        setSuccess(`Proje başarıyla ${mode === 'edit' ? 'güncellendi' : 'eklendi'}!`);
        
        // Kısa bir delay ile kullanıcıya success mesajını göster
        setTimeout(() => {
          onSuccess(); // Bu, parent component'te loadProjects() fonksiyonunu çağırır
          handleClose();
        }, 1000);
      } else {
        console.error('Backend hatası:', response.error);
        setError(response.error || `Proje ${mode === 'edit' ? 'güncellenirken' : 'oluşturulurken'} bir hata oluştu`);
      }
    } catch (error) {
      console.error('Network/Client hatası:', error);
      setError('Bağlantı hatası. Lütfen tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: name === 'budget' ? parseFloat(value) || 0 : 
               name === 'priority' ? parseInt(value) : value
    });
    // Input değiştiğinde hata mesajını temizle
    if (error) setError('');
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold">
            {mode === 'edit' ? 'Proje Düzenle' : 'Yeni Proje Ekle'}
          </h2>
          <button
            onClick={handleClose}
            className="p-1 hover:bg-gray-100 rounded"
            disabled={loading}
          >
            <X size={20} />
          </button>
        </div>

        {/* Error Message */}
        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        )}

        {/* Success Message */}
        {success && (
          <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded-md">
            <p className="text-sm text-green-600">{success}</p>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <Input
            label="Proje Adı"
            name="name"
            value={formData.name}
            onChange={handleChange}
            required
            placeholder="Proje adını girin"
            disabled={loading}
          />

          <div>
            <label className="block text-sm font-medium text-secondary-700 mb-2">
              Açıklama
            </label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleChange}
              rows={3}
              className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 disabled:bg-gray-50 disabled:cursor-not-allowed"
              placeholder="Proje açıklamasını girin"
              disabled={loading}
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-secondary-700 mb-2">
              Müşteri *
            </label>
            <select
              name="clientId"
              value={formData.clientId}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 disabled:bg-gray-50 disabled:cursor-not-allowed"
              required
              disabled={loading}
            >
              <option value="">Müşteri seçiniz</option>
              {clients.map((client) => (
                <option key={client.id} value={client.id}>
                  {client.name}
                </option>
              ))}
            </select>
          </div>

          <Input
            label="Başlangıç Tarihi"
            name="startDate"
            type="date"
            value={formData.startDate}
            onChange={handleChange}
            disabled={loading}
          />

          <Input
            label="Bitiş Tarihi"
            name="deadlineDate"
            type="date"
            value={formData.deadlineDate}
            onChange={handleChange}
            disabled={loading}
          />

          <Input
            label="Bütçe"
            name="budget"
            type="number"
            step="0.01"
            value={formData.budget || ''}
            onChange={handleChange}
            placeholder="0.00"
            disabled={loading}
          />

          <div>
            <label className="block text-sm font-medium text-secondary-700 mb-2">
              Öncelik
            </label>
            <select
              name="priority"
              value={formData.priority}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 disabled:bg-gray-50 disabled:cursor-not-allowed"
              disabled={loading}
            >
              {PRIORITY_OPTIONS.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>

          <div className="flex space-x-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              className="flex-1"
              disabled={loading}
            >
              İptal
            </Button>
            <Button
              type="submit"
              loading={loading}
              className="flex-1"
              disabled={loading}
            >
              {loading ? 
                (mode === 'edit' ? 'Güncelleniyor...' : 'Kaydediliyor...') : 
                (mode === 'edit' ? 'Güncelle' : 'Kaydet')
              }
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export { ProjectModal };