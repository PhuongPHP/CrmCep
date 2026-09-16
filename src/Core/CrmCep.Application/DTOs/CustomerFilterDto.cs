using CrmCep.Domain.Enums;

namespace CrmCep.Application.DTOs;

/// <summary>
/// Query filter parameters for searching, filtering, and paging customers.
/// </summary>
public class CustomerFilterDto
{
    /// <summary>
    /// Search term matched against FullName or PhoneNumber.
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// Filter by operational status.
    /// </summary>
    public CustomerStatus? Status { get; set; }

    /// <summary>
    /// Filter by customer segment.
    /// </summary>
    public CustomerSegment? Segment { get; set; }

    /// <summary>
    /// 1-based page index.
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;
}
