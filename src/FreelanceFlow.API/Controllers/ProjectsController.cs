using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FreelanceFlow.Application.DTOs.Project;
using FreelanceFlow.Application.Services.Interfaces;

namespace FreelanceFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _projectService.GetAllAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _projectService.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var result = await _projectService.CreateAsync(dto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        // Yeni oluşturulan projeyi detaylarıyla birlikte getir
        var projectResult = await _projectService.GetByIdAsync(result.Value);
        
        if (!projectResult.IsSuccess)
        {
            // Fallback - sadece ID ile response döndür
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { 
                success = true, 
                data = new { id = result.Value }, 
                message = "Proje başarıyla oluşturuldu" 
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { 
            success = true, 
            data = projectResult.Value, 
            message = "Proje başarıyla oluşturuldu" 
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
    {
        var result = await _projectService.UpdateAsync(id, dto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value,
            message = "Proje başarıyla güncellendi" 
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _projectService.DeleteAsync(id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            message = result.Value 
        });
    }

    /// <summary>
    /// Get active projects only
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveProjects()
    {
        try
        {
            var allProjects = await _projectService.GetAllAsync();
            if (!allProjects.IsSuccess)
            {
                return BadRequest(new { error = allProjects.Error });
            }

            var activeProjects = allProjects.Value.Where(p => p.IsActive);
            
            return Ok(new { 
                success = true, 
                data = activeProjects 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Aktif projeler yüklenirken hata oluştu." });
        }
    }

    /// <summary>
    /// Update project active status
    /// </summary>
    [HttpPut("{id}/active-status")]
    public async Task<IActionResult> UpdateProjectActiveStatus(Guid id, [FromBody] UpdateProjectActiveStatusDto dto)
    {
        var result = await _projectService.UpdateProjectActiveStatusAsync(id, dto.IsActive);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            message = result.Value 
        });
    }
}