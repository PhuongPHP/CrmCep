using CrmCep.Domain.Entities;

namespace CrmCep.Application.Interfaces;

/// <summary>
/// Service interface for generating signed JSON Web Tokens.
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
