using System.ComponentModel.DataAnnotations;

namespace FurniFlowUz.Application.DTOs.Common;

/// <summary>
/// Base filter class for pagination, searching, and sorting
/// </summary>
public class BaseFilter
{
    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Search term for filtering results
    /// </summary>
    [MaxLength(200)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Property name to sort by
    /// </summary>
    [MaxLength(50)]
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort in descending order if true, ascending if false
    /// </summary>
    public bool SortDescending { get; set; } = false;
}
