using FurniFlowUz.Application.DTOs.User;
using FurniFlowUz.Application.Exceptions;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Domain.Interfaces;
using FurniFlowUz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.Application.Services;

/// <summary>
/// Service for user-related operations
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public UserService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets all available constructors for order assignment
    /// EFFICIENT QUERY - Joins Employees with Users, filters by role and active status
    /// Returns display-ready data sorted by name
    /// </summary>
    public async Task<List<ConstructorDto>> GetAvailableConstructorsAsync()
    {
        return await _dbContext.Employees
            .Include(e => e.User)
            .Where(e =>
                e.IsActive &&                           // ✅ Employee must be active
                e.User.IsActive &&                      // ✅ User must be active
                e.User.Role == UserRole.Constructor)    // ✅ User must have Constructor role
            .OrderBy(e => e.FullName)                   // ✅ Ordered alphabetically
            .Select(e => new ConstructorDto
            {
                Id = e.Id,                              // ✅ Employee ID for order assignment
                FullName = e.FullName,
                Phone = e.Phone
            })
            .ToListAsync();
    }

    /// <summary>
    /// Gets all available production managers for order assignment
    /// EFFICIENT QUERY - Joins Employees with Users, filters by role and active status
    /// Returns display-ready data sorted by name
    /// </summary>
    public async Task<List<ConstructorDto>> GetAvailableProductionManagersAsync()
    {
        return await _dbContext.Employees
            .Include(e => e.User)
            .Where(e =>
                e.IsActive &&                                  // ✅ Employee must be active
                e.User.IsActive &&                             // ✅ User must be active
                e.User.Role == UserRole.ProductionManager)     // ✅ User must have ProductionManager role
            .OrderBy(e => e.FullName)                          // ✅ Ordered alphabetically
            .Select(e => new ConstructorDto
            {
                Id = e.Id,                                     // ✅ Employee ID for order assignment
                FullName = e.FullName,
                Phone = e.Phone
            })
            .ToListAsync();
    }

    /// <summary>
    /// Gets all available team leaders for task assignment
    /// EFFICIENT QUERY - Joins Employees with Users, filters by role and active status
    /// Returns display-ready data sorted by name
    /// </summary>
    public async Task<List<ConstructorDto>> GetAvailableTeamLeadersAsync()
    {
        return await _dbContext.Employees
            .Include(e => e.User)
            .Where(e =>
                e.IsActive &&                                  // ✅ Employee must be active
                e.User.IsActive &&                             // ✅ User must be active
                e.User.Role == UserRole.TeamLeader)            // ✅ User must have TeamLeader role
            .OrderBy(e => e.FullName)                          // ✅ Ordered alphabetically
            .Select(e => new ConstructorDto
            {
                Id = e.Id,                                     // ✅ Employee ID for task assignment
                FullName = e.FullName,
                Phone = e.Phone
            })
            .ToListAsync();
    }

    /// <summary>
    /// Gets all available team leaders with assignment statistics
    /// EFFICIENT QUERY - Includes department, position, and counts active/completed assignments
    /// </summary>
    public async Task<List<TeamLeaderDto>> GetTeamLeadersWithStatsAsync()
    {
        // First, get all team leader users with their employee info
        var teamLeaderUsers = await _dbContext.Users
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Department)
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Position)
            .Where(u =>
                u.IsActive &&
                u.Role == UserRole.TeamLeader &&
                u.Employee != null &&
                u.Employee.IsActive)
            .ToListAsync();

        // Get assignment counts for these team leaders
        var teamLeaderIds = teamLeaderUsers.Select(u => u.Id).ToList();

        var assignmentStats = await _dbContext.CategoryAssignments
            .Where(ca => teamLeaderIds.Contains(ca.TeamLeaderId))
            .GroupBy(ca => ca.TeamLeaderId)
            .Select(g => new
            {
                TeamLeaderId = g.Key,
                ActiveCount = g.Count(ca => ca.Status == CategoryAssignmentStatus.Assigned || ca.Status == CategoryAssignmentStatus.InProgress),
                CompletedCount = g.Count(ca => ca.Status == CategoryAssignmentStatus.Completed)
            })
            .ToListAsync();

        // Get team names for these team leaders
        var teamNames = await _dbContext.Teams
            .Where(t => teamLeaderIds.Contains(t.TeamLeaderId))
            .Select(t => new { t.TeamLeaderId, t.Name })
            .ToListAsync();

        // Combine data
        return teamLeaderUsers
            .OrderBy(u => u.Employee!.FullName)
            .Select(u =>
            {
                var stats = assignmentStats.FirstOrDefault(s => s.TeamLeaderId == u.Id);
                var teamName = teamNames.FirstOrDefault(t => t.TeamLeaderId == u.Id)?.Name;

                return new TeamLeaderDto
                {
                    Id = u.Id,  // User ID for assignment
                    FullName = u.Employee!.FullName,
                    Phone = u.Employee.Phone,
                    Department = u.Employee.Department?.Name ?? "-",
                    Position = u.Employee.Position?.Name ?? "Team Leader",
                    ActiveAssignments = stats?.ActiveCount ?? 0,
                    CompletedAssignments = stats?.CompletedCount ?? 0,
                    TeamName = teamName
                };
            })
            .ToList();
    }

    /// <summary>
    /// Gets all furniture types for constructor view with role-based filtering
    /// EFFICIENT QUERY - Joins with Order and Customer, includes counts
    /// Returns ONLY furniture types from orders accessible to the current user
    /// </summary>
    public async Task<List<FurnitureTypeListDto>> GetFurnitureTypesAsync()
    {
        var query = _dbContext.FurnitureTypes
            .Include(ft => ft.Order)
                .ThenInclude(o => o.Customer)
            .Include(ft => ft.Details)
            .Include(ft => ft.Drawings)
            .AsQueryable();

        // ✅ Apply role-based filtering
        if (_currentUserService.IsAuthenticated && _currentUserService.UserId.HasValue)
        {
            var currentUserId = _currentUserService.UserId.Value;
            var currentRole = _currentUserService.Role;

            if (Enum.TryParse<UserRole>(currentRole, out var userRole))
            {
                switch (userRole)
                {
                    case UserRole.Constructor:
                        // Constructors see ONLY furniture types from orders assigned to them
                        query = query.Where(ft => ft.Order.AssignedConstructorId == currentUserId);
                        break;

                    case UserRole.ProductionManager:
                        // Production Managers see ONLY furniture types from orders assigned to them
                        query = query.Where(ft => ft.Order.AssignedProductionManagerId == currentUserId);
                        break;

                    case UserRole.Salesperson:
                        // Salespeople see ONLY furniture types from orders they created
                        query = query.Where(ft => ft.Order.CreatedBy == currentUserId);
                        break;

                    case UserRole.Director:
                        // Directors see ALL furniture types
                        break;

                    default:
                        // Other roles see nothing
                        return new List<FurnitureTypeListDto>();
                }
            }
        }

        return await query
            .OrderByDescending(ft => ft.CreatedAt)           // ✅ Most recent first
            .Select(ft => new FurnitureTypeListDto
            {
                Id = ft.Id,
                Name = ft.Name,
                OrderId = ft.OrderId,
                OrderNumber = ft.Order.OrderNumber,
                ProgressPercentage = ft.ProgressPercentage,
                DetailsCount = ft.Details.Count,             // ✅ Count of components
                DrawingsCount = ft.Drawings.Count,           // ✅ Count of drawings
                CustomerName = ft.Order.Customer != null
                    ? ft.Order.Customer.FullName
                    : null,                                  // ✅ Customer name if available
                HasTechnicalSpecification = ft.TechnicalSpecificationId.HasValue
            })
            .ToListAsync();
    }

    /// <summary>
    /// Gets a specific furniture type by ID with role-based access control
    /// EFFICIENT QUERY - Joins with Order and Customer, includes counts
    /// Returns display-ready data or null if not found or user lacks access
    /// </summary>
    public async Task<FurnitureTypeListDto?> GetFurnitureTypeByIdAsync(int id)
    {
        var query = _dbContext.FurnitureTypes
            .Include(ft => ft.Order)
                .ThenInclude(o => o.Customer)
            .Include(ft => ft.Details)
            .Include(ft => ft.Drawings)
            .Where(ft => ft.Id == id)                        // ✅ Filter by ID
            .AsQueryable();

        // ✅ Apply role-based access control
        if (_currentUserService.IsAuthenticated && _currentUserService.UserId.HasValue)
        {
            var currentUserId = _currentUserService.UserId.Value;
            var currentRole = _currentUserService.Role;

            if (Enum.TryParse<UserRole>(currentRole, out var userRole))
            {
                switch (userRole)
                {
                    case UserRole.Constructor:
                        // Constructors can ONLY access furniture types from assigned orders
                        query = query.Where(ft => ft.Order.AssignedConstructorId == currentUserId);
                        break;

                    case UserRole.ProductionManager:
                        // Production Managers can ONLY access furniture types from assigned orders
                        query = query.Where(ft => ft.Order.AssignedProductionManagerId == currentUserId);
                        break;

                    case UserRole.Salesperson:
                        // Salespeople can ONLY access furniture types from their created orders
                        query = query.Where(ft => ft.Order.CreatedBy == currentUserId);
                        break;

                    case UserRole.Director:
                        // Directors can access ALL furniture types
                        break;

                    default:
                        // Other roles have no access
                        return null;
                }
            }
        }

        return await query
            .Select(ft => new FurnitureTypeListDto
            {
                Id = ft.Id,
                Name = ft.Name,
                OrderId = ft.OrderId,
                OrderNumber = ft.Order.OrderNumber,
                ProgressPercentage = ft.ProgressPercentage,
                DetailsCount = ft.Details.Count,             // ✅ Count of components
                DrawingsCount = ft.Drawings.Count,           // ✅ Count of drawings
                CustomerName = ft.Order.Customer != null
                    ? ft.Order.Customer.FullName
                    : null,                                  // ✅ Customer name if available
                HasTechnicalSpecification = ft.TechnicalSpecificationId.HasValue
            })
            .FirstOrDefaultAsync();                          // ✅ Returns null if not found or no access
    }

    /// <summary>
    /// Gets orders assigned to the current user (as constructor)
    /// EFFICIENT QUERY - Gets current user's employee ID, then filters orders
    /// Returns display-ready data sorted by deadline
    /// </summary>
    public async Task<List<AssignedOrderDto>> GetOrdersAssignedToConstructorAsync()
    {
        // Get current user's employee ID
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return new List<AssignedOrderDto>();
        }

        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId.Value && e.IsActive);

        if (employee == null)
        {
            return new List<AssignedOrderDto>();
        }

        return await _dbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.Category)
            .Include(o => o.FurnitureTypes)
            .Where(o => o.AssignedConstructorId == employee.Id)  // ✅ Filter by current user's employee ID
            .OrderBy(o => o.DeadlineDate)                        // ✅ Ordered by deadline (earliest first)
            .Select(o => new AssignedOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.Customer.FullName,
                CategoryName = o.Category.Name,
                Status = o.Status,
                ProgressPercentage = o.ProgressPercentage,
                DeadlineDate = o.DeadlineDate,
                FurnitureTypesCount = o.FurnitureTypes.Count,   // ✅ Count of furniture types
                CreatedAt = o.CreatedAt,
                Notes = o.Notes
            })
            .ToListAsync();
    }

    /// <summary>
    /// Gets orders assigned to the current user (as production manager)
    /// EFFICIENT QUERY - Gets current user's employee ID, then filters orders
    /// Returns display-ready data sorted by deadline
    /// </summary>
    public async Task<List<AssignedOrderDto>> GetOrdersAssignedToProductionManagerAsync()
    {
        // Get current user's employee ID
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return new List<AssignedOrderDto>();
        }

        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId.Value && e.IsActive);

        if (employee == null)
        {
            return new List<AssignedOrderDto>();
        }

        return await _dbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.Category)
            .Include(o => o.FurnitureTypes)
            .Where(o => o.AssignedProductionManagerId == employee.Id)  // ✅ Filter by current user's employee ID
            .OrderBy(o => o.DeadlineDate)                              // ✅ Ordered by deadline (earliest first)
            .Select(o => new AssignedOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.Customer.FullName,
                CategoryName = o.Category.Name,
                Status = o.Status,
                ProgressPercentage = o.ProgressPercentage,
                DeadlineDate = o.DeadlineDate,
                FurnitureTypesCount = o.FurnitureTypes.Count,         // ✅ Count of furniture types
                CreatedAt = o.CreatedAt,
                Notes = o.Notes
            })
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new ProductionManager user account
    /// Creates both User (for authentication) and Employee (for assignment) records
    /// CRITICAL: Must create User FIRST, then Employee linked via UserId
    /// </summary>
    public async Task<ProductionManagerDto> CreateProductionManagerAsync(CreateProductionManagerDto request)
    {
        // ✅ Step 1: Validate email doesn't exist
        var existingUsers = await _dbContext.Users
            .Where(u => u.Email == request.Email)
            .ToListAsync();

        if (existingUsers.Any())
        {
            throw new ValidationException($"Email {request.Email} is already registered.");
        }

        // ✅ Step 2: Hash password using BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // ✅ Step 3: Create User record (for authentication/JWT)
        var user = new FurniFlowUz.Domain.Entities.User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = UserRole.ProductionManager,  // ⚠️ CRITICAL: Role must be ProductionManager
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();  // ⚠️ MUST save here to get User.Id

        // ✅ Step 4: Find or create "ProductionManager" position
        var position = await _dbContext.Positions
            .FirstOrDefaultAsync(p => p.Name == "Production Manager");

        if (position == null)
        {
            // Create position if it doesn't exist
            position = new Position
            {
                Name = "Production Manager",
                Description = "Manages production operations and teams",
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Positions.Add(position);
            await _dbContext.SaveChangesAsync();
        }

        // ✅ Step 5: Find or create "Production" department
        var department = await _dbContext.Departments
            .FirstOrDefaultAsync(d => d.Name == "Production");

        if (department == null)
        {
            // Create department if it doesn't exist
            department = new Department
            {
                Name = "Production",
                Description = "Production department",
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Departments.Add(department);
            await _dbContext.SaveChangesAsync();
        }

        // ✅ Step 6: Create Employee record (linked to User via UserId)
        var employee = new Employee
        {
            UserId = user.Id,  // ⚠️ CRITICAL: Links to User record
            FullName = $"{request.FirstName} {request.LastName}",
            Phone = request.Phone,
            PositionId = position.Id,
            DepartmentId = department.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();

        // ✅ Step 7: Return DTO with both IDs
        return new ProductionManagerDto
        {
            UserId = user.Id,
            EmployeeId = employee.Id,
            Email = user.Email,
            FullName = employee.FullName,
            Phone = employee.Phone,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
