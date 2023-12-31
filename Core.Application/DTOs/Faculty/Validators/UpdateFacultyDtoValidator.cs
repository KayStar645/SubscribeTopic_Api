﻿using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FacultyEntity = Core.Domain.Entities.Faculties;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Faculty.Validators
{
    public class UpdateFacultyDtoValidator : AbstractValidator<UpdateFacultyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFacultyDtoValidator(IUnitOfWork unitOfWork, int? currentId)
        {
            _unitOfWork = unitOfWork;

            Include(new FacultyDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                                        .FirstOrDefaultAsync(x => x.InternalCode == internalCode && x.Id != currentId);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190))
                .MustAsync(async (name, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                                        .FirstOrDefaultAsync(x => x.Name == name && x.Id != currentId);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("name"));

            RuleFor(x => x.Dean_TeacherId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<TeacherEntity>().GetByIdInclude(id)
                                            .Where(x => x.Department.FacultyId == currentId)
                                            .FirstOrDefaultAsync();

                    return exists != null || id == null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("dean_TeacherId"));
        }
    }
}
