using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;
using MajorEntity = Core.Domain.Entities.Major;


namespace Core.Application.DTOs.Major.Validators
{
    public class MajorDtoValidator : AbstractValidator<IMajorDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MajorDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.FacultyId)
                .MustAsync(async (id, token) =>
                {
                    var facultyEntityExists = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return facultyEntityExists != null;
                })
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("facultyId", "facultys"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var major = await _unitOfWork.Repository<MajorEntity>()
                                      .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return major != null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190));
        }
    }
}
