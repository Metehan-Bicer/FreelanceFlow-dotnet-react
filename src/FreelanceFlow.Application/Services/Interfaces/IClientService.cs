using FreelanceFlow.Application.DTOs.Client;
using FreelanceFlow.Core.Common;

namespace FreelanceFlow.Application.Services.Interfaces;

public interface IClientService
{
    Task<Result<IEnumerable<ClientDto>>> GetAllClientsAsync();
    Task<Result<ClientDto>> GetClientByIdAsync(Guid id);
    Task<Result<ClientDto>> CreateClientAsync(CreateClientDto createClientDto);
    Task<Result<ClientDto>> UpdateClientAsync(Guid id, UpdateClientDto updateClientDto);
    Task<Result> DeleteClientAsync(Guid id);
    Task<Result<IEnumerable<ClientDto>>> SearchClientsByNameAsync(string name);
    Task<Result<ClientDto>> GetClientByEmailAsync(string email);
    Task<Result<IEnumerable<ClientDto>>> GetActiveClientsAsync();
}