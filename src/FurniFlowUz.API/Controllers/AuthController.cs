using FurniFlowUz.Application.DTOs.Auth;
using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for authentication and user management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Login response with JWT token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<LoginResponseDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var result = await _authService.LoginAsync(request, cancellationToken);
        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Login successful"));
    }

    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="request">User registration data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<UserDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var result = await _authService.RegisterAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id },
            ApiResponse<UserDto>.SuccessResponse(result, "User registered successfully"));
    }

    /// <summary>
    /// Changes the password for a user
    /// </summary>
    /// <param name="request">Password change request containing old and new password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword(
        [FromBody] ChangePasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        await _authService.ChangePasswordAsync(userId, request, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Password changed successfully"));
    }

    /// <summary>
    /// Gets all users in the system
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all users</returns>
    [HttpGet("users")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsers(
        CancellationToken cancellationToken)
    {
        var users = await _authService.GetAllUsersAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users, "Users retrieved successfully"));
    }

    /// <summary>
    /// Gets a specific user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User information</returns>
    [HttpGet("users/{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var user = await _authService.GetUserByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User retrieved successfully"));
    }

    /// <summary>
    /// Updates user information
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Updated user data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user information</returns>
    [HttpPut("users/{id}")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(
        [FromRoute] int id,
        [FromBody] UpdateUserDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<UserDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var user = await _authService.UpdateUserAsync(id, request, cancellationToken);
        return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User updated successfully"));
    }

    /// <summary>
    /// Deletes a user (soft delete)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("users/{id}")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _authService.DeleteUserAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "User deleted successfully"));
    }
}
