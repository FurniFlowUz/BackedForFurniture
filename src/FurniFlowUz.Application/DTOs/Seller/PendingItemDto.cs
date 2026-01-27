namespace FurniFlowUz.Application.DTOs.Seller;

/// <summary>
/// Pending item requiring seller attention
/// </summary>
public class PendingItemDto
{
    /// <summary>
    /// Pending item unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Item type (PendingApproval, AwaitingPayment, RequiresFollowUp, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Item title/summary
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Item description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Priority level (Low, Medium, High, Urgent)
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// Related entity type (Contract, Order, Customer, etc.)
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Related entity ID
    /// </summary>
    public int? RelatedEntityId { get; set; }

    /// <summary>
    /// Due date for the pending item
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Date when item became pending
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Customer name if applicable
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Amount if applicable
    /// </summary>
    public decimal? Amount { get; set; }
}
