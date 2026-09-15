using CrmCep.Domain.Enums;

namespace CrmCep.Application.DTOs;

/// <summary>
/// Data transfer object representing customer details for display and grid binding.
/// </summary>
public class CustomerDto
{
    public Guid Id { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public CustomerSegment Segment { get; set; }
    public CustomerStatus Status { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
