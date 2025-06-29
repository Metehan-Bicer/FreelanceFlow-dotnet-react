using AutoMapper;
using FreelanceFlow.Application.DTOs.Client;
using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Core.Common;
using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Interfaces.Repositories;

namespace FreelanceFlow.Application.Services.Implementations;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public ClientService(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ClientDto>>> GetAllClientsAsync()
    {
        try
        {
            var clients = await _clientRepository.GetAllAsync();
            var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);
            return Result<IEnumerable<ClientDto>>.Success(clientDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ClientDto>>.Failure($"Error retrieving clients: {ex.Message}");
        }
    }

    public async Task<Result<ClientDto>> GetClientByIdAsync(Guid id)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null || client.IsDeleted)
            {
                return Result<ClientDto>.Failure("Client not found");
            }

            var clientDto = _mapper.Map<ClientDto>(client);
            return Result<ClientDto>.Success(clientDto);
        }
        catch (Exception ex)
        {
            return Result<ClientDto>.Failure($"Error retrieving client: {ex.Message}");
        }
    }

    public async Task<Result<ClientDto>> CreateClientAsync(CreateClientDto createClientDto)
    {
        try
        {
            // Check if email already exists
            var existingClient = await _clientRepository.GetByEmailAsync(createClientDto.Email);
            if (existingClient != null && !existingClient.IsDeleted)
            {
                return Result<ClientDto>.Failure("A client with this email already exists");
            }

            var client = _mapper.Map<Client>(createClientDto);
            var createdClient = await _clientRepository.AddAsync(client);
            var clientDto = _mapper.Map<ClientDto>(createdClient);

            return Result<ClientDto>.Success(clientDto);
        }
        catch (Exception ex)
        {
            return Result<ClientDto>.Failure($"Error creating client: {ex.Message}");
        }
    }

    public async Task<Result<ClientDto>> UpdateClientAsync(Guid id, UpdateClientDto updateClientDto)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null || client.IsDeleted)
            {
                return Result<ClientDto>.Failure("Client not found");
            }

            // Check if email is being changed and if it already exists
            if (client.Email != updateClientDto.Email)
            {
                var existingClient = await _clientRepository.GetByEmailAsync(updateClientDto.Email);
                if (existingClient != null && existingClient.Id != id && !existingClient.IsDeleted)
                {
                    return Result<ClientDto>.Failure("A client with this email already exists");
                }
            }

            _mapper.Map(updateClientDto, client);
            await _clientRepository.UpdateAsync(client);
            
            var clientDto = _mapper.Map<ClientDto>(client);
            return Result<ClientDto>.Success(clientDto);
        }
        catch (Exception ex)
        {
            return Result<ClientDto>.Failure($"Error updating client: {ex.Message}");
        }
    }

    public async Task<Result> DeleteClientAsync(Guid id)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null || client.IsDeleted)
            {
                return Result.Failure("Client not found");
            }

            // Soft delete
            client.IsDeleted = true;
            await _clientRepository.UpdateAsync(client);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting client: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<ClientDto>>> SearchClientsByNameAsync(string name)
    {
        try
        {
            var clients = await _clientRepository.SearchByNameAsync(name);
            var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);
            return Result<IEnumerable<ClientDto>>.Success(clientDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ClientDto>>.Failure($"Error searching clients: {ex.Message}");
        }
    }

    public async Task<Result<ClientDto>> GetClientByEmailAsync(string email)
    {
        try
        {
            var client = await _clientRepository.GetByEmailAsync(email);
            if (client == null || client.IsDeleted)
            {
                return Result<ClientDto>.Failure("Client not found");
            }

            var clientDto = _mapper.Map<ClientDto>(client);
            return Result<ClientDto>.Success(clientDto);
        }
        catch (Exception ex)
        {
            return Result<ClientDto>.Failure($"Error retrieving client by email: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<ClientDto>>> GetActiveClientsAsync()
    {
        try
        {
            var clients = await _clientRepository.GetActiveClientsAsync();
            var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);
            return Result<IEnumerable<ClientDto>>.Success(clientDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ClientDto>>.Failure($"Error retrieving active clients: {ex.Message}");
        }
    }
}