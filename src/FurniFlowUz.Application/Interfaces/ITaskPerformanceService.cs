using FurniFlowUz.Application.DTOs.TaskPerformance;

namespace FurniFlowUz.Application.Interfaces;

/// <summary>
/// Service interface for task performance and KPI tracking
/// </summary>
public interface ITaskPerformanceService
{
    /// <summary>
    /// Gets performance record for a specific task
    /// </summary>
    Task<TaskPerformanceDto> GetByTaskIdAsync(int detailTaskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets KPI metrics for current employee
    /// </summary>
    Task<EmployeeKPIDto> GetMyKPIAsync(DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets KPI metrics for a specific employee
    /// </summary>
    Task<EmployeeKPIDto> GetEmployeeKPIAsync(int employeeId, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets KPI metrics for a team
    /// </summary>
    Task<TeamKPIDto> GetTeamKPIAsync(int teamId, DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets company-wide KPI metrics (Director only)
    /// </summary>
    Task<TeamKPIDto> GetCompanyKPIAsync(DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records performance for a completed task
    /// </summary>
    Task<TaskPerformanceDto> RecordPerformanceAsync(CreateTaskPerformanceDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates efficiency percentage based on estimated vs actual duration
    /// </summary>
    decimal CalculateEfficiency(TimeSpan estimatedDuration, TimeSpan actualDuration);
}
