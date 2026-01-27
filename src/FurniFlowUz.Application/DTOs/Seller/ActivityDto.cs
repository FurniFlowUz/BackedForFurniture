namespace FurniFlowUz.Application.DTOs.Seller;

/// <summary>
/// Activity item for seller dashboard
/// </summary>
public class ActivityDto
{
    /// <summary>
    /// Activity unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Activity type (ContractCreated, OrderUpdated, PaymentReceived, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Activity title/summary
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Activity description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Related entity type (Contract, Order, Customer, etc.)
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Related entity ID
    /// </summary>
    public int? RelatedEntityId { get; set; }

    /// <summary>
    /// Activity timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who performed the activity
    /// </summary>
    public string? PerformedBy { get; set; }

    /// <summary>
    /// Activity status icon or color indicator
    /// </summary>
    public string? StatusIndicator { get; set; }
}
