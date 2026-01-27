using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Customer;
using FurniFlowUz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniFlowUz.API.Controllers;

/// <summary>
/// Controller for customer management operations
/// Used by Salesperson and Director roles
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Salesperson,Director")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all customers
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all customers</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomerDto>>>> GetCustomers(
        CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(customers, "Customers retrieved successfully"));
    }

    /// <summary>
    /// Gets a specific customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomer(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<CustomerDto>.SuccessResponse(customer, "Customer retrieved successfully"));
    }

    /// <summary>
    /// Creates a new customer
    /// Used by Salesperson when meeting with a new client
    /// </summary>
    /// <param name="request">Customer creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created customer information</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> CreateCustomer(
        [FromBody] CreateCustomerDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CustomerDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var customer = await _customerService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<CustomerDto>.SuccessResponse(customer, "Customer created successfully"));
    }

    /// <summary>
    /// Updates an existing customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="request">Updated customer data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated customer information</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> UpdateCustomer(
        [FromRoute] int id,
        [FromBody] UpdateCustomerDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CustomerDto>.FailureResponse(
                "Invalid request data",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var customer = await _customerService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<CustomerDto>.SuccessResponse(customer, "Customer updated successfully"));
    }

    /// <summary>
    /// Deletes a customer (soft delete)
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCustomer(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _customerService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Customer deleted successfully"));
    }

    /// <summary>
    /// Searches customers by name, phone, or email
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching customers</returns>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomerDto>>>> SearchCustomers(
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        var customers = await _customerService.SearchAsync(searchTerm, cancellationToken);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(customers, "Search completed successfully"));
    }
}
