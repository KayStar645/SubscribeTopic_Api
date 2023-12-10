using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using JobEntity = Core.Domain.Entities.Job;

namespace Core.Application.DTOs.Exchanges.Validators
{
    public class CreateExchangeDtoValidator : AbstractValidator<CreateExchangeDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateExchangeDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ValidatorTransform.Required("content"))
                .MaximumLength(6000).WithMessage(ValidatorTransform.MaximumLength("content", 6000));

            RuleFor(x => x.JobId)
               .MustAsync(async (id, token) =>
               {
                   var exists = await _unitOfWork.Repository<JobEntity>().GetByIdAsync(id);
                   return exists != null;
               })
               .WithMessage(id => ValidatorTransform.NotExistsValueInTable("JobId", "Job"));
        }
    }
}
