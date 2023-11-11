using Core.Application.DTOs.Teacher;
using Core.Application.Transform;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .MinimumLength(6).WithMessage(ValidatorTransform.MinimumLength("UserName", 6));

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage(ValidatorTransform.Required("Password"))
                .MinimumLength(6).WithMessage(ValidatorTransform.MinimumLength("Password", 6))
                .Must(password => password.Any(char.IsLower))
                .WithMessage(ValidatorTransform.AnyIsLower("Password"))
                .Must(password => password.Any(char.IsUpper))
                .WithMessage(ValidatorTransform.AnyIsUpper("Password"))
                .Must(password => password.Any(char.IsDigit))
                .WithMessage(ValidatorTransform.AnyIsDigit("Password"))
                .Must(password => password.Any(ch => !char.IsLetterOrDigit(ch)))
                .WithMessage(ValidatorTransform.AnyIsLetterOrDigit("Password"));
        }
    }
}
