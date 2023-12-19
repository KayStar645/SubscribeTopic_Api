using Core.Application.Models.Identity.Auths;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.Models.Identity.Validators
{
    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        /*
         .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$")
         .WithMessage("Password phải chứa ít nhất 1 ký tự đặc biệt, 1 số, 1 chữ thường và 1 chữ hoa.");
         */
        public RegistrationRequestValidator() 
        {
            RuleFor(p => p.UserName)
                .NotEmpty().WithMessage(ValidatorTransform.Required("UserName"))
                .MinimumLength(3).WithMessage(ValidatorTransform.MinimumLength("UserName", 3));

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage(ValidatorTransform.Required("Password"))
                .MinimumLength(3).WithMessage(ValidatorTransform.MinimumLength("Password", 3));
                //.Must(password => password.Any(char.IsLower))
                //.WithMessage(ValidatorTransform.AnyIsLower("Password"))
                //.Must(password => password.Any(char.IsUpper))
                //.WithMessage(ValidatorTransform.AnyIsUpper("Password"))
                //.Must(password => password.Any(char.IsDigit))
                //.WithMessage(ValidatorTransform.AnyIsDigit("Password"))
                //.Must(password => password.Any(ch => !char.IsLetterOrDigit(ch)))
                //.WithMessage(ValidatorTransform.AnyIsLetterOrDigit("Password"));
        }
    }
}
