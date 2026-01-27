using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Enums;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace FurniFlowUz.Application.BackgroundJobs;

/// <summary>
/// Background job for warehouse low stock alerts
/// Checks for warehouse items below minimum stock levels
/// </summary>
public class WarehouseAlertJob
{
    private readonly IWarehouseService _warehouseService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<WarehouseAlertJob> _logger;

    public WarehouseAlertJob(
        IWarehouseService warehouseService,
        INotificationService notificationService,
        ILogger<WarehouseAlertJob> logger)
    {
        _warehouseService = warehouseService;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the warehouse alert job
    /// Finds items with low stock and sends notifications to Director and WarehouseManager
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("Starting warehouse alert job at {Time}", DateTime.UtcNow);

            // Get all low stock alerts from warehouse service
            var lowStockAlerts = await _warehouseService.GetLowStockAlertsAsync();
            var alertsList = lowStockAlerts.ToList();

            _logger.LogInformation("Found {Count} items with low stock levels", alertsList.Count);

            if (!alertsList.Any())
            {
                _logger.LogInformation("No low stock alerts to process");
                return;
            }

            // Group alerts by severity for better notification management
            var criticalAlerts = alertsList.Where(a => a.Severity == "Critical").ToList();
            var warningAlerts = alertsList.Where(a => a.Severity == "Warning").ToList();

            // Send notifications for critical alerts
            if (criticalAlerts.Any())
            {
                foreach (var alert in criticalAlerts)
                {
                    try
                    {
                        var message = $"CRITICAL: {alert.ItemName} is critically low on stock. " +
                                    $"Current: {alert.CurrentStock} {alert.Unit}, " +
                                    $"Minimum: {alert.MinimumStock} {alert.Unit}. " +
                                    $"Stock level: {alert.StockPercentage}% of minimum. " +
                                    $"Immediate restocking required!";

                        // Notify Director
                        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                        {
                            Type = "Error",
                            Title = "Critical Stock Alert",
                            Message = message,
                            Role = UserRole.Director.ToString(),
                            RelatedEntityType = "WarehouseItem",
                            RelatedEntityId = alert.ItemId
                        });

                        // Notify WarehouseManager
                        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                        {
                            Type = "Error",
                            Title = "Critical Stock Alert",
                            Message = message,
                            Role = UserRole.WarehouseManager.ToString(),
                            RelatedEntityType = "WarehouseItem",
                            RelatedEntityId = alert.ItemId
                        });

                        _logger.LogInformation("Sent critical stock alert for item {ItemName}", alert.ItemName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending critical alert for item {ItemName}", alert.ItemName);
                    }
                }
            }

            // Send notifications for warning alerts
            if (warningAlerts.Any())
            {
                foreach (var alert in warningAlerts)
                {
                    try
                    {
                        var message = $"Low stock warning for {alert.ItemName}. " +
                                    $"Current: {alert.CurrentStock} {alert.Unit}, " +
                                    $"Minimum: {alert.MinimumStock} {alert.Unit}. " +
                                    $"Stock level: {alert.StockPercentage}% of minimum. " +
                                    $"Please plan for restocking.";

                        // Notify Director
                        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                        {
                            Type = "Warning",
                            Title = "Low Stock Alert",
                            Message = message,
                            Role = UserRole.Director.ToString(),
                            RelatedEntityType = "WarehouseItem",
                            RelatedEntityId = alert.ItemId
                        });

                        // Notify WarehouseManager
                        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                        {
                            Type = "Warning",
                            Title = "Low Stock Alert",
                            Message = message,
                            Role = UserRole.WarehouseManager.ToString(),
                            RelatedEntityType = "WarehouseItem",
                            RelatedEntityId = alert.ItemId
                        });

                        _logger.LogInformation("Sent low stock warning for item {ItemName}", alert.ItemName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending warning alert for item {ItemName}", alert.ItemName);
                    }
                }
            }

            _logger.LogInformation(
                "Warehouse alert job completed successfully. Sent {CriticalCount} critical alerts and {WarningCount} warning alerts",
                criticalAlerts.Count,
                warningAlerts.Count
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing warehouse alert job");
            throw;
        }
    }
}
