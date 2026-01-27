using AutoMapper;
using FurniFlowUz.Application.DTOs.Auth;
using FurniFlowUz.Application.DTOs.Category;
using FurniFlowUz.Application.DTOs.CategoryAssignment;
using FurniFlowUz.Application.DTOs.Constructor;
using FurniFlowUz.Application.DTOs.Contract;
using FurniFlowUz.Application.DTOs.Customer;
using FurniFlowUz.Application.DTOs.Department;
using FurniFlowUz.Application.DTOs.DetailTask;
using FurniFlowUz.Application.DTOs.Employee;
using FurniFlowUz.Application.DTOs.MaterialAssignment;
using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Application.DTOs.Order;
using FurniFlowUz.Application.DTOs.Position;
using FurniFlowUz.Application.DTOs.Production;
using FurniFlowUz.Application.DTOs.TaskPerformance;
using FurniFlowUz.Application.DTOs.Warehouse;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between Domain entities and DTOs
/// </summary>
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        ConfigureUserMappings();
        ConfigureEmployeeMappings();
        ConfigurePositionMappings();
        ConfigureDepartmentMappings();
        ConfigureCustomerMappings();
        ConfigureCategoryMappings();
        ConfigureContractMappings();
        ConfigureOrderMappings();
        ConfigureConstructorMappings();
        ConfigureProductionMappings();
        ConfigureWarehouseMappings();
        ConfigureNotificationMappings();
        ConfigureCategoryAssignmentMappings();
        ConfigureDetailTaskMappings();
        ConfigureTaskPerformanceMappings();
        ConfigureMaterialAssignmentMappings();
    }

    /// <summary>
    /// Configure User entity mappings
    /// </summary>
    private void ConfigureUserMappings()
    {
        // User to UserDto
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? string.Empty));

        // RegisterRequestDto to User
        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedOrdersAsConstructor, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedOrdersAsProductionManager, opt => opt.Ignore())
            .ForMember(dest => dest.LeadingTeams, opt => opt.Ignore())
            .ForMember(dest => dest.TeamMemberships, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.WarehouseTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialRequests, opt => opt.Ignore())
            .ForMember(dest => dest.ConfirmedMaterialRequests, opt => opt.Ignore())
            .ForMember(dest => dest.Notifications, opt => opt.Ignore())
            .ForMember(dest => dest.AuditLogs, opt => opt.Ignore());

        // UpdateUserDto to User
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedOrdersAsConstructor, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedOrdersAsProductionManager, opt => opt.Ignore())
            .ForMember(dest => dest.LeadingTeams, opt => opt.Ignore())
            .ForMember(dest => dest.TeamMemberships, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.WarehouseTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialRequests, opt => opt.Ignore())
            .ForMember(dest => dest.ConfirmedMaterialRequests, opt => opt.Ignore())
            .ForMember(dest => dest.Notifications, opt => opt.Ignore())
            .ForMember(dest => dest.AuditLogs, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure Customer entity mappings
    /// </summary>
    private void ConfigureCustomerMappings()
    {
        // Customer to CustomerDto
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => ExtractFirstName(src.FullName)))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => ExtractLastName(src.FullName)));

        // CreateCustomerDto to Customer
        CreateMap<CreateCustomerDto, Customer>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Contracts, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());

        // UpdateCustomerDto to Customer
        CreateMap<UpdateCustomerDto, Customer>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Contracts, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure Category entity mappings
    /// </summary>
    private void ConfigureCategoryMappings()
    {
        // Category to CategoryDto
        CreateMap<Category, CategoryDto>().ReverseMap();

        // CreateCategoryDto to Category
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());

        // UpdateCategoryDto to Category
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure Contract entity mappings
    /// </summary>
    private void ConfigureContractMappings()
    {
        // Contract to ContractDto
        CreateMap<Contract, ContractDto>()
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => ParseCategoryIds(src.CategoryIds)))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

        // Contract to ContractSummaryDto
        CreateMap<Contract, ContractSummaryDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => ParseCategoryIds(src.CategoryIds)))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // CreateContractDto to Contract - mapping handled in service
        // UpdateContractDto to Contract - mapping handled in service
    }

    /// <summary>
    /// Helper method to parse comma-separated category IDs
    /// </summary>
    private static List<int> ParseCategoryIds(string categoryIds)
    {
        if (string.IsNullOrWhiteSpace(categoryIds))
            return new List<int>();

        return categoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => int.TryParse(id.Trim(), out var result) ? result : 0)
            .Where(id => id > 0)
            .ToList();
    }

    /// <summary>
    /// Configure Order entity mappings
    /// </summary>
    private void ConfigureOrderMappings()
    {
        // Order to OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.FurnitureTypes, opt => opt.MapFrom(src => src.FurnitureTypes))
            .ForMember(dest => dest.AssignedConstructor, opt => opt.MapFrom(src => src.AssignedConstructor))
            .ForMember(dest => dest.AssignedProductionManager, opt => opt.MapFrom(src => src.AssignedProductionManager))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Notes ?? string.Empty))
            .ForMember(dest => dest.ExpectedDeliveryDate, opt => opt.MapFrom(src => src.DeadlineDate))
            .ForMember(dest => dest.ActualDeliveryDate, opt => opt.MapFrom(src => src.CompletedAt));

        // Order to OrderSummaryDto
        CreateMap<Order, OrderSummaryDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.FurnitureTypesCount, opt => opt.MapFrom(src => src.FurnitureTypes.Count))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ExpectedDeliveryDate, opt => opt.MapFrom(src => src.DeadlineDate))
            .ForMember(dest => dest.AssignedConstructorName, opt => opt.MapFrom(src => src.AssignedConstructor != null ? $"{src.AssignedConstructor.FirstName} {src.AssignedConstructor.LastName}" : null))
            .ForMember(dest => dest.AssignedProductionManagerName, opt => opt.MapFrom(src => src.AssignedProductionManager != null ? $"{src.AssignedProductionManager.FirstName} {src.AssignedProductionManager.LastName}" : null));

        // CreateOrderDto to Order - NO LONGER USED
        // Order creation now derives all data from Contract in OrderService.CreateAsync
        // Mapping removed to prevent confusion

        // UpdateOrderDto to Order
        CreateMap<UpdateOrderDto, Order>()
            .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.ContractId, opt => opt.Ignore())
            .ForMember(dest => dest.DeadlineDate, opt => opt.MapFrom(src => src.ExpectedDeliveryDate))
            .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.ActualDeliveryDate))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProgressPercentage, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedConstructorId, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedProductionManagerId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Contract, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedConstructor, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedProductionManager, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureTypes, opt => opt.Ignore())
            .ForMember(dest => dest.WorkTasks, opt => opt.Ignore());

        // AssignOrderDto to Order (partial mapping)
        CreateMap<AssignOrderDto, Order>()
            .ForMember(dest => dest.AssignedConstructorId, opt => opt.MapFrom(src => src.ConstructorId.HasValue ? (int?)src.ConstructorId.Value : null))
            .ForMember(dest => dest.AssignedProductionManagerId, opt => opt.MapFrom(src => src.ProductionManagerId.HasValue ? (int?)src.ProductionManagerId.Value : null));
    }

    /// <summary>
    /// Configure Constructor module entity mappings (FurnitureType, Detail, Drawing, TechnicalSpecification)
    /// </summary>
    private void ConfigureConstructorMappings()
    {
        // FurnitureType to FurnitureTypeDto
        CreateMap<FurnitureType, FurnitureTypeDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order.OrderNumber))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details))
            .ForMember(dest => dest.Drawings, opt => opt.MapFrom(src => src.Drawings))
            .ForMember(dest => dest.TechnicalSpecification, opt => opt.MapFrom(src => src.TechnicalSpecification));

        // CreateFurnitureTypeDto to FurnitureType
        CreateMap<CreateFurnitureTypeDto, FurnitureType>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => (int)src.OrderId))
            .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TechnicalSpecificationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.TechnicalSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.Details, opt => opt.Ignore())
            .ForMember(dest => dest.Drawings, opt => opt.Ignore())
            .ForMember(dest => dest.WorkTasks, opt => opt.Ignore());

        // UpdateFurnitureTypeDto to FurnitureType
        CreateMap<UpdateFurnitureTypeDto, FurnitureType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.ProgressPercentage, opt => opt.Ignore())
            .ForMember(dest => dest.TechnicalSpecificationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.TechnicalSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.Details, opt => opt.Ignore())
            .ForMember(dest => dest.Drawings, opt => opt.Ignore())
            .ForMember(dest => dest.WorkTasks, opt => opt.Ignore());

        // Detail to DetailDto
        CreateMap<Detail, DetailDto>().ReverseMap();

        // CreateDetailDto to Detail
        CreateMap<CreateDetailDto, Detail>()
            .ForMember(dest => dest.FurnitureTypeId, opt => opt.MapFrom(src => (int)src.FurnitureTypeId))
            .ForMember(dest => dest.Material, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureType, opt => opt.Ignore());

        // UpdateDetailDto to Detail
        CreateMap<UpdateDetailDto, Detail>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureTypeId, opt => opt.Ignore())
            .ForMember(dest => dest.Material, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureType, opt => opt.Ignore());

        // Drawing to DrawingDto
        CreateMap<Drawing, DrawingDto>().ReverseMap();

        // TechnicalSpecification to TechnicalSpecificationDto
        CreateMap<TechnicalSpecification, TechnicalSpecificationDto>().ReverseMap();

        // CreateTechnicalSpecificationDto to TechnicalSpecification
        CreateMap<CreateTechnicalSpecificationDto, TechnicalSpecification>()
            .ForMember(dest => dest.FurnitureTypeId, opt => opt.MapFrom(src => (int)src.FurnitureTypeId))
            .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureType, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure Production module entity mappings (Team, WorkTask, ProductionStage)
    /// </summary>
    private void ConfigureProductionMappings()
    {
        // Team to TeamDto
        CreateMap<Team, TeamDto>()
            .ForMember(dest => dest.TeamLeader, opt => opt.MapFrom(src => src.TeamLeader))
            .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members));

        // CreateTeamDto to Team
        CreateMap<CreateTeamDto, Team>()
            .ForMember(dest => dest.TeamLeaderId, opt => opt.MapFrom(src => (int)src.TeamLeaderId))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TeamLeader, opt => opt.Ignore())
            .ForMember(dest => dest.Members, opt => opt.Ignore())
            .ForMember(dest => dest.WorkTasks, opt => opt.Ignore())
            .ForMember(dest => dest.WarehouseTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialRequests, opt => opt.Ignore());

        // UpdateTeamDto to Team
        CreateMap<UpdateTeamDto, Team>()
            .ForMember(dest => dest.TeamLeaderId, opt => opt.MapFrom(src => (int)src.TeamLeaderId))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TeamLeader, opt => opt.Ignore())
            .ForMember(dest => dest.Members, opt => opt.Ignore())
            .ForMember(dest => dest.WorkTasks, opt => opt.Ignore())
            .ForMember(dest => dest.WarehouseTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialRequests, opt => opt.Ignore());

        // WorkTask to WorkTaskDto
        CreateMap<WorkTask, WorkTaskDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order.OrderNumber))
            .ForMember(dest => dest.FurnitureTypeName, opt => opt.MapFrom(src => src.FurnitureType != null ? src.FurnitureType.Name : null))
            .ForMember(dest => dest.ProductionStage, opt => opt.MapFrom(src => src.ProductionStage))
            .ForMember(dest => dest.AssignedTeam, opt => opt.MapFrom(src => src.Team))
            .ForMember(dest => dest.AssignedWorker, opt => opt.MapFrom(src => src.AssignedWorker))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // CreateWorkTaskDto to WorkTask
        CreateMap<CreateWorkTaskDto, WorkTask>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => (int)src.OrderId))
            .ForMember(dest => dest.FurnitureTypeId, opt => opt.MapFrom(src => src.FurnitureTypeId.HasValue ? (int?)src.FurnitureTypeId.Value : null))
            .ForMember(dest => dest.ProductionStageId, opt => opt.MapFrom(src => (int)src.ProductionStageId))
            .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => (int)src.TeamId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.TaskStatus.Pending))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedWorkerId, opt => opt.Ignore())
            .ForMember(dest => dest.ActualHours, opt => opt.Ignore())
            .ForMember(dest => dest.StartedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureType, opt => opt.Ignore())
            .ForMember(dest => dest.ProductionStage, opt => opt.Ignore())
            .ForMember(dest => dest.Team, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedWorker, opt => opt.Ignore());

        // UpdateWorkTaskDto to WorkTask
        CreateMap<UpdateWorkTaskDto, WorkTask>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureTypeId, opt => opt.Ignore())
            .ForMember(dest => dest.ProductionStageId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.SequenceOrder, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedWorkerId, opt => opt.Ignore())
            .ForMember(dest => dest.ActualHours, opt => opt.Ignore())
            .ForMember(dest => dest.StartedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureType, opt => opt.Ignore())
            .ForMember(dest => dest.ProductionStage, opt => opt.Ignore())
            .ForMember(dest => dest.Team, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedWorker, opt => opt.Ignore());

        // ProductionStage to ProductionStageDto
        CreateMap<ProductionStage, ProductionStageDto>();

        CreateMap<ProductionStageDto, ProductionStage>()
            .ForMember(dest => dest.WorkTasks, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure Warehouse module entity mappings (WarehouseItem, WarehouseTransaction, MaterialRequest)
    /// </summary>
    private void ConfigureWarehouseMappings()
    {
        // WarehouseItem to WarehouseItemDto
        CreateMap<WarehouseItem, WarehouseItemDto>()
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()));

        // CreateWarehouseItemDto to WarehouseItem
        CreateMap<CreateWarehouseItemDto, WarehouseItem>()
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => Enum.Parse<UnitOfMeasurement>(src.Unit)))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore());

        // UpdateWarehouseItemDto to WarehouseItem
        CreateMap<UpdateWarehouseItemDto, WarehouseItem>()
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => Enum.Parse<UnitOfMeasurement>(src.Unit)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentStock, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore());

        // WarehouseTransaction to WarehouseTransactionDto
        CreateMap<WarehouseTransaction, WarehouseTransactionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.WarehouseItemName, opt => opt.MapFrom(src => src.WarehouseItem.Name))
            .ForMember(dest => dest.WarehouseItemSKU, opt => opt.MapFrom(src => src.WarehouseItem.SKU))
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null))
            .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"));

        // CreateIncomeTransactionDto to WarehouseTransaction
        CreateMap<CreateIncomeTransactionDto, WarehouseTransaction>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => WarehouseTransactionType.Income))
            .ForMember(dest => dest.WarehouseItemId, opt => opt.MapFrom(src => (int)src.WarehouseItemId))
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TeamId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.WarehouseItem, opt => opt.Ignore())
            .ForMember(dest => dest.Team, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialRequest, opt => opt.Ignore());

        // CreateOutcomeTransactionDto to WarehouseTransaction
        CreateMap<CreateOutcomeTransactionDto, WarehouseTransaction>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => WarehouseTransactionType.Outcome))
            .ForMember(dest => dest.WarehouseItemId, opt => opt.MapFrom(src => (int)src.WarehouseItemId))
            .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => (int)src.TeamId))
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.WarehouseItem, opt => opt.Ignore())
            .ForMember(dest => dest.Team, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialRequest, opt => opt.Ignore());

        // MaterialRequest to MaterialRequestDto
        CreateMap<MaterialRequest, MaterialRequestDto>()
            .ForMember(dest => dest.WarehouseTransaction, opt => opt.MapFrom(src => src.WarehouseTransaction))
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
            .ForMember(dest => dest.RequestedByUserName, opt => opt.MapFrom(src => $"{src.RequestedByUser.FirstName} {src.RequestedByUser.LastName}"))
            .ForMember(dest => dest.ConfirmedByUserName, opt => opt.MapFrom(src => src.ConfirmedByUser != null ? $"{src.ConfirmedByUser.FirstName} {src.ConfirmedByUser.LastName}" : null))
            .ForMember(dest => dest.ConfirmationStatus, opt => opt.MapFrom(src => src.ConfirmationStatus.ToString()));
    }

    /// <summary>
    /// Configure Notification entity mappings
    /// </summary>
    private void ConfigureNotificationMappings()
    {
        // Notification to NotificationDto
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.HasValue ? src.Role.Value.ToString() : null))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : null));

        // CreateNotificationDto to Notification
        CreateMap<CreateNotificationDto, Notification>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<NotificationType>(src.Type)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.HasValue ? (int?)src.UserId.Value : null))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Role) ? Enum.Parse<UserRole>(src.Role) : (UserRole?)null))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }

    /// <summary>
    /// Helper method to extract first name from full name
    /// </summary>
    private static string ExtractFirstName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    /// <summary>
    /// Helper method to extract last name from full name
    /// </summary>
    private static string ExtractLastName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
    }

    /// <summary>
    /// Helper method to calculate percentage
    /// </summary>
    private static decimal CalculatePercentage(decimal part, decimal total)
    {
        if (total == 0) return 0;
        return Math.Round((part / total) * 100, 2);
    }

    /// <summary>
    /// Configure Employee entity mappings
    /// </summary>
    private void ConfigureEmployeeMappings()
    {
        // Employee to EmployeeDto
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Position.Name))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role.ToString()));

        // CreateEmployeeWithUserDto to Employee
        CreateMap<CreateEmployeeWithUserDto, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Position, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        // UpdateEmployeeDto to Employee
        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Position, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.ActiveTasks, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.OnTimePercent, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure Position entity mappings
    /// </summary>
    private void ConfigurePositionMappings()
    {
        // Position to PositionDto
        CreateMap<Position, PositionDto>();

        // CreatePositionDto to Position
        CreateMap<CreatePositionDto, Position>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Employees, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        // UpdatePositionDto to Position
        CreateMap<UpdatePositionDto, Position>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Employees, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure Department entity mappings
    /// </summary>
    private void ConfigureDepartmentMappings()
    {
        // Department to DepartmentDto
        CreateMap<Department, DepartmentDto>();

        // CreateDepartmentDto to Department
        CreateMap<CreateDepartmentDto, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Employees, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        // UpdateDepartmentDto to Department
        CreateMap<UpdateDepartmentDto, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Employees, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure CategoryAssignment entity mappings
    /// </summary>
    private void ConfigureCategoryAssignmentMappings()
    {
        // CategoryAssignment to CategoryAssignmentDto
        CreateMap<Domain.Entities.CategoryAssignment, CategoryAssignmentDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureTypeName, opt => opt.Ignore())
            .ForMember(dest => dest.TeamLeaderName, opt => opt.Ignore())
            .ForMember(dest => dest.TeamName, opt => opt.Ignore())
            .ForMember(dest => dest.TotalTasks, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore());

        // CategoryAssignment to CategoryAssignmentSummaryDto
        CreateMap<Domain.Entities.CategoryAssignment, CategoryAssignmentSummaryDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureTypeName, opt => opt.Ignore())
            .ForMember(dest => dest.TeamLeaderName, opt => opt.Ignore())
            .ForMember(dest => dest.TeamName, opt => opt.Ignore())
            .ForMember(dest => dest.TaskProgress, opt => opt.Ignore())
            .ForMember(dest => dest.CompletionPercent, opt => opt.Ignore());

        // CreateCategoryAssignmentDto to CategoryAssignment
        CreateMap<CreateCategoryAssignmentDto, Domain.Entities.CategoryAssignment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CategoryAssignmentStatus.Assigned))
            .ForMember(dest => dest.AssignedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.StartedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureType, opt => opt.Ignore())
            .ForMember(dest => dest.TeamLeader, opt => opt.Ignore())
            .ForMember(dest => dest.Team, opt => opt.Ignore())
            .ForMember(dest => dest.DetailTasks, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure DetailTask entity mappings
    /// </summary>
    private void ConfigureDetailTaskMappings()
    {
        // DetailTask to DetailTaskDto
        CreateMap<Domain.Entities.DetailTask, DetailTaskDto>()
            .ForMember(dest => dest.DetailName, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedEmployeeName, opt => opt.Ignore())
            .ForMember(dest => dest.DependsOnTaskName, opt => opt.Ignore())
            .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
            .ForMember(dest => dest.FurnitureTypeName, opt => opt.Ignore())
            .ForMember(dest => dest.StartedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore());

        // DetailTask to DetailTaskListDto
        CreateMap<Domain.Entities.DetailTask, DetailTaskListDto>()
            .ForMember(dest => dest.DetailName, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedEmployeeName, opt => opt.Ignore())
            .ForMember(dest => dest.IsLocked, opt => opt.Ignore());

        // CreateDetailTaskDto to DetailTask
        CreateMap<CreateDetailTaskDto, Domain.Entities.DetailTask>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DetailTaskStatus.Pending))
            .ForMember(dest => dest.StartedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryAssignment, opt => opt.Ignore())
            .ForMember(dest => dest.Detail, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedEmployee, opt => opt.Ignore())
            .ForMember(dest => dest.DependsOnTask, opt => opt.Ignore())
            .ForMember(dest => dest.Performance, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure TaskPerformance entity mappings
    /// </summary>
    private void ConfigureTaskPerformanceMappings()
    {
        // TaskPerformance to TaskPerformanceDto
        CreateMap<Domain.Entities.TaskPerformance, TaskPerformanceDto>();

        // CreateTaskPerformanceDto to TaskPerformance
        CreateMap<CreateTaskPerformanceDto, Domain.Entities.TaskPerformance>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EfficiencyPercent, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DetailTask, opt => opt.Ignore());
    }

    /// <summary>
    /// Configure MaterialAssignment entity mappings
    /// </summary>
    private void ConfigureMaterialAssignmentMappings()
    {
        // MaterialAssignment to MaterialAssignmentDto
        CreateMap<Domain.Entities.MaterialAssignment, MaterialAssignmentDto>()
            .ForMember(dest => dest.RequestNumber, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialName, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialSKU, opt => opt.Ignore())
            .ForMember(dest => dest.Unit, opt => opt.Ignore())
            .ForMember(dest => dest.TeamName, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeName, opt => opt.Ignore())
            .ForMember(dest => dest.ReceivedByUserId, opt => opt.Ignore())
            .ForMember(dest => dest.ReceivedByUserName, opt => opt.Ignore());

        // CreateMaterialAssignmentDto to MaterialAssignment
        CreateMap<CreateMaterialAssignmentDto, Domain.Entities.MaterialAssignment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ReceivedConfirmed, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.ReceivedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialRequest, opt => opt.Ignore())
            .ForMember(dest => dest.WarehouseItem, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedToTeam, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedToEmployee, opt => opt.Ignore());
    }
}
