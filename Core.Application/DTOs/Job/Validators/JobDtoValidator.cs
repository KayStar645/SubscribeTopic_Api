using Core.Application.Contracts.Persistence;
using Core.Application.Services;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Job.Validators
{
    public class JobDtoValidator : AbstractValidator<IJobDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public JobDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190));

            RuleFor(x => x.Instructions)
                .MaximumLength(5000).WithMessage(ValidatorTransform.MaximumLength("instructions", 5000));

            RuleFor(x => x.Due)
                .Must(due => CustomValidator.IsEqualOrAfterDay(due, DateTime.Now))
                .WithMessage(ValidatorTransform.GreaterEqualOrThanDay("due", DateTime.Now));

            RuleFor(x => x.Files)
                .Must(files =>
                {
                    if (files == null)
                        return true;
                    foreach (var file in files)
                    {
                        if (!string.IsNullOrEmpty(file) && !Uri.TryCreate(file, UriKind.Absolute, out _))
                            return false;
                    }

                    return true;
                })
                .WithMessage(ValidatorTransform.MustUrls("files"));
        }
    }
}
