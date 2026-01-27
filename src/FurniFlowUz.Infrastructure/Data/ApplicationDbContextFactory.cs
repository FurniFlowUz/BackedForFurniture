using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FurniFlowUz.Infrastructure.Data;

/// <summary>
/// Design-time factory for ApplicationDbContext (used by EF Core migrations)
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use default connection string for migrations
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=FurniFlowUzDb;Trusted_Connection=True;MultipleActiveResultSets=true",
            b => b.MigrationsAssembly("FurniFlowUz.Infrastructure"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
