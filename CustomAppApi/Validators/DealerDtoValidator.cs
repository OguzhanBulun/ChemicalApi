using FluentValidation;
using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Validators
{
    public class DealerDtoValidator : AbstractValidator<DealerDto>
    {
        public DealerDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Firma adı boş olamaz")
                .MaximumLength(100).WithMessage("Firma adı en fazla 100 karakter olabilir");

            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage("Vergi numarası boş olamaz")
                .Length(10).WithMessage("Vergi numarası 10 karakter olmalıdır")
                .Matches("^[0-9]*$").WithMessage("Vergi numarası sadece rakam içermelidir");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email adresi boş olamaz")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası boş olamaz")
                .Matches(@"^[0-9\-\+]{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Adres boş olamaz")
                .MaximumLength(200).WithMessage("Adres en fazla 200 karakter olabilir");
        }
    }
} 