using AutoMapper;
using FurniFlowUz.Application.DTOs.Common;
using FurniFlowUz.Application.DTOs.Contract;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Repositories;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for contract management
/// </summary>
public class ContractService : IContractService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public ContractService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<ContractDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var contracts = await _unitOfWork.Contracts.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<ContractDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Contracts.GetByIdAsync(id, cancellationToken);
        if (contract == null)
        {
            throw new NotFoundException(nameof(Contract), id);
        }

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<PaginatedResult<ContractSummaryDto>> GetPagedAsync(ContractFilterDto filter, CancellationToken cancellationToken = default)
    {
        // Build filter predicate
        var predicate = BuildFilterPredicate(filter);

        // Get paginated data with Customer included
        var contracts = await _unitOfWork.Contracts.GetPagedAsync(
            filter.PageNumber,
            filter.PageSize,
            predicate,
            orderBy: q => q.OrderByDescending(c => c.CreatedAt),
            includeProperties: "Customer",
            cancellationToken);

        var totalCount = await _unitOfWork.Contracts.CountAsync(cancellationToken);

        // Load all categories once for efficient lookup
        var allCategories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        var categoryDict = allCategories.ToDictionary(c => c.Id, c => c.Name);

        // Load all users (sellers) once for efficient lookup
        var allUsers = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var userDict = allUsers.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

        // Project to DTOs with complete data
        var contractDtos = contracts.Select(contract => new ContractSummaryDto
        {
            Id = contract.Id,
            ContractNumber = contract.ContractNumber,
            CustomerName = contract.Customer?.FullName ?? "Unknown",

            // Resolve seller name from CreatedBy
            SellerName = contract.CreatedBy.HasValue && userDict.ContainsKey(contract.CreatedBy.Value)
                ? userDict[contract.CreatedBy.Value]
                : null,

            // Parse CategoryIds and resolve names
            CategoryIds = string.IsNullOrEmpty(contract.CategoryIds)
                ? new List<int>()
                : contract.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var catId) ? catId : 0)
                    .Where(id => id > 0)
                    .ToList(),

            CategoryNames = string.IsNullOrEmpty(contract.CategoryIds)
                ? new List<string>()
                : contract.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var catId) ? catId : 0)
                    .Where(id => id > 0 && categoryDict.ContainsKey(id))
                    .Select(id => categoryDict[id])
                    .ToList(),

            TotalAmount = contract.TotalAmount,
            AdvancePaymentAmount = contract.AdvancePaymentAmount,
            RemainingAmount = contract.RemainingAmount,
            DeadlineDate = contract.DeadlineDate,
            PaymentStatus = contract.PaymentStatus.ToString(),
            Status = contract.Status.ToString(),
            RequiresApproval = contract.RequiresApproval,
            CreatedAt = contract.CreatedAt
        }).ToList();

        return new PaginatedResult<ContractSummaryDto>
        {
            Items = contractDtos,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<ContractDto> CreateAsync(CreateContractDto request, CancellationToken cancellationToken = default)
    {
        // BACKWARD COMPATIBILITY: Convert old format to new format

        // Convert single CategoryId to CategoryIds list
        if (request.CategoryId.HasValue && (request.CategoryIds == null || !request.CategoryIds.Any()))
        {
            request.CategoryIds = new List<int> { request.CategoryId.Value };
        }

        // Convert AdvancePaymentPercentage to AdvancePaymentAmount
        if (request.AdvancePaymentPercentage.HasValue && request.AdvancePaymentAmount == 0)
        {
            request.AdvancePaymentAmount = request.TotalAmount * (request.AdvancePaymentPercentage.Value / 100.0m);
        }


        // Convert deprecated fields to new fields
        if (!string.IsNullOrEmpty(request.Description) && string.IsNullOrEmpty(request.AdditionalNotes))
        {
            request.AdditionalNotes = request.Description;
        }
        else if (!string.IsNullOrEmpty(request.Notes) && string.IsNullOrEmpty(request.AdditionalNotes))
        {
            request.AdditionalNotes = request.Notes;
        }

        // Convert Terms to DeliveryTerms and PenaltyTerms
        if (!string.IsNullOrEmpty(request.Terms) && string.IsNullOrEmpty(request.DeliveryTerms) && string.IsNullOrEmpty(request.PenaltyTerms))
        {
            request.DeliveryTerms = request.Terms;
        }

        // Validate advance payment amount
        if (request.AdvancePaymentAmount < 0)
        {
            throw new ValidationException("Advance payment amount must be 0 or greater.");
        }

        if (request.AdvancePaymentAmount > request.TotalAmount)
        {
            throw new ValidationException("Advance payment amount cannot exceed total amount.");
        }

        // ===================================================================
        // CUSTOMER HANDLING: Support both existing and new customer flows
        // ===================================================================
        int customerId;
        Customer customer;

        // Validate: Either CustomerId OR NewCustomer must be provided (enforced by validator)
        bool hasCustomerId = request.CustomerId.HasValue && request.CustomerId.Value > 0;
        bool hasNewCustomer = request.NewCustomer != null;

        if (!hasCustomerId && !hasNewCustomer)
        {
            throw new ValidationException("Either CustomerId or NewCustomer must be provided.");
        }

        if (hasCustomerId && hasNewCustomer)
        {
            throw new ValidationException("Cannot provide both CustomerId and NewCustomer. Choose one.");
        }

        if (hasCustomerId)
        {
            // EXISTING CUSTOMER FLOW
            customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId!.Value, cancellationToken);
            if (customer == null)
            {
                throw new NotFoundException(nameof(Customer), request.CustomerId!.Value);
            }
            customerId = customer.Id;
        }
        else
        {
            // NEW CUSTOMER FLOW
            var newCustomerData = request.NewCustomer!;

            // Check if phone number already exists
            var existingCustomers = await _unitOfWork.Customers.FindAsync(
                c => c.PhoneNumber == newCustomerData.PhoneNumber,
                cancellationToken);

            if (existingCustomers.Any())
            {
                throw new ValidationException($"Customer with phone number '{newCustomerData.PhoneNumber}' already exists. Please use the existing customer.");
            }

            // Check if email already exists (if provided)
            if (!string.IsNullOrWhiteSpace(newCustomerData.Email))
            {
                var emailExists = await _unitOfWork.Customers.FindAsync(
                    c => c.Email != null && c.Email.ToLower() == newCustomerData.Email.ToLower(),
                    cancellationToken);

                if (emailExists.Any())
                {
                    throw new ValidationException($"Customer with email '{newCustomerData.Email}' already exists. Please use the existing customer.");
                }
            }

            // Create the new customer
            customer = new Customer
            {
                FullName = newCustomerData.FullName.Trim(),
                PhoneNumber = newCustomerData.PhoneNumber,
                Email = newCustomerData.Email,
                Address = newCustomerData.Address,
                Notes = newCustomerData.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            customerId = customer.Id;
        }

        // Validate categories exist
        if (request.CategoryIds == null || !request.CategoryIds.Any())
        {
            throw new ValidationException("At least one category is required.");
        }

        foreach (var categoryId in request.CategoryIds)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), categoryId);
            }
        }

        // Calculate remaining amount
        var remainingAmount = request.TotalAmount - request.AdvancePaymentAmount;

        // Generate contract number
        var contractNumber = await GenerateContractNumberAsync(cancellationToken);

        // Create contract
        var contract = new Contract
        {
            ContractNumber = contractNumber,
            CustomerId = customerId,
            CategoryIds = string.Join(",", request.CategoryIds),
            TotalAmount = request.TotalAmount,
            AdvancePaymentAmount = request.AdvancePaymentAmount,
            RemainingAmount = remainingAmount,
            PaymentStatus = PaymentStatus.Pending,
            Status = ContractStatus.New,
            DeadlineDate = request.DeadlineDate,
            SignedDate = request.SignedDate,
            DeliveryTerms = request.DeliveryTerms,
            PenaltyTerms = request.PenaltyTerms,
            AdditionalNotes = request.AdditionalNotes,
            RequiresApproval = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Contracts.AddAsync(contract, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to Director and Salesperson
        await _notificationService.CreateNotificationAsync(new DTOs.Notification.CreateNotificationDto
        {
            Title = "Yangi Shartnoma Yaratildi",
            Message = $"{contractNumber} raqamli shartnoma {customer.FullName} mijoz uchun yaratildi.",
            Type = NotificationType.OrderStatusChanged.ToString(),
            Role = UserRole.Director.ToString()
        }, cancellationToken);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> UpdateAsync(int id, UpdateContractDto request, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Contracts.GetByIdAsync(id, cancellationToken);
        if (contract == null)
        {
            throw new NotFoundException(nameof(Contract), id);
        }

        // Validate advance payment amount
        if (request.AdvancePaymentAmount < 0)
        {
            throw new ValidationException("Advance payment amount must be 0 or greater.");
        }

        if (request.AdvancePaymentAmount > request.TotalAmount)
        {
            throw new ValidationException("Advance payment amount cannot exceed total amount.");
        }

        // Validate categories exist
        if (request.CategoryIds == null || !request.CategoryIds.Any())
        {
            throw new ValidationException("At least one category is required.");
        }

        foreach (var categoryId in request.CategoryIds)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), categoryId);
            }
        }

        // Calculate remaining amount
        var remainingAmount = request.TotalAmount - request.AdvancePaymentAmount;

        // Update contract
        contract.CategoryIds = string.Join(",", request.CategoryIds);
        contract.TotalAmount = request.TotalAmount;
        contract.AdvancePaymentAmount = request.AdvancePaymentAmount;
        contract.RemainingAmount = remainingAmount;
        if (!string.IsNullOrEmpty(request.PaymentStatus))
            contract.PaymentStatus = Enum.Parse<PaymentStatus>(request.PaymentStatus);
        contract.Status = Enum.Parse<ContractStatus>(request.Status);
        contract.DeadlineDate = request.DeadlineDate;
        contract.SignedDate = request.SignedDate;
        contract.DeliveryTerms = request.DeliveryTerms;
        contract.PenaltyTerms = request.PenaltyTerms;
        contract.AdditionalNotes = request.AdditionalNotes;
        contract.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Contracts.Update(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContractDto>(contract);
    }

    public async Task UpdateStatusAsync(int id, ContractStatus status, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Contracts.GetByIdAsync(id, cancellationToken);
        if (contract == null)
        {
            throw new NotFoundException(nameof(Contract), id);
        }

        // Business rule validation for status changes
        ValidateStatusChange(contract, status);

        contract.Status = status;
        contract.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Contracts.Update(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Validates if a status change is allowed based on business rules
    /// </summary>
    private void ValidateStatusChange(Contract contract, ContractStatus newStatus)
    {
        // Cannot change status of completed contracts
        if (contract.Status == ContractStatus.Completed)
        {
            throw new BusinessException("Cannot update the status of a completed contract.");
        }

        // Cannot change status of cancelled contracts
        if (contract.Status == ContractStatus.Cancelled)
        {
            throw new BusinessException("Cannot update the status of a cancelled contract.");
        }

        // Cannot set status to the same value
        if (contract.Status == newStatus)
        {
            throw new BusinessException($"Contract is already in '{newStatus}' status.");
        }

        // Cannot go directly from New to Completed
        if (contract.Status == ContractStatus.New && newStatus == ContractStatus.Completed)
        {
            throw new BusinessException("Cannot complete a contract directly from 'New' status. Activate the contract first.");
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var contract = await _unitOfWork.Contracts.GetByIdAsync(id, cancellationToken);
        if (contract == null)
        {
            throw new NotFoundException(nameof(Contract), id);
        }

        // Check if contract has associated orders
        var orders = await _unitOfWork.Orders.FindAsync(o => o.ContractId == id, cancellationToken);
        if (orders.Any())
        {
            throw new BusinessException("Cannot delete contract with associated orders.");
        }

        _unitOfWork.Contracts.Remove(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> GenerateContractNumberAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow;
        var prefix = $"SH-{today:yyyy}";

        // CRITICAL: We need to check ALL contracts including soft-deleted ones
        // to avoid reusing contract numbers. The repository has a global query filter
        // that excludes soft-deleted records by default, so we use FindIgnoringQueryFiltersAsync.

        // Get all contracts with this year's prefix (including soft-deleted)
        var yearContracts = await _unitOfWork.Contracts.FindIgnoringQueryFiltersAsync(
            c => c.ContractNumber.StartsWith(prefix),
            cancellationToken);

        int maxSequence = 0;
        if (yearContracts.Any())
        {
            // Extract sequence numbers from existing contract numbers
            foreach (var contract in yearContracts)
            {
                var parts = contract.ContractNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out var seq))
                {
                    if (seq > maxSequence)
                    {
                        maxSequence = seq;
                    }
                }
            }
        }

        // Increment to get next sequence
        var sequence = maxSequence + 1;
        var contractNumber = $"{prefix}-{sequence:D4}";

        // Double-check that this number doesn't already exist (race condition protection)
        // Also ignore query filters here to check all contracts including soft-deleted
        var existing = await _unitOfWork.Contracts.FindIgnoringQueryFiltersAsync(
            c => c.ContractNumber == contractNumber,
            cancellationToken);

        if (existing.Any())
        {
            // If collision detected, try next number
            sequence++;
            contractNumber = $"{prefix}-{sequence:D4}";
        }

        return contractNumber;
    }

    #region Private Helper Methods

    private System.Linq.Expressions.Expression<Func<Contract, bool>> BuildFilterPredicate(ContractFilterDto filter)
    {
        var predicate = System.Linq.Expressions.Expression.Lambda<Func<Contract, bool>>(
            System.Linq.Expressions.Expression.Constant(true),
            System.Linq.Expressions.Expression.Parameter(typeof(Contract), "c"));

        // Apply filters based on ContractFilterDto properties
        // This is a simplified version - you can enhance it based on actual filter properties

        return predicate;
    }

    #endregion
}
