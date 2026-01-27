using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FurniFlowUz.Application.DTOs.Auth;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for authentication and user management
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        // Find user by email
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email, cancellationToken);
        var user = users.FirstOrDefault();

        if (user == null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedException("User account is inactive.");
        }

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(GetJwtExpirationHours());

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = Guid.NewGuid().ToString(), // Simple implementation, can be enhanced
            ExpiresAt = expiresAt,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        // Check if email already exists
        var existingUsers = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email, cancellationToken);
        if (existingUsers.Any())
        {
            throw new ValidationException("Email is already registered.");
        }

        // Parse role from string to enum
        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
        {
            throw new ValidationException($"Invalid role: {request.Role}");
        }

        // Create new user
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = userRole,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDto>(user);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        // Verify old password
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new ValidationException("Current password is incorrect.");
        }

        // Update to new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserDto> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        // Check email uniqueness if email is being changed
        if (!string.IsNullOrEmpty(request.Email) && user.Email != request.Email)
        {
            var existingUsers = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email, cancellationToken);
            if (existingUsers.Any())
            {
                throw new ValidationException("Email is already registered.");
            }
        }

        // Update user properties
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        if (!string.IsNullOrEmpty(request.Email))
            user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        if (!string.IsNullOrEmpty(request.Role))
            user.Role = Enum.Parse<UserRole>(request.Role);
        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        // Soft delete by setting IsActive to false
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    #region Private Helper Methods

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "FurniFlowUz";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "FurniFlowUz";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(GetJwtExpirationHours()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private double GetJwtExpirationHours()
    {
        var expirationHours = _configuration["Jwt:ExpirationHours"];
        return double.TryParse(expirationHours, out var hours) ? hours : 24;
    }

    #endregion
}
