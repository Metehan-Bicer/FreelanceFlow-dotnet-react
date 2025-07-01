using AutoMapper;
using FreelanceFlow.Application.DTOs.Project;
using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Core.Common;
using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Enums;
using FreelanceFlow.Domain.Interfaces.Repositories;

namespace FreelanceFlow.Application.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public ProjectService(IProjectRepository projectRepository, IClientRepository clientRepository, IMapper mapper)
    {
        _projectRepository = projectRepository;
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ProjectDto>>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllWithClientsAsync();
        var projectDtos = _mapper.Map<IEnumerable<ProjectDto>>(projects);
        return Result<IEnumerable<ProjectDto>>.Success(projectDtos);
    }

    public async Task<Result<ProjectDto>> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdWithClientAsync(id);
        
        if (project == null)
        {
            return Result<ProjectDto>.Failure("Proje bulunamadı.");
        }

        var projectDto = _mapper.Map<ProjectDto>(project);
        return Result<ProjectDto>.Success(projectDto);
    }

    public async Task<Result<Guid>> CreateAsync(CreateProjectDto dto)
    {
        try
        {
            // Check if client exists
            var client = await _clientRepository.GetByIdAsync(dto.ClientId);
            if (client == null)
            {
                return Result<Guid>.Failure("Belirtilen müşteri bulunamadı.");
            }

            // Sadece aktif müşteriler için proje oluşturulabilir
            if (client.Status != FreelanceFlow.Domain.Enums.ClientStatus.Active)
            {
                return Result<Guid>.Failure("Pasif durumda olan müşteri için proje oluşturulamaz. Önce müşteriyi aktif yapın.");
            }

            // Manuel olarak Project entity oluştur
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                ClientId = dto.ClientId,
                StartDate = dto.StartDate,
                DeadlineDate = dto.DeadlineDate,
                Budget = dto.Budget,
                Priority = dto.Priority,
                Status = ProjectStatus.Planning,
                ProgressPercentage = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _projectRepository.AddAsync(project);
            return Result<Guid>.Success(project.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Error creating project: {ex.Message}");
        }
    }

    public async Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        
        if (project == null)
        {
            return Result<ProjectDto>.Failure("Proje bulunamadı.");
        }

        _mapper.Map(dto, project);
        await _projectRepository.UpdateAsync(project);

        var updatedProject = await _projectRepository.GetByIdWithClientAsync(id);
        var projectDto = _mapper.Map<ProjectDto>(updatedProject);
        
        return Result<ProjectDto>.Success(projectDto);
    }

    public async Task<Result<string>> DeleteAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        
        if (project == null)
        {
            return Result<string>.Failure("Proje bulunamadı.");
        }

        await _projectRepository.DeleteAsync(project);
        return Result<string>.Success("Proje başarıyla silindi.");
    }

    public async Task<Result<IEnumerable<ProjectDto>>> GetByClientIdAsync(Guid clientId)
    {
        var projects = await _projectRepository.GetByClientIdAsync(clientId);
        var projectDtos = _mapper.Map<IEnumerable<ProjectDto>>(projects);
        return Result<IEnumerable<ProjectDto>>.Success(projectDtos);
    }

    public async Task<Result<string>> UpdateStatusAsync(Guid id, ProjectStatus status)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        
        if (project == null)
        {
            return Result<string>.Failure("Proje bulunamadı.");
        }

        project.Status = status;
        
        // Auto-update progress and active status based on status
        switch (status)
        {
            case ProjectStatus.Planning:
                project.ProgressPercentage = 0;
                project.IsActive = true; // Planlama aşamasındaki projeler aktif
                break;
            case ProjectStatus.InProgress:
                if (project.ProgressPercentage == 0)
                    project.ProgressPercentage = 10;
                project.IsActive = true; // Devam eden projeler aktif
                break;
            case ProjectStatus.OnHold:
                project.IsActive = true; // Beklemedeki projeler aktif kalır (tekrar başlayabilir)
                break;
            case ProjectStatus.Completed:
                project.ProgressPercentage = 100;
                project.IsActive = false; // Tamamlanan projeler pasif
                break;
            case ProjectStatus.Cancelled:
                project.IsActive = false; // İptal edilen projeler pasif
                break;
        }

        await _projectRepository.UpdateAsync(project);
        return Result<string>.Success("Proje durumu güncellendi.");
    }

    public async Task<Result<string>> UpdateProjectActiveStatusAsync(Guid id, bool isActive)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        
        if (project == null)
        {
            return Result<string>.Failure("Proje bulunamadı.");
        }

        // İş kuralları kontrolü
        if (!isActive && (project.Status == ProjectStatus.InProgress || project.Status == ProjectStatus.Planning))
        {
            return Result<string>.Failure("Devam eden veya planlama aşamasındaki projeler pasif yapılamaz. Önce proje durumunu değiştirin.");
        }

        project.IsActive = isActive;
        await _projectRepository.UpdateAsync(project);
        
        return Result<string>.Success($"Proje {(isActive ? "aktif" : "pasif")} duruma getirildi.");
    }
}