using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using JobEntity = Core.Domain.Entities.Job;

namespace Core.Application.DTOs.JobResults.Validators
{
    public class SubmitJobResultsDtoValidator : AbstractValidator<SubmitJobResultsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubmitJobResultsDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.JobId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<JobEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("JobId", "Job"));

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
