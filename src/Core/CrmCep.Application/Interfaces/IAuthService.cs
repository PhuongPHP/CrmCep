using CrmCep.Application.Common;
using CrmCep.Application.DTOs;

namespace CrmCep.Application.Interfaces;

/// <summary>
/// Business service interface for authentication operations.
/// </summary>
public interface IAuthService
{
    Task<ApiResponse<TokenDto>> LoginAsync(LoginDto request);
}
