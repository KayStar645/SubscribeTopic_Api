using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using StudentJoinEntity = Core.Domain.Entities.StudentJoin;

namespace Core.Application.DTOs.Point.Validators
{
    public class CreateOrUpdatePointDtoValidator : AbstractValidator<CreateOrUpdatePointDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrUpdatePointDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Scores)
                .NotEmpty().WithMessage(ValidatorTransform.Required("scores"))
                .GreaterThanOrEqualTo(0).WithMessage(ValidatorTransform.GreaterThanOrEqualTo("scores", 0))
                .LessThanOrEqualTo(10).WithMessage(ValidatorTransform.LessThanOrEqualTo("scores", 10));

            RuleFor(x => x.StudentJoinId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<StudentJoinEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("studentJoinId", "studentJoin"));

        }
    }
}
