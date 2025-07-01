import React, { useState, useEffect } from 'react';
import { Plus, Search, Edit, Trash2, Calendar, DollarSign, User, BarChart3 } from 'lucide-react';
import { DashboardLayout } from '../components/layout/DashboardLayout';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Badge } from '../components/ui/Badge';
import { Loading } from '../components/ui/Loading';
import { ProjectModal } from '../components/ProjectModal';
import { projectService } from '../services/projectService';
import { Project, ProjectStatus } from '../types';
import { formatCurrency, formatDate } from '../utils';

const ProjectsPage: React.FC = () => {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState('');
  const [filterStatus, setFilterStatus] = useState<ProjectStatus | 'All'>('All');
  const [showModal, setShowModal] = useState(false);
  const [selectedProject, setSelectedProject] = useState<Project | undefined>(undefined);
  const [modalMode, setModalMode] = useState<'create' | 'edit'>('create');

  useEffect(() => {
    loadProjects();
  }, []);

  const loadProjects = async () => {
    try {
      setLoading(true);
      setError('');
      const response = await projectService.getAll();
      if (response.success && response.data) {
        // Backend'den gelen verinin array olduğundan emin olalım
        setProjects(Array.isArray(response.data) ? response.data : []);
      } else {
        setError(response.error || 'Projeler yüklenirken hata oluştu');
        setProjects([]); // Hata durumunda boş array set et
      }
    } catch (err) {
      setError('Projeler yüklenirken hata oluştu');
      setProjects([]); // Hata durumunda boş array set et
      console.error('Error loading projects:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Bu projeyi silmek istediğinizden emin misiniz?')) {
      try {
        const response = await projectService.delete(id);
        if (response.success) {
          setProjects(projects.filter(project => project.id !== id));
        } else {
          setError('Proje silinirken hata oluştu');
        }
      } catch (err) {
        setError('Proje silinirken hata oluştu');
      }
    }
  };

  const handleCreateProject = () => {
    setSelectedProject(undefined);
    setModalMode('create');
    setShowModal(true);
  };

  const handleEditProject = (project: Project) => {
    setSelectedProject(project);
    setModalMode('edit');
    setShowModal(true);
  };

  const handleModalSuccess = () => {
    loadProjects(); // Reload projects after successful creation/update
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setSelectedProject(undefined);
    setModalMode('create');
  };

  const getStatusBadgeVariant = (status: ProjectStatus) => {
    switch (status) {
      case 'Completed':
        return 'success';
      case 'InProgress':
        return 'info';
      case 'OnHold':
        return 'warning';
      case 'Cancelled':
        return 'error';
      default:
        return 'default';
    }
  };

  const getStatusText = (status: ProjectStatus) => {
    switch (status) {
      case 'Planning':
        return 'Planlama';
      case 'InProgress':
        return 'Devam Ediyor';
      case 'OnHold':
        return 'Beklemede';
      case 'Completed':
        return 'Tamamlandı';
      case 'Cancelled':
        return 'İptal Edildi';
      default:
        return status;
    }
  };

  // Filter işleminden önce projects'in array olduğundan emin olalım
  const safeProjects = Array.isArray(projects) ? projects : [];
  
  let filteredProjects = safeProjects.filter(project =>
    project.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    project.clientName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (filterStatus !== 'All') {
    filteredProjects = filteredProjects.filter(project => project.status === filterStatus);
  }

  if (loading) {
    return (
      <DashboardLayout title="Projeler">
        <div className="flex items-center justify-center h-64">
          <Loading size="lg" text="Projeler yükleniyor..." />
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout title="Projeler">
      <div className="space-y-6">
        {/* Header Actions */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div className="flex flex-col sm:flex-row gap-4 flex-1">
            <div className="relative flex-1 max-w-md">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-secondary-400" size={16} />
              <Input
                type="text"
                placeholder="Proje ara..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
            <select
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value as ProjectStatus | 'All')}
              className="px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
            >
              <option value="All">Tüm Durumlar</option>
              <option value="Planning">Planlama</option>
              <option value="InProgress">Devam Ediyor</option>
              <option value="OnHold">Beklemede</option>
              <option value="Completed">Tamamlandı</option>
              <option value="Cancelled">İptal Edildi</option>
            </select>
          </div>
          <Button onClick={handleCreateProject}>
            <Plus size={16} className="mr-2" />
            Yeni Proje
          </Button>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-md p-4">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        )}

        {/* Projects Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
          {filteredProjects.map((project) => (
            <Card key={project.id}>
              <CardHeader className="pb-3">
                <div className="flex items-start justify-between">
                  <div>
                    <CardTitle className="text-lg mb-2">{project.name}</CardTitle>
                    <Badge variant={getStatusBadgeVariant(project.status)} size="sm">
                      {getStatusText(project.status)}
                    </Badge>
                  </div>
                  <div className="flex space-x-1">
                    <Button 
                      variant="ghost" 
                      size="sm"
                      onClick={() => handleEditProject(project)}
                    >
                      <Edit size={16} />
                    </Button>
                    <Button 
                      variant="ghost" 
                      size="sm"
                      onClick={() => handleDelete(project.id)}
                    >
                      <Trash2 size={16} />
                    </Button>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  {project.description && (
                    <p className="text-sm text-secondary-600 line-clamp-2">
                      {project.description}
                    </p>
                  )}
                  
                  <div className="flex items-center text-sm text-secondary-600">
                    <User size={14} className="mr-2" />
                    {project.clientName}
                  </div>
                  
                  <div className="flex items-center text-sm text-secondary-600">
                    <Calendar size={14} className="mr-2" />
                    {formatDate(project.startDate, 'short')}
                    {project.endDate && ` - ${formatDate(project.endDate, 'short')}`}
                  </div>
                  
                  {project.budget && (
                    <div className="flex items-center text-sm text-secondary-600">
                      <DollarSign size={14} className="mr-2" />
                      {formatCurrency(project.budget)}
                    </div>
                  )}

                  {project.hourlyRate && (
                    <div className="flex items-center text-sm text-secondary-600">
                      <BarChart3 size={14} className="mr-2" />
                      {formatCurrency(project.hourlyRate)}/saat
                    </div>
                  )}

                  <div className="pt-3 border-t border-secondary-200">
                    <div className="flex justify-between items-center text-sm">
                      <span className="text-secondary-600">Durum:</span>
                      <Badge variant={project.isActive ? 'success' : 'error'} size="sm">
                        {project.isActive ? 'Aktif' : 'Pasif'}
                      </Badge>
                    </div>
                    <div className="flex justify-between text-sm mt-1">
                      <span className="text-secondary-600">Oluşturulma:</span>
                      <span className="font-medium">{formatDate(project.createdAt, 'short')}</span>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        {filteredProjects.length === 0 && !loading && (
          <div className="text-center py-12">
            <p className="text-secondary-500">
              {searchTerm || filterStatus !== 'All' 
                ? 'Arama kriterlerine uygun proje bulunamadı.' 
                : 'Henüz proje bulunmuyor.'
              }
            </p>
            {!searchTerm && filterStatus === 'All' && (
              <Button className="mt-4" onClick={handleCreateProject}>
                İlk Projeyi Ekle
              </Button>
            )}
          </div>
        )}

        {/* Project Modal */}
        <ProjectModal
          isOpen={showModal}
          onClose={handleCloseModal}
          onSuccess={handleModalSuccess}
          project={selectedProject}
          mode={modalMode}
        />
      </div>
    </DashboardLayout>
  );
};

export default ProjectsPage;