using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.JobResults.Validators
{
    public class SubmitJobResultsDtoValidator : AbstractValidator<SubmitJobResultsDto>
    {
        public SubmitJobResultsDtoValidator() 
        {
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
