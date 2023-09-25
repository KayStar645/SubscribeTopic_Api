using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FacultyEntity = Core.Domain.Entities.Faculty;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Faculty.Validators
{
    public class UpdateFacultyDtoValidator : AbstractValidator<UpdateFacultyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFacultyDtoValidator(IUnitOfWork unitOfWork, int currentId)
        {
            _unitOfWork = unitOfWork;

            Include(new FacultyDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                               .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190))
                .MustAsync(async (name, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                                        .FirstOrDefaultAsync(x => x.Id != currentId && x.Name == name);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("name"));

            RuleFor(x => x.Dean_TeacherId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<TeacherEntity>().GetByIdInclude(id)
                                            .Where(x => x.Department.FacultyId == id)
                                            .FirstOrDefaultAsync();

                    return exists != null || id == null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("dean_TeacherId"));
        }
    }
}
