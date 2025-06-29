using FreelanceFlow.Application.DTOs.Auth;
using FreelanceFlow.Core.Common;

namespace FreelanceFlow.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<Result<string>> RegisterAsync(RegisterRequestDto request);
    Task<Result<string>> ChangePasswordAsync(Guid userId, ChangePasswordDto request);
    Task<Result<string>> RefreshTokenAsync(string token);
}