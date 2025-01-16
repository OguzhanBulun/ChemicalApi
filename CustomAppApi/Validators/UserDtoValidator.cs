using FluentValidation;
using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır")
                .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email adresi boş olamaz")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz");

            RuleFor(x => x.UserType)
                .IsInEnum().WithMessage("Geçerli bir kullanıcı tipi seçiniz");
        }
    }
} 