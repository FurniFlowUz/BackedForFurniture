using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Contract;
using FurniFlowUz.Application.DTOs.Seller;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for contract management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Salesperson,Director")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;
    private readonly ISellerService _sellerService;
    private readonly ILogger<ContractsController> _logger;

    public ContractsController(
        IContractService contractService,
        ISellerService sellerService,
        ILogger<ContractsController> logger)
    {
        _contractService = contractService;
        _sellerService = sellerService;
        _logger = logger;
    }

    /// <summary>
    /// Gets paginated list of contracts with filtering
    /// </summary>
    /// <param name="filter">Filter parameters for contracts</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of contracts</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<ContractSummaryDto>>>> GetContracts(
        [FromQuery] ContractFilterDto filter,
        CancellationToken cancellationToken)
    {
        var contracts = await _contractService.GetPagedAsync(filter, cancellationToken);
        return Ok(ApiResponse<PaginatedResult<ContractSummaryDto>>.SuccessResponse(contracts, "Contracts retrieved successfully"));
    }

    /// <summary>
    /// Gets a specific contract by ID with all details
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed contract information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ContractDto>>> GetContract(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var contract = await _contractService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ContractDto>.SuccessResponse(contract, "Contract retrieved successfully"));
    }

    /// <summary>
    /// Creates a new contract
    /// </summary>
    /// <param name="request">Contract creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created contract information</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContractDto>>> CreateContract(
        [FromBody] CreateContractDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<ContractDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var contract = await _contractService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetContract), new { id = contract.Id },
            ApiResponse<ContractDto>.SuccessResponse(contract, "Contract created successfully"));
    }

    /// <summary>
    /// Updates an existing contract
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <param name="request">Updated contract data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated contract information</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ContractDto>>> UpdateContract(
        [FromRoute] int id,
        [FromBody] UpdateContractDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<ContractDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var contract = await _contractService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<ContractDto>.SuccessResponse(contract, "Contract updated successfully"));
    }

    /// <summary>
    /// Updates the status of a contract
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <param name="request">Status update request containing the new status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateContractStatus(
        [FromRoute] int id,
        [FromBody] UpdateContractStatusDto request,
        CancellationToken cancellationToken)
    {
        // Validate model state
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        // Validate enum value
        if (!Enum.IsDefined(typeof(ContractStatus), request.Status))
        {
            _logger.LogWarning("Invalid contract status value: {Status}", request.Status);
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid contract status",
                new List<string> { $"The status value '{request.Status}' is not valid. Valid values are: {string.Join(", ", Enum.GetNames(typeof(ContractStatus)))}" }));
        }

        await _contractService.UpdateStatusAsync(id, request.Status, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Contract status updated successfully"));
    }

    /// <summary>
    /// Deletes a contract (soft delete)
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteContract(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _contractService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Contract deleted successfully"));
    }

    /// <summary>
    /// Gets contract statistics for the current user
    /// PERFORMANCE OPTIMIZED - No CancellationToken to avoid TaskCanceledException
    /// </summary>
    /// <returns>Contract statistics including active, pending, completed counts and revenue</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<ContractStatsDto>>> GetContractStats()
    {
        var stats = await _sellerService.GetDashboardStatsAsync();
        return Ok(ApiResponse<ContractStatsDto>.SuccessResponse(stats, "Contract statistics retrieved successfully"));
    }
}
