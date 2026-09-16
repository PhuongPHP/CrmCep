namespace CrmCep.Application.DTOs;

/// <summary>
/// Response payload containing JWT token and authenticated user profile.
/// </summary>
public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int ExpiresInHours { get; set; }
}
