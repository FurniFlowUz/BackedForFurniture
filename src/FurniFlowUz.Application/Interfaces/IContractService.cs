using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Contract;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for contract management
/// </summary>
public interface IContractService
{
    /// <summary>
    /// Gets all contracts
    /// </summary>
    Task<IEnumerable<ContractDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a contract by ID with all related data
    /// </summary>
    Task<ContractDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated contracts with filtering
    /// </summary>
    Task<PaginatedResult<ContractSummaryDto>> GetPagedAsync(ContractFilterDto filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new contract with auto-generated contract number
    /// </summary>
    Task<ContractDto> CreateAsync(CreateContractDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing contract
    /// </summary>
    Task<ContractDto> UpdateAsync(int id, UpdateContractDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates contract status
    /// </summary>
    Task UpdateStatusAsync(int id, ContractStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a contract (soft delete)
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a unique contract number
    /// </summary>
    Task<string> GenerateContractNumberAsync(CancellationToken cancellationToken = default);
}
