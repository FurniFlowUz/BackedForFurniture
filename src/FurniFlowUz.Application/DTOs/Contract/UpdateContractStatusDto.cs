using System.ComponentModel.DataAnnotations;
using FurniFlowUz.Domain.Enums;

namespace FurniFlowUz.Application.DTOs.Contract;

/// <summary>
/// DTO for updating contract status
/// </summary>
public class UpdateContractStatusDto
{
    /// <summary>
    /// New contract status
    /// </summary>
    [Required(ErrorMessage = "Status is required")]
    public ContractStatus Status { get; set; }
}
