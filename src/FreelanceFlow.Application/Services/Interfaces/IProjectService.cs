using FreelanceFlow.Application.DTOs.Project;
using FreelanceFlow.Core.Common;
using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Application.Services.Interfaces;

public interface IProjectService
{
    Task<Result<IEnumerable<ProjectDto>>> GetAllAsync();
    Task<Result<ProjectDto>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(CreateProjectDto dto);
    Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectDto dto);
    Task<Result<string>> DeleteAsync(Guid id);
    Task<Result<IEnumerable<ProjectDto>>> GetByClientIdAsync(Guid clientId);
    Task<Result<string>> UpdateStatusAsync(Guid id, ProjectStatus status);
    Task<Result<string>> UpdateProjectActiveStatusAsync(Guid id, bool isActive);
}