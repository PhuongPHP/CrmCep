using CrmCep.Application.Common;
using CrmCep.Application.DTOs;
using CrmCep.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmCep.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginDto> _validator;

    public AuthController(IAuthService authService, IValidator<LoginDto> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    /// <summary>
    /// Authenticates administrative credentials and returns a signed JWT access token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<TokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TokenDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<TokenDto>.Fail("Dữ liệu đăng nhập không hợp lệ.", errors));
        }

        var result = await _authService.LoginAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
