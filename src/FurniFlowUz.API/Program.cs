using FurniFlowUz.API.Extensions;
using FurniFlowUz.API.Hubs;
using FurniFlowUz.API.Middleware;
using FurniFlowUz.Application.BackgroundJobs;
using Hangfire;
using Hangfire.Dashboard;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/furniflowuz-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddHttpContextAccessor();

// Add custom services via extension methods
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddSignalRServices();

var app = builder.Build();

// Seed the database with initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FurniFlowUz.Infrastructure.Data.ApplicationDbContext>();
        await FurniFlowUz.Infrastructure.Data.DbInitializer.Initialize(context);
        Log.Information("Database seeded successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding the database");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FurniFlowUz API v1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FurniFlowUz API v1");
    });
}

// Use HTTPS redirection
app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

// Serve static files (for uploaded files)
app.UseStaticFiles();

// Use authentication and authorization BEFORE exception middleware
app.UseAuthentication();
app.UseAuthorization();

// Use custom exception middleware (after auth to catch auth exceptions)
app.UseExceptionMiddleware();

// Use audit middleware (after authentication and exception handling)
app.UseAuditMiddleware();

// Configure Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Schedule recurring background jobs
RecurringJob.AddOrUpdate<DeadlineNotificationJob>(
    "deadline-notifications",
    job => job.ExecuteAsync(),
    Cron.Hourly); // Run every hour

RecurringJob.AddOrUpdate<WarehouseAlertJob>(
    "warehouse-alerts",
    job => job.ExecuteAsync(),
    Cron.Daily); // Run daily

RecurringJob.AddOrUpdate<KPICalculationJob>(
    "kpi-calculation",
    job => job.ExecuteAsync(),
    Cron.Daily); // Run daily

// Map controllers
app.MapControllers();

// Map SignalR hub
app.MapHub<NotificationHub>("/hubs/notifications");

// Log startup
Log.Information("FurniFlowUz API starting up...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>
/// Custom Hangfire authorization filter for dashboard access
/// Only allows Director role to access the dashboard
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow access only to authenticated users with Director role
        return httpContext.User.Identity?.IsAuthenticated == true &&
               httpContext.User.IsInRole("Director");
    }
}
