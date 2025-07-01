using Microsoft.AspNetCore.Mvc;
using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Application.DTOs.Client;
using FreelanceFlow.Core.Common;

namespace FreelanceFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IClientService clientService, ILogger<ClientsController> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    /// <summary>
    /// Get all clients with pagination support
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
    {
        try
        {
            var result = await _clientService.GetAllClientsAsync();
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching clients");
            return StatusCode(500, "Internal server error occurred");
        }
    }

    /// <summary>
    /// Get client by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientDto>> GetClient(Guid id)
    {
        try
        {
            var result = await _clientService.GetClientByIdAsync(id);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching client {ClientId}", id);
            return StatusCode(500, "Internal server error occurred");
        }
    }

    /// <summary>
    /// Create a new client
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientDto createClientDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _clientService.CreateClientAsync(createClientDto);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(GetClient), new { id = result.Value!.Id }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating client");
            return StatusCode(500, "Internal server error occurred");
        }
    }

    /// <summary>
    /// Update an existing client
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientDto>> UpdateClient(Guid id, [FromBody] UpdateClientDto updateClientDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _clientService.UpdateClientAsync(id, updateClientDto);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating client {ClientId}", id);
            return StatusCode(500, "Internal server error occurred");
        }
    }

    /// <summary>
    /// Delete a client
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteClient(Guid id)
    {
        try
        {
            var result = await _clientService.DeleteClientAsync(id);
            
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting client {ClientId}", id);
            return StatusCode(500, "Internal server error occurred");
        }
    }

    /// <summary>
    /// Search clients by name
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ClientDto>>> SearchClients([FromQuery] string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search name cannot be empty");
            }

            var result = await _clientService.SearchClientsByNameAsync(name);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching clients with name {Name}", name);
            return StatusCode(500, "Internal server error occurred");
        }
    }

    /// <summary>
    /// Get active clients only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetActiveClients()
    {
        try
        {
            var result = await _clientService.GetActiveClientsAsync();
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching active clients");
            return StatusCode(500, "Internal server error occurred");
        }
    }

    /// <summary>
    /// Update client status (Active/Inactive/Suspended)
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateClientStatus(Guid id, [FromBody] UpdateClientStatusDto dto)
    {
        try
        {
            var result = await _clientService.UpdateClientStatusAsync(id, dto.Status);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(new { 
                success = true, 
                data = result.Value,
                message = "Müşteri durumu başarıyla güncellendi" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating client status");
            return StatusCode(500, "Internal server error occurred");
        }
    }
}