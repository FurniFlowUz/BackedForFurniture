using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Customer;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for customer management
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Gets all customers
    /// </summary>
    Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a customer by ID
    /// </summary>
    Task<CustomerDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated customers with search functionality
    /// </summary>
    Task<PaginatedResult<CustomerDto>> GetPagedAsync(BaseFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new customer
    /// </summary>
    Task<CustomerDto> CreateAsync(CreateCustomerDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing customer
    /// </summary>
    Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a customer (soft delete by setting IsActive to false)
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches customers by name, phone number, or email
    /// </summary>
    Task<IEnumerable<CustomerDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
