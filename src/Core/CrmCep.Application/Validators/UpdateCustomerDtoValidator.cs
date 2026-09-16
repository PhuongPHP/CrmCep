using CrmCep.Application.DTOs;
using FluentValidation;

namespace CrmCep.Application.Validators;

/// <summary>
/// Fluent validation rules for customer modification.
/// </summary>
public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên không được để trống.")
            .MinimumLength(2).WithMessage("Họ và tên phải có ít nhất 2 ký tự.")
            .MaximumLength(100).WithMessage("Họ và tên không được vượt quá 100 ký tự.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .Matches(@"^(03|05|07|08|09)\d{8}$")
            .WithMessage("Số điện thoại không hợp lệ (phải là số di động 10 chữ số tại Việt Nam).");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Định dạng email không hợp lệ.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Ngày sinh không được để trống.")
            .Must(BeAtLeast18YearsOld)
            .WithMessage("Khách hàng phải từ đủ 18 tuổi trở lên để thực hiện giao dịch tài chính vi mô.")
            .Must(BeUnder100YearsOld)
            .WithMessage("Ngày sinh không hợp lệ (tuổi không được vượt quá 100).");
    }

    private static bool BeAtLeast18YearsOld(DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age)) age--;
        return age >= 18;
    }

    private static bool BeUnder100YearsOld(DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age)) age--;
        return age <= 100;
    }
}
