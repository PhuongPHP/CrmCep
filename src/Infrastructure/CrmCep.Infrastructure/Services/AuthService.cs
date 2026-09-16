using CrmCep.Application.Common;
using CrmCep.Application.DTOs;
using CrmCep.Application.Interfaces;
using CrmCep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CrmCep.Infrastructure.Services;

/// <summary>
/// Handles authentication credentials verification and JWT token issuance.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(ApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ApiResponse<TokenDto>> LoginAsync(LoginDto request)
    {
        // Find active user by username
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.Trim().ToLower() && u.IsActive);

        if (user == null)
        {
            return ApiResponse<TokenDto>.Fail("Tên đăng nhập hoặc mật khẩu không chính xác.");
        }

        // Verify cryptographic hash
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return ApiResponse<TokenDto>.Fail("Tên đăng nhập hoặc mật khẩu không chính xác.");
        }

        // Generate signed JWT Bearer token
        var token = _jwtTokenGenerator.GenerateToken(user);

        var result = new TokenDto
        {
            AccessToken = token,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            ExpiresInHours = 8
        };

        return ApiResponse<TokenDto>.Ok(result, "Đăng nhập thành công.");
    }
}
