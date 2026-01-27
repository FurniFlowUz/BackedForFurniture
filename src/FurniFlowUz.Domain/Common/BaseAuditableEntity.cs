namespace FurniFlowUz.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
