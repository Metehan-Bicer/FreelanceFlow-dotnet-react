using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FreelanceFlow.Application.DTOs.Auth;
using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Core.Common;
using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Interfaces.Repositories;
using FreelanceFlow.Domain.Enums;
using AutoMapper;
using BC = BCrypt.Net.BCrypt;

namespace FreelanceFlow.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        
        if (user == null || !user.IsActive)
        {
            return Result<LoginResponseDto>.Failure("Geçersiz kullanıcı adı veya şifre.");
        }

        if (!BC.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponseDto>.Failure("Geçersiz kullanıcı adı veya şifre.");
        }

        var token = GenerateJwtToken(user);
        var expiryMinutesStr = _configuration["JwtSettings:ExpiryMinutes"];
        if (string.IsNullOrEmpty(expiryMinutesStr) || !int.TryParse(expiryMinutesStr, out var expiryMinutes))
        {
            expiryMinutes = 60; // Default 60 minutes
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var response = new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };

        return Result<LoginResponseDto>.Success(response);
    }

    public async Task<Result<string>> RegisterAsync(RegisterRequestDto request)
    {
        if (await _userRepository.IsUsernameExistsAsync(request.Username))
        {
            return Result<string>.Failure("Bu kullanıcı adı zaten kullanılmaktadır.");
        }

        if (await _userRepository.IsEmailExistsAsync(request.Email))
        {
            return Result<string>.Failure("Bu email adresi zaten kullanılmaktadır.");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BC.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Role = UserRole.User,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        
        return Result<string>.Success("Kullanıcı başarıyla oluşturuldu.");
    }

    public async Task<Result<string>> ChangePasswordAsync(Guid userId, ChangePasswordDto request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return Result<string>.Failure("Kullanıcı bulunamadı.");
        }

        if (!BC.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return Result<string>.Failure("Mevcut şifre yanlış.");
        }

        user.PasswordHash = BC.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);

        return Result<string>.Success("Şifre başarıyla değiştirildi.");
    }

    public async Task<Result<string>> RefreshTokenAsync(string token)
    {
        // Token refresh logic can be implemented here if needed
        await Task.CompletedTask;
        return Result<string>.Failure("Token yenileme henüz desteklenmiyor.");
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKeyStr = jwtSettings["SecretKey"];
        var expiryMinutesStr = jwtSettings["ExpiryMinutes"];
        
        if (string.IsNullOrEmpty(secretKeyStr))
        {
            throw new InvalidOperationException("JWT SecretKey configuration is missing");
        }
        
        if (string.IsNullOrEmpty(expiryMinutesStr) || !int.TryParse(expiryMinutesStr, out var expiryMinutes))
        {
            expiryMinutes = 60; // Default 60 minutes
        }

        var secretKey = Encoding.UTF8.GetBytes(secretKeyStr);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        var key = new SymmetricSecurityKey(secretKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }
}