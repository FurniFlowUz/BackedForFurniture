namespace FurniFlowUz.Domain.Enums;

public enum OrderStatus
{
    New,
    Assigned,              // Category assigned to Team Leader
    SpecificationsReady,
    InProduction,
    QualityCheck,          // Awaiting final quality control
    Completed,
    Delivered,             // Delivered to customer
    Cancelled
}
