using FurniFlowUz.Application.DTOs.Auth;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for authentication and user management
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user
    /// </summary>
    Task<UserDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes user password
    /// </summary>
    Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    Task<UserDto> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users
    /// </summary>
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user information
    /// </summary>
    Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user (soft delete by setting IsActive to false)
    /// </summary>
    Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
}
