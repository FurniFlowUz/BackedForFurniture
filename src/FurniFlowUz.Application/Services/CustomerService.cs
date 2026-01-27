using AutoMapper;
using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Customer;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for customer management
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _unitOfWork.Customers.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException(nameof(Customer), id);
        }

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<PaginatedResult<CustomerDto>> GetPagedAsync(BaseFilter filter, CancellationToken cancellationToken = default)
    {
        // Build search predicate if search term is provided
        System.Linq.Expressions.Expression<Func<Customer, bool>>? predicate = null;
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            predicate = c => c.FullName.ToLower().Contains(searchTerm) ||
                           c.PhoneNumber.Contains(searchTerm) ||
                           (c.Email != null && c.Email.ToLower().Contains(searchTerm));
        }

        var customers = await _unitOfWork.Customers.GetPagedAsync(
            filter.PageNumber,
            filter.PageSize,
            predicate,
            orderBy: q => q.OrderBy(c => c.FullName),
            cancellationToken: cancellationToken);

        var totalCount = await _unitOfWork.Customers.CountAsync(cancellationToken);

        var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);

        return new PaginatedResult<CustomerDto>
        {
            Items = customerDtos.ToList(),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto request, CancellationToken cancellationToken = default)
    {
        // Check if phone number already exists
        var existingCustomers = await _unitOfWork.Customers.FindAsync(
            c => c.PhoneNumber == request.PhoneNumber,
            cancellationToken);

        if (existingCustomers.Any())
        {
            throw new ValidationException($"Customer with phone number '{request.PhoneNumber}' already exists.");
        }

        // Check if email already exists (if provided)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await _unitOfWork.Customers.FindAsync(
                c => c.Email != null && c.Email.ToLower() == request.Email.ToLower(),
                cancellationToken);

            if (emailExists.Any())
            {
                throw new ValidationException($"Customer with email '{request.Email}' already exists.");
            }
        }

        var customer = new Customer
        {
            FullName = $"{request.FirstName} {request.LastName}".Trim(),
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto request, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException(nameof(Customer), id);
        }

        // Check if new phone number conflicts with existing customers
        if (customer.PhoneNumber != request.PhoneNumber)
        {
            var existingCustomers = await _unitOfWork.Customers.FindAsync(
                c => c.PhoneNumber == request.PhoneNumber && c.Id != id,
                cancellationToken);

            if (existingCustomers.Any())
            {
                throw new ValidationException($"Customer with phone number '{request.PhoneNumber}' already exists.");
            }
        }

        // Check if new email conflicts with existing customers (if provided)
        if (!string.IsNullOrWhiteSpace(request.Email) && customer.Email != request.Email)
        {
            var emailExists = await _unitOfWork.Customers.FindAsync(
                c => c.Email != null && c.Email.ToLower() == request.Email.ToLower() && c.Id != id,
                cancellationToken);

            if (emailExists.Any())
            {
                throw new ValidationException($"Customer with email '{request.Email}' already exists.");
            }
        }

        customer.FullName = $"{request.FirstName} {request.LastName}".Trim();
        customer.PhoneNumber = request.PhoneNumber;
        customer.Email = request.Email;
        customer.Address = request.Address;
        customer.Notes = request.Notes;
        customer.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException(nameof(Customer), id);
        }

        // Check if customer has associated contracts or orders
        var contracts = await _unitOfWork.Contracts.FindAsync(c => c.CustomerId == id, cancellationToken);
        if (contracts.Any())
        {
            throw new BusinessException("Cannot delete customer with associated contracts.");
        }

        var orders = await _unitOfWork.Orders.FindAsync(o => o.CustomerId == id, cancellationToken);
        if (orders.Any())
        {
            throw new BusinessException("Cannot delete customer with associated orders.");
        }

        _unitOfWork.Customers.Remove(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<CustomerDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        var searchLower = searchTerm.ToLower().Trim();

        var customers = await _unitOfWork.Customers.FindAsync(
            c => c.FullName.ToLower().Contains(searchLower) ||
                 (c.PhoneNumber != null && c.PhoneNumber.Contains(searchTerm)) ||
                 (c.Email != null && c.Email.ToLower().Contains(searchLower)),
            cancellationToken);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }
}
