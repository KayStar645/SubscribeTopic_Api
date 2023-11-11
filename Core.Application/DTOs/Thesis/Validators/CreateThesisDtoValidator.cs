using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;
using TeacherEntity = Core.Domain.Entities.Teacher;
using MajorEntity = Core.Domain.Entities.Major;

namespace Core.Application.DTOs.Thesis.Validators
{
    public class CreateThesisDtoValidator : AbstractValidator<CreateThesisDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateThesisDtoValidator(IUnitOfWork unitOfWork, int? minQuantity)
        {
            _unitOfWork = unitOfWork;

            Include(new ThesisDtoValidator(_unitOfWork, minQuantity));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var major = await _unitOfWork.Repository<ThesisEntity>()
                                      .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return major == null;
                }).WithMessage(ValidatorTransform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190))
                .MustAsync(async (name, token) =>
                {
                    var exists = await _unitOfWork.Repository<ThesisEntity>()
                                      .FirstOrDefaultAsync(x => x.Name == name);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("name"));

            RuleFor(x => x.Summary)
                .NotEmpty().WithMessage(ValidatorTransform.Required("summary"))
                .MaximumLength(6000).WithMessage(ValidatorTransform.MaximumLength("summary", 6000));

            RuleFor(x => x.ThesisInstructionsId)
            .MustAsync(async (instructionsId, token) =>
            {
                if(instructionsId != null)
                {
                    foreach (var instructionId in instructionsId)
                    {
                        var thesis = await _unitOfWork.Repository<TeacherEntity>()
                            .FirstOrDefaultAsync(x => x.Id == instructionId);
                        if (thesis == null)
                        {
                            return false;
                        }
                    }
                }    

                return true;
            })
            .WithMessage(ValidatorTransform.Exists("thesisInstructions"));

            RuleFor(x => x.ThesisMajorsId)
            .MustAsync(async (majorsId, token) =>
            {
                if(majorsId != null)
                {
                    foreach (var majorId in majorsId)
                    {
                        var thesis = await _unitOfWork.Repository<MajorEntity>()
                            .FirstOrDefaultAsync(x => x.Id == majorId);
                        if (thesis == null)
                        {
                            return false;
                        }
                    }
                }  

                return true;
            })
            .WithMessage(ValidatorTransform.Exists("thesisMajors"));
        }
    }
}
