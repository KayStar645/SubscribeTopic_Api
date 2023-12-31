﻿using FluentValidation;
using Core.Application.Transform;
using Core.Application.Contracts.Persistence;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class UpdateTeacherDtoValidator : AbstractValidator<UpdateTeacherDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTeacherDtoValidator(IUnitOfWork unitOfWork, int? currentId)
        {
            _unitOfWork = unitOfWork;

            Include(new TeacherDtoValidator(_unitOfWork));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));

            RuleFor(x => x.InternalCode)
               .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
               .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
               .MustAsync(async (internalCode, token) =>
               {
                   var teacher = await _unitOfWork.Repository<TeacherEntity>()
                       .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                   return teacher == null;
               })
               .WithMessage(internalCode => ValidatorTransform.Exists("internalCode"));
        }
    }
}
