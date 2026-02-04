using FurniFlowUz.Application.DTOs.User;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for user-related operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets all available constructors for order assignment
    /// Returns only active users with Constructor role and their employee records
    /// </summary>
    /// <returns>List of available constructors with ID, name, and phone</returns>
    Task<List<ConstructorDto>> GetAvailableConstructorsAsync();

    /// <summary>
    /// Gets all available production managers for order assignment
    /// Returns only active users with ProductionManager role and their employee records
    /// </summary>
    /// <returns>List of available production managers with ID, name, and phone</returns>
    Task<List<ConstructorDto>> GetAvailableProductionManagersAsync();

    /// <summary>
    /// Gets all available team leaders for task assignment
    /// Returns only active users with TeamLeader role and their employee records
    /// </summary>
    /// <returns>List of available team leaders with ID, name, and phone</returns>
    Task<List<ConstructorDto>> GetAvailableTeamLeadersAsync();

    /// <summary>
    /// Gets all available team leaders with assignment statistics
    /// Returns team leaders with department, position, and active assignment counts
    /// </summary>
    /// <returns>List of team leaders with statistics</returns>
    Task<List<TeamLeaderDto>> GetTeamLeadersWithStatsAsync();

    /// <summary>
    /// Gets all furniture types (for constructor view)
    /// Returns furniture types with basic info and counts
    /// </summary>
    /// <returns>List of furniture types with summary information</returns>
    Task<List<FurnitureTypeListDto>> GetFurnitureTypesAsync();

    /// <summary>
    /// Gets a specific furniture type by ID
    /// Returns furniture type with basic info and counts
    /// </summary>
    /// <param name="id">Furniture type ID</param>
    /// <returns>Furniture type details or null if not found</returns>
    Task<FurnitureTypeListDto?> GetFurnitureTypeByIdAsync(int id);

    /// <summary>
    /// Gets orders assigned to the current user (as constructor)
    /// Returns only orders where the current user is assigned as constructor
    /// </summary>
    /// <returns>List of assigned orders with summary information</returns>
    Task<List<AssignedOrderDto>> GetOrdersAssignedToConstructorAsync();

    /// <summary>
    /// Gets orders assigned to the current user (as production manager)
    /// Returns only orders where the current user is assigned as production manager
    /// </summary>
    /// <returns>List of assigned orders with summary information</returns>
    Task<List<AssignedOrderDto>> GetOrdersAssignedToProductionManagerAsync();

    /// <summary>
    /// Creates a new ProductionManager user account
    /// Creates both User record (for authentication) and Employee record (for assignment)
    /// AUTHORIZATION: Only Director can create ProductionManager accounts
    /// </summary>
    /// <param name="request">ProductionManager creation request</param>
    /// <returns>Created ProductionManager information</returns>
    Task<ProductionManagerDto> CreateProductionManagerAsync(CreateProductionManagerDto request);
}
