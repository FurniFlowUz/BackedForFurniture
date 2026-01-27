using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FurniFlowUz.Domain.Common;

namespace FurniFlowUz.Domain.Entities;

public class KPI : BaseEntity
{
    [Required]
    public DateTime Date { get; set; }

    public int OrdersCompleted { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Revenue { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? AverageCompletionTime { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? TeamProductivity { get; set; }

    public int ActiveOrders { get; set; }

    public int DelayedOrders { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? MaterialUtilizationRate { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? CustomerSatisfactionRate { get; set; }

    public int NewCustomers { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
