using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FurniFlowUz.Infrastructure.Data;

/// <summary>
/// Database initializer for seeding initial data
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Initialize the database with migrations and seed data
    /// </summary>
    /// <param name="context">Application database context</param>
    public static async Task Initialize(ApplicationDbContext context)
    {
        // 1. Ensure database is created and migrations are applied
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception)
        {
            // Migration may fail if already applied - continue with seeding
        }

        // 2. Seed Production Stages
        await SeedProductionStages(context);

        // 3. Seed Categories
        await SeedCategories(context);

        // 4. Seed Admin and Demo Users
        await SeedUsers(context);

        // 5. Seed Demo Customers
        await SeedCustomers(context);

        // 6. Seed Demo Warehouse Items
        await SeedWarehouseItems(context);
    }

    private static async Task SeedProductionStages(ApplicationDbContext context)
    {
        // Check if production stages already exist
        if (await context.ProductionStages.AnyAsync())
        {
            return;
        }

        var stages = new[]
        {
            new ProductionStage
            {
                Name = "Arra",
                StageType = ProductionStageType.Sawing,
                SequenceOrder = 1,
                EstimatedDurationHours = 8,
                Description = "Sawing and cutting materials to required dimensions",
                IsActive = true
            },
            new ProductionStage
            {
                Name = "Rover",
                StageType = ProductionStageType.Routing,
                SequenceOrder = 2,
                EstimatedDurationHours = 12,
                Description = "Routing and shaping furniture parts",
                IsActive = true
            },
            new ProductionStage
            {
                Name = "Krom",
                StageType = ProductionStageType.EdgeBanding,
                SequenceOrder = 3,
                EstimatedDurationHours = 6,
                Description = "Edge banding application for clean finishes",
                IsActive = true
            },
            new ProductionStage
            {
                Name = "Shkurka",
                StageType = ProductionStageType.Sanding,
                SequenceOrder = 4,
                EstimatedDurationHours = 4,
                Description = "Sanding and surface preparation",
                IsActive = true
            }
        };

        await context.ProductionStages.AddRangeAsync(stages);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCategories(ApplicationDbContext context)
    {
        // Check if categories already exist
        if (await context.Categories.AnyAsync())
        {
            return;
        }

        var categories = new[]
        {
            new Category
            {
                Name = "Oshxona mebeli",
                Description = "Kitchen furniture including cabinets, countertops, and storage solutions",
                IsActive = true
            },
            new Category
            {
                Name = "Shkaf-kupe",
                Description = "Sliding wardrobe systems and built-in closets",
                IsActive = true
            },
            new Category
            {
                Name = "Yotoqxona mebeli",
                Description = "Bedroom furniture including beds, wardrobes, and nightstands",
                IsActive = true
            },
            new Category
            {
                Name = "Ofis mebellari",
                Description = "Office furniture including desks, chairs, and storage units",
                IsActive = true
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsers(ApplicationDbContext context)
    {
        // Check if users already exist
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var users = new[]
        {
            // Admin User
            new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@furniflowauz.com",
                PhoneNumber = "+998901234567",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Director,
                IsActive = true
            },
            // Demo Salesperson
            new User
            {
                FirstName = "Sales",
                LastName = "Person",
                Email = "sales@furniflowauz.com",
                PhoneNumber = "+998901234568",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Sales123!"),
                Role = UserRole.Salesperson,
                IsActive = true
            },
            // Demo Constructor
            new User
            {
                FirstName = "Constructor",
                LastName = "User",
                Email = "constructor@furniflowauz.com",
                PhoneNumber = "+998901234569",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Const123!"),
                Role = UserRole.Constructor,
                IsActive = true
            },
            // Demo Production Manager
            new User
            {
                FirstName = "Production",
                LastName = "Manager",
                Email = "production@furniflowauz.com",
                PhoneNumber = "+998901234570",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Prod123!"),
                Role = UserRole.ProductionManager,
                IsActive = true
            },
            // Demo Warehouse Manager
            new User
            {
                FirstName = "Warehouse",
                LastName = "Manager",
                Email = "warehouse@furniflowauz.com",
                PhoneNumber = "+998901234571",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ware123!"),
                Role = UserRole.WarehouseManager,
                IsActive = true
            },
            // Demo Team Leader
            new User
            {
                FirstName = "Team",
                LastName = "Leader",
                Email = "teamlead@furniflowauz.com",
                PhoneNumber = "+998901234572",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Team123!"),
                Role = UserRole.TeamLeader,
                IsActive = true
            },
            // Demo Worker
            new User
            {
                FirstName = "Worker",
                LastName = "User",
                Email = "worker@furniflowauz.com",
                PhoneNumber = "+998901234573",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Work123!"),
                Role = UserRole.Worker,
                IsActive = true
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCustomers(ApplicationDbContext context)
    {
        // Check if customers already exist
        if (await context.Customers.AnyAsync())
        {
            return;
        }

        var customers = new[]
        {
            new Customer
            {
                FullName = "Alisher Navoiy",
                PhoneNumber = "+998901111111",
                Email = "alisher.navoiy@example.com",
                Address = "Tashkent, Mirzo Ulugbek District, Navoiy Street 45",
                Notes = "Preferred customer, interested in modern kitchen designs"
            },
            new Customer
            {
                FullName = "Nodira Begim",
                PhoneNumber = "+998902222222",
                Email = "nodira.begim@example.com",
                Address = "Tashkent, Chilanzar District, Bunyodkor Avenue 12",
                Notes = "Looking for complete bedroom furniture set"
            },
            new Customer
            {
                FullName = "Mirzo Ulugbek",
                PhoneNumber = "+998903333333",
                Email = "mirzo.ulugbek@example.com",
                Address = "Tashkent, Yunusabad District, Amir Temur Street 78",
                Notes = "Corporate client, needs office furniture for new office space"
            }
        };

        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();
    }

    private static async Task SeedWarehouseItems(ApplicationDbContext context)
    {
        // Check if warehouse items already exist
        if (await context.WarehouseItems.AnyAsync())
        {
            return;
        }

        var warehouseItems = new[]
        {
            new WarehouseItem
            {
                Name = "Chipboard - White",
                SKU = "MAT-001",
                CurrentStock = 100,
                MinimumStock = 50,
                Unit = UnitOfMeasurement.SquareMeter,
                UnitPrice = 50000,
                Description = "Standard white chipboard for furniture manufacturing",
                IsActive = true
            },
            new WarehouseItem
            {
                Name = "Edge Tape - PVC White",
                SKU = "MAT-002",
                CurrentStock = 500,
                MinimumStock = 100,
                Unit = UnitOfMeasurement.Meter,
                UnitPrice = 2500,
                Description = "PVC edge tape for clean furniture edges",
                IsActive = true
            },
            new WarehouseItem
            {
                Name = "Hinges - Standard",
                SKU = "MAT-003",
                CurrentStock = 1000,
                MinimumStock = 200,
                Unit = UnitOfMeasurement.Piece,
                UnitPrice = 5000,
                Description = "Standard door hinges for cabinets and wardrobes",
                IsActive = true
            },
            new WarehouseItem
            {
                Name = "Handles - Chrome Finish",
                SKU = "MAT-004",
                CurrentStock = 500,
                MinimumStock = 100,
                Unit = UnitOfMeasurement.Piece,
                UnitPrice = 8000,
                Description = "Chrome finish handles for doors and drawers",
                IsActive = true
            },
            new WarehouseItem
            {
                Name = "Wood Glue - Industrial",
                SKU = "MAT-005",
                CurrentStock = 50,
                MinimumStock = 20,
                Unit = UnitOfMeasurement.Liter,
                UnitPrice = 35000,
                Description = "High-strength industrial wood glue",
                IsActive = true
            },
            new WarehouseItem
            {
                Name = "Drawer Slides - Full Extension",
                SKU = "MAT-006",
                CurrentStock = 300,
                MinimumStock = 50,
                Unit = UnitOfMeasurement.Piece,
                UnitPrice = 15000,
                Description = "Full extension drawer slides for smooth operation",
                IsActive = true
            }
        };

        await context.WarehouseItems.AddRangeAsync(warehouseItems);
        await context.SaveChangesAsync();
    }
}
