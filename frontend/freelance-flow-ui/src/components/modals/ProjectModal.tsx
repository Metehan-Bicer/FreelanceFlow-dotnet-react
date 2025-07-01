import React, { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogClose } from '../ui/dialog';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { Textarea } from '../ui/textarea';
import { Label } from '../ui/label';
import { projectService } from '../../services/projectService';
import { clientService } from '../../services/clientService';
import { CreateProjectRequest, Client } from '../../types';

interface ProjectModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const ProjectModal: React.FC<ProjectModalProps> = ({ isOpen, onClose, onSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [clients, setClients] = useState<Client[]>([]);
  const [formData, setFormData] = useState<CreateProjectRequest>({
    name: '',
    description: '',
    clientId: '',
    startDate: '',
    deadlineDate: undefined,
    budget: undefined,
    priority: 2 // Medium (number value)
  });

  useEffect(() => {
    if (isOpen) {
      loadClients();
    }
  }, [isOpen]);

  const loadClients = async () => {
    try {
      const response = await clientService.getAll();
      if (response.success && response.data) {
        setClients(response.data);
      }
    } catch (error) {
      console.error('Error loading clients:', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const projectData: CreateProjectRequest = {
        name: formData.name,
        description: formData.description,
        clientId: formData.clientId,
        startDate: formData.startDate,
        deadlineDate: formData.deadlineDate || undefined,
        budget: formData.budget || undefined,
        priority: formData.priority
      };

      const response = await projectService.create(projectData);
      if (response.success) {
        onSuccess();
        onClose();
        // Reset form
        setFormData({
          name: '',
          description: '',
          clientId: '',
          startDate: '',
          deadlineDate: undefined,
          budget: undefined,
          priority: 2 // Medium (number value)
        });
      } else {
        alert(response.error || 'Proje oluşturulurken hata oluştu');
      }
    } catch (error) {
      console.error('Project creation error:', error);
      alert('Proje oluşturulurken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: name === 'budget' || name === 'priority' ? (value ? Number(value) : undefined) : value
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="bg-white rounded-lg p-6 w-full max-w-md max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Yeni Proje Ekle</DialogTitle>
          <DialogClose onClick={onClose} />
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <Label htmlFor="name">Proje Adı *</Label>
            <Input
              id="name"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
              placeholder="Proje adını girin"
            />
          </div>

          <div>
            <Label htmlFor="clientId">Müşteri *</Label>
            <select
              id="clientId"
              name="clientId"
              value={formData.clientId}
              onChange={handleChange}
              required
              className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
            >
              <option value="">Müşteri seçin</option>
              {clients.map((client) => (
                <option key={client.id} value={client.id}>
                  {client.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <Label htmlFor="description">Açıklama</Label>
            <Textarea
              id="description"
              name="description"
              value={formData.description}
              onChange={handleChange}
              rows={3}
              placeholder="Proje açıklaması..."
            />
          </div>

          <div>
            <Label htmlFor="startDate">Başlangıç Tarihi *</Label>
            <Input
              id="startDate"
              name="startDate"
              type="date"
              value={formData.startDate}
              onChange={handleChange}
              required
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="budget">Bütçe</Label>
              <Input
                id="budget"
                type="number"
                value={formData.budget || ''}
                onChange={(e) => setFormData(prev => ({ 
                  ...prev, 
                  budget: e.target.value ? parseFloat(e.target.value) : undefined 
                }))}
                placeholder="0.00"
                min="0"
                step="0.01"
              />
            </div>

            <div>
              <Label htmlFor="priority">Öncelik</Label>
              <select
                id="priority"
                value={formData.priority}
                onChange={(e) => setFormData(prev => ({ 
                  ...prev, 
                  priority: parseInt(e.target.value) // Convert string to number
                }))}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value={1}>Düşük</option>
                <option value={2}>Orta</option>
                <option value={3}>Yüksek</option>
                <option value={4}>Kritik</option>
              </select>
            </div>
          </div>

          <div>
            <Label htmlFor="deadlineDate">Bitiş Tarihi</Label>
            <Input
              id="deadlineDate"
              type="date"
              value={formData.deadlineDate || ''}
              onChange={(e) => setFormData(prev => ({ 
                ...prev, 
                deadlineDate: e.target.value || undefined 
              }))}
            />
          </div>

          <div className="flex space-x-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={onClose}
              className="flex-1"
            >
              İptal
            </Button>
            <Button
              type="submit"
              loading={loading}
              className="flex-1"
            >
              Kaydet
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
};