using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Seller;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for seller/salesperson-specific operations (dashboard, activities, pending items)
/// </summary>
[ApiController]
[Route("api/seller")]
[Authorize(Roles = "Salesperson")]
public class SellerController : ControllerBase
{
    private readonly IContractService _contractService;
    private readonly ISellerService _sellerService;
    private readonly ILogger<SellerController> _logger;

    public SellerController(
        IContractService contractService,
        ISellerService sellerService,
        ILogger<SellerController> logger)
    {
        _contractService = contractService;
        _sellerService = sellerService;
        _logger = logger;
    }

    /// <summary>
    /// Gets dashboard statistics for the seller
    /// PERFORMANCE OPTIMIZED - No CancellationToken to avoid TaskCanceledException
    /// Uses efficient aggregate queries for fast dashboard loads
    /// </summary>
    /// <returns>Seller dashboard statistics including contracts, orders, and revenue</returns>
    [HttpGet("dashboard/stats")]
    public async Task<ActionResult<ApiResponse<ContractStatsDto>>> GetDashboardStats()
    {
        var stats = await _sellerService.GetDashboardStatsAsync();
        return Ok(ApiResponse<ContractStatsDto>.SuccessResponse(stats, "Dashboard statistics retrieved successfully"));
    }

    /// <summary>
    /// Gets recent activities for the seller
    /// PERFORMANCE OPTIMIZED - No CancellationToken to avoid TaskCanceledException
    /// Uses efficient queries with proper limits
    /// </summary>
    /// <param name="limit">Maximum number of activities to retrieve (default: 10)</param>
    /// <returns>List of recent activities related to contracts and orders</returns>
    [HttpGet("activities")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ActivityDto>>>> GetActivities(
        [FromQuery] int limit = 10)
    {
        var activities = await _sellerService.GetRecentActivitiesAsync(limit);
        return Ok(ApiResponse<IEnumerable<ActivityDto>>.SuccessResponse(activities, "Activities retrieved successfully"));
    }

    /// <summary>
    /// Gets pending items requiring seller attention
    /// PERFORMANCE OPTIMIZED - No CancellationToken to avoid TaskCanceledException
    /// Uses efficient filtered queries
    /// </summary>
    /// <returns>List of pending items needing action</returns>
    [HttpGet("pending-items")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PendingItemDto>>>> GetPendingItems()
    {
        var pendingItems = await _sellerService.GetPendingItemsAsync();
        return Ok(ApiResponse<IEnumerable<PendingItemDto>>.SuccessResponse(pendingItems, "Pending items retrieved successfully"));
    }
}
