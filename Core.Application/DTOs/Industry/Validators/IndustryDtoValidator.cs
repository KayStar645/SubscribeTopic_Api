using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.DTOs.Industry.Validators
{
    public class IndustryDtoValidator : AbstractValidator<IIndustryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndustryDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.FacultyId)
                .MustAsync(async (id, token) =>
                {
                    var facultyEntityExists = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return facultyEntityExists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("facultyId", "faculties"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190));
        }
    }
}
