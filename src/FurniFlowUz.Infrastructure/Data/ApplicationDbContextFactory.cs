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
        // Build configuration from appsettings
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FurniFlowUz.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("DatabaseConnection"),
            b => b.MigrationsAssembly("FurniFlowUz.Infrastructure"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
