using CrmCep.Application.DTOs;
using FluentValidation;

namespace CrmCep.Application.Validators;

/// <summary>
/// Fluent validation rules for user authentication credentials.
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.");
    }
}
