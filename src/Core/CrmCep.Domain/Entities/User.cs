using CrmCep.Domain.Common;

namespace CrmCep.Domain.Entities;

/// <summary>
/// Represents an authenticated system user (e.g., Credit Officer, Branch Admin).
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Unique username for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Cryptographically secure password hash (BCrypt).
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Full display name of the staff member.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Corporate email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Assigned role for authorization (Admin, Staff).
    /// </summary>
    public string Role { get; set; } = "Admin";

    /// <summary>
    /// Account active flag.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
