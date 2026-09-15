using CrmCep.Domain.Common;
using CrmCep.Domain.Enums;

namespace CrmCep.Domain.Entities;

/// <summary>
/// Represents a microfinance customer entity in CEP CRM.
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>
    /// Unique customer code format (e.g., KH-2026-0001).
    /// </summary>
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// Full legal name of the customer.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Contact email address (optional, validated against RFC standards).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Primary mobile phone number (required, 10-digit VN format).
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date of birth (must be at least 18 years of age).
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Microfinance segment (Worker, MicroMerchant, Freelancer).
    /// </summary>
    public CustomerSegment Segment { get; set; } = CustomerSegment.Worker;

    /// <summary>
    /// Current operational status (Active, Suspended, Locked).
    /// </summary>
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;

    /// <summary>
    /// Residential or workplace address in CEP service zone.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Internal credit officer notes or interaction details.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Soft delete flag to preserve financial audit trail.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Timestamp when soft deletion occurred.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
