using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.DTOs.Job.Validators
{
    public class CreateJobDtoValidator : AbstractValidator<CreateJobDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateJobDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            Include(new JobDtoValidator(_unitOfWork));


            RuleFor(x => x.ThesisId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<ThesisEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("thesisId", "thesis"));
        }
    }
}
