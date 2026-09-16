using CrmCep.Domain.Enums;

namespace CrmCep.Application.DTOs;

/// <summary>
/// Payload data transfer object for updating customer details.
/// CustomerCode is immutable to maintain financial identity.
/// </summary>
public class UpdateCustomerDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public CustomerSegment Segment { get; set; } = CustomerSegment.Worker;
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    public string? Address { get; set; }
    public string? Notes { get; set; }
}
