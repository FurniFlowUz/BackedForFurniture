using FurniFlowUz.Domain.Entities;
using FurniFlowUz.Domain.Enums;
using FurniFlowUz.Infrastructure.Repositories;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace FurniFlowUz.Application.BackgroundJobs;

/// <summary>
/// Background job for calculating daily KPIs
/// Calculates and stores key performance indicators for business analytics
/// </summary>
public class KPICalculationJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KPICalculationJob> _logger;

    public KPICalculationJob(
        IUnitOfWork unitOfWork,
        ILogger<KPICalculationJob> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Executes the KPI calculation job
    /// Calculates daily KPIs including orders completed, revenue, team productivity, etc.
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("Starting KPI calculation job at {Time}", DateTime.UtcNow);

            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var yesterday = today.AddDays(-1);

            // Check if KPI for today already exists
            var existingKpi = await _unitOfWork.KPIs.FindAsync(k => k.Date == today);
            if (existingKpi.Any())
            {
                _logger.LogWarning("KPI for date {Date} already exists. Skipping calculation.", today);
                return;
            }

            // 1. Calculate Orders Completed - orders completed on this day
            var completedOrders = await _unitOfWork.Orders.FindAsync(
                o => o.Status == OrderStatus.Completed &&
                     o.CompletedAt.HasValue &&
                     o.CompletedAt.Value >= yesterday &&
                     o.CompletedAt.Value < today
            );

            var completedOrdersList = completedOrders.ToList();
            var ordersCompleted = completedOrdersList.Count;

            _logger.LogInformation("Found {Count} completed orders for yesterday", ordersCompleted);

            // 2. Calculate Revenue - from contracts with completed orders
            decimal totalRevenue = 0;
            if (completedOrdersList.Any())
            {
                var contractIds = completedOrdersList
                    .Where(o => o.ContractId.HasValue)
                    .Select(o => o.ContractId!.Value)
                    .Distinct()
                    .ToList();

                if (contractIds.Any())
                {
                    var completedContracts = await _unitOfWork.Contracts.FindAsync(
                        c => contractIds.Contains(c.Id) &&
                             c.PaymentStatus == PaymentStatus.FullyPaid
                    );

                    totalRevenue = completedContracts.Sum(c => c.TotalAmount);
                }
            }

            _logger.LogInformation("Calculated total revenue: {Revenue}", totalRevenue);

            // 3. Calculate Average Completion Time
            decimal? avgCompletionTime = null;
            if (completedOrdersList.Any())
            {
                var completionTimes = completedOrdersList
                    .Where(o => o.CompletedAt.HasValue)
                    .Select(o => (o.CompletedAt!.Value - o.CreatedAt).TotalDays)
                    .ToList();

                if (completionTimes.Any())
                {
                    avgCompletionTime = (decimal)completionTimes.Average();
                }
            }

            _logger.LogInformation("Average completion time: {Time} days", avgCompletionTime);

            // 4. Calculate Team Productivity (completed tasks / assigned tasks for yesterday)
            var allTasks = await _unitOfWork.WorkTasks.FindAsync(
                t => t.CreatedAt >= yesterday && t.CreatedAt < today ||
                     (t.CompletedAt.HasValue && t.CompletedAt.Value >= yesterday && t.CompletedAt.Value < today)
            );

            var tasksList = allTasks.ToList();
            var completedTasks = tasksList.Count(t => t.Status == Domain.Enums.TaskStatus.Completed);
            var assignedTasks = tasksList.Count;

            decimal? teamProductivity = null;
            if (assignedTasks > 0)
            {
                teamProductivity = Math.Round((decimal)completedTasks / assignedTasks * 100, 2);
            }

            _logger.LogInformation("Team productivity: {Productivity}% ({Completed}/{Assigned} tasks)",
                teamProductivity, completedTasks, assignedTasks);

            // 5. Calculate Material Utilization Rate
            // Based on warehouse transactions - materials used vs available
            var warehouseTransactions = await _unitOfWork.WarehouseTransactions.FindAsync(
                t => t.TransactionDate >= yesterday && t.TransactionDate < today
            );

            var transactionsList = warehouseTransactions.ToList();
            var totalMaterialsUsed = transactionsList
                .Where(t => t.Type == Domain.Enums.WarehouseTransactionType.Outcome)
                .Sum(t => t.Quantity);

            var allWarehouseItems = await _unitOfWork.WarehouseItems.GetAllAsync();
            var totalMaterialsAvailable = allWarehouseItems.Sum(i => i.CurrentStock);

            decimal? materialUtilizationRate = null;
            if (totalMaterialsAvailable > 0)
            {
                materialUtilizationRate = Math.Round((totalMaterialsUsed / totalMaterialsAvailable) * 100, 2);
            }

            _logger.LogInformation("Material utilization rate: {Rate}%", materialUtilizationRate);

            // 6. Calculate Customer Satisfaction Rate
            // Based on completed orders without delays
            decimal? customerSatisfactionRate = null;
            if (completedOrdersList.Any())
            {
                var onTimeOrders = completedOrdersList.Count(o =>
                    o.CompletedAt.HasValue && o.CompletedAt.Value <= o.DeadlineDate
                );

                customerSatisfactionRate = Math.Round((decimal)onTimeOrders / completedOrdersList.Count * 100, 2);
            }

            _logger.LogInformation("Customer satisfaction rate: {Rate}%", customerSatisfactionRate);

            // 7. Get Active Orders count
            var activeOrders = await _unitOfWork.Orders.FindAsync(
                o => o.Status == OrderStatus.InProduction ||
                     o.Status == OrderStatus.SpecificationsReady
            );

            var activeOrdersCount = activeOrders.Count();

            // 8. Get Delayed Orders count (orders past deadline and not completed)
            var delayedOrders = await _unitOfWork.Orders.FindAsync(
                o => o.DeadlineDate < today &&
                     o.Status != OrderStatus.Completed &&
                     o.Status != OrderStatus.Cancelled
            );

            var delayedOrdersCount = delayedOrders.Count();

            // 9. Get New Customers count
            var newCustomers = await _unitOfWork.Customers.FindAsync(
                c => c.CreatedAt >= yesterday && c.CreatedAt < today
            );

            var newCustomersCount = newCustomers.Count();

            // Create KPI entity
            var kpi = new KPI
            {
                Date = yesterday, // Store for yesterday's data
                OrdersCompleted = ordersCompleted,
                Revenue = totalRevenue,
                AverageCompletionTime = avgCompletionTime,
                TeamProductivity = teamProductivity,
                MaterialUtilizationRate = materialUtilizationRate,
                CustomerSatisfactionRate = customerSatisfactionRate,
                ActiveOrders = activeOrdersCount,
                DelayedOrders = delayedOrdersCount,
                NewCustomers = newCustomersCount,
                Notes = $"Auto-generated KPI for {yesterday:yyyy-MM-dd}"
            };

            // Save KPI to database
            await _unitOfWork.KPIs.AddAsync(kpi);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "KPI calculation job completed successfully. KPI saved for date {Date}. " +
                "Orders: {Orders}, Revenue: {Revenue}, Team Productivity: {Productivity}%, " +
                "Material Utilization: {MaterialRate}%, Customer Satisfaction: {SatisfactionRate}%",
                yesterday,
                ordersCompleted,
                totalRevenue,
                teamProductivity,
                materialUtilizationRate,
                customerSatisfactionRate
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing KPI calculation job");
            throw;
        }
    }
}
