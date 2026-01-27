using FurniFlowUz.Application.DTOs.Notification;
using FurniFlowUz.Application.Interfaces;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Repositories;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace FurniFlowUz.Application.BackgroundJobs;

/// <summary>
/// Background job for sending deadline notifications
/// Checks for orders and contracts with deadlines approaching in 3 days
/// </summary>
public class DeadlineNotificationJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeadlineNotificationJob> _logger;

    public DeadlineNotificationJob(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<DeadlineNotificationJob> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the deadline notification job
    /// Finds orders and contracts with deadlines 3 days away and sends notifications
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("Starting deadline notification job at {Time}", DateTime.UtcNow);

            var today = DateTime.UtcNow.Date;
            var targetDate = today.AddDays(3);
            var targetDateEnd = targetDate.AddDays(1);

            // Find orders with deadlines 3 days away
            var upcomingOrders = await _unitOfWork.Orders.FindAsync(
                o => o.DeadlineDate >= targetDate &&
                     o.DeadlineDate < targetDateEnd &&
                     o.Status != OrderStatus.Completed &&
                     o.Status != OrderStatus.Cancelled
            );

            var ordersList = upcomingOrders.ToList();
            _logger.LogInformation("Found {Count} orders with deadlines approaching in 3 days", ordersList.Count);

            // Send notifications for each order
            foreach (var order in ordersList)
            {
                try
                {
                    // Notify Director
                    await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                    {
                        Type = "Warning",
                        Title = "Order Deadline Approaching",
                        Message = $"Order {order.OrderNumber} deadline is in 3 days (Due: {order.DeadlineDate:yyyy-MM-dd}). Current progress: {order.ProgressPercentage}%",
                        Role = UserRole.Director.ToString(),
                        RelatedEntityType = "Order",
                        RelatedEntityId = order.Id
                    });

                    // Notify ProductionManager if assigned
                    if (order.AssignedProductionManagerId.HasValue)
                    {
                        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                        {
                            Type = "Warning",
                            Title = "Order Deadline Approaching",
                            Message = $"Order {order.OrderNumber} deadline is in 3 days (Due: {order.DeadlineDate:yyyy-MM-dd}). Current progress: {order.ProgressPercentage}%",
                            Role = UserRole.ProductionManager.ToString(),
                            RelatedEntityType = "Order",
                            RelatedEntityId = order.Id
                        });
                    }

                    _logger.LogInformation("Sent deadline notification for order {OrderNumber}", order.OrderNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending notification for order {OrderNumber}", order.OrderNumber);
                }
            }

            // Find contracts with production completion dates 3 days away
            var allContracts = await _unitOfWork.Contracts.GetAllAsync();
            var upcomingContracts = allContracts.Where(c =>
                c.SignedDate.HasValue &&
                c.SignedDate.Value.AddDays(c.ProductionDurationDays) >= targetDate &&
                c.SignedDate.Value.AddDays(c.ProductionDurationDays) < targetDateEnd &&
                c.Status != Domain.Enums.ContractStatus.Completed &&
                c.Status != Domain.Enums.ContractStatus.Cancelled
            );

            var contractsList = upcomingContracts.ToList();
            _logger.LogInformation("Found {Count} contracts with deadlines approaching in 3 days", contractsList.Count);

            // Send notifications for each contract
            foreach (var contract in contractsList)
            {
                try
                {
                    var dueDate = contract.SignedDate?.AddDays(contract.ProductionDurationDays);
                    var dueDateStr = dueDate?.ToString("yyyy-MM-dd") ?? "TBD";

                    // Notify Director
                    await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                    {
                        Type = "Warning",
                        Title = "Contract Deadline Approaching",
                        Message = $"Contract {contract.ContractNumber} production completion is in 3 days (Due: {dueDateStr}). Total amount: {contract.TotalAmount:C}",
                        Role = UserRole.Director.ToString(),
                        RelatedEntityType = "Contract",
                        RelatedEntityId = contract.Id
                    });

                    // Notify Salesperson
                    await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                    {
                        Type = "Warning",
                        Title = "Contract Deadline Approaching",
                        Message = $"Contract {contract.ContractNumber} production completion is in 3 days (Due: {dueDateStr}). Total amount: {contract.TotalAmount:C}",
                        Role = UserRole.Salesperson.ToString(),
                        RelatedEntityType = "Contract",
                        RelatedEntityId = contract.Id
                    });

                    _logger.LogInformation("Sent deadline notification for contract {ContractNumber}", contract.ContractNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending notification for contract {ContractNumber}", contract.ContractNumber);
                }
            }

            _logger.LogInformation(
                "Deadline notification job completed successfully. Processed {OrderCount} orders and {ContractCount} contracts",
                ordersList.Count,
                contractsList.Count
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing deadline notification job");
            throw;
        }
    }
}
