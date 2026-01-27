using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Dashboard;
using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.DTOs.Warehouse;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for director-specific operations (dashboard, statistics, overview)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Director")]
public class DirectorController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IOrderService _orderService;
    private readonly IWarehouseService _warehouseService;
    private readonly ILogger<DirectorController> _logger;

    public DirectorController(
        IDashboardService dashboardService,
        IOrderService orderService,
        IWarehouseService warehouseService,
        ILogger<DirectorController> logger)
    {
        _dashboardService = dashboardService;
        _orderService = orderService;
        _warehouseService = warehouseService;
        _logger = logger;
    }

    /// <summary>
    /// Gets comprehensive dashboard data for the director
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Director dashboard with all statistics and alerts</returns>
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<DirectorDashboardDto>>> GetDashboard(
        CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardService.GetDirectorDashboardAsync(cancellationToken);
        return Ok(ApiResponse<DirectorDashboardDto>.SuccessResponse(dashboard, "Dashboard data retrieved successfully"));
    }

    /// <summary>
    /// Gets general system statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>General statistics including orders, contracts, revenue</returns>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<StatisticsDto>>> GetStatistics(
        CancellationToken cancellationToken)
    {
        var statistics = await _dashboardService.GetStatisticsAsync(cancellationToken);
        return Ok(ApiResponse<StatisticsDto>.SuccessResponse(statistics, "Statistics retrieved successfully"));
    }

    /// <summary>
    /// Gets all orders with filtering options
    /// </summary>
    /// <param name="filter">Order filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of orders</returns>
    [HttpGet("orders")]
    public async Task<ActionResult<ApiResponse<PaginatedResult<OrderListDto>>>> GetOrders(
        [FromQuery] OrderFilterDto filter,
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetPagedAsync(filter, cancellationToken);
        return Ok(ApiResponse<PaginatedResult<OrderListDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
    }

    /// <summary>
    /// Gets warehouse alerts for low stock items
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of warehouse items with low stock</returns>
    [HttpGet("warehouse-alerts")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WarehouseAlertDto>>>> GetWarehouseAlerts(
        CancellationToken cancellationToken)
    {
        var alerts = await _warehouseService.GetLowStockAlertsAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<WarehouseAlertDto>>.SuccessResponse(alerts, "Warehouse alerts retrieved successfully"));
    }
}
