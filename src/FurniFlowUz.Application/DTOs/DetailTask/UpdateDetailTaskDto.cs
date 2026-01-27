using System.ComponentModel.DataAnnotations;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.DTOs.DetailTask;

/// <summary>
/// DTO for updating a detail task
/// </summary>
public class UpdateDetailTaskDto
{
    /// <summary>
    /// New task status
    /// </summary>
    [Required(ErrorMessage = "Status is required")]
    public DetailTaskStatus Status { get; set; }

    /// <summary>
    /// Optional notes for the update
    /// </summary>
    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
