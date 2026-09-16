namespace CrmCep.Application.DTOs;

/// <summary>
/// Credentials payload for administrative user authentication.
/// </summary>
public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
