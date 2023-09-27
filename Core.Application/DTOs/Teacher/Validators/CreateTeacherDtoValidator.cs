﻿using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using TeacherEntity = Core.Domain.Entities.Teacher;
using FacultyEntity = Core.Domain.Entities.Faculty;
using DepartmentEntity = Core.Domain.Entities.Department;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTeacherDtoValidator(IUnitOfWork unitOfWork, int? departmentId)
        {
            _unitOfWork = unitOfWork;

            Include(new TeacherDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
               .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
               .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
               .MustAsync(async (internalCode, token) =>
               {
                   var teacher = await _unitOfWork.Repository<TeacherEntity>()
                       .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                   return teacher == null;
               })
               .WithMessage(internalCode => ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.Type)
                .Must(type => string.IsNullOrEmpty(type) || TeacherEntity.GetType().Any(x => x.Equals(type)))
                .MustAsync(async (type, token) =>
                {
                    if(type == TeacherEntity.TYPE_TEACHER_MINISTRY)
                    {
                        var faculty = await _unitOfWork.Repository<DepartmentEntity>()
                                        .FindByCondition(x => x.Id == departmentId)
                                        .Select(x => x.Faculty).FirstOrDefaultAsync();

                        var exists = await _unitOfWork.Repository<TeacherEntity>()
                                .GetAllInclude(x => x.Type == TeacherEntity.TYPE_TEACHER_MINISTRY)
                                .AnyAsync(x => x.Department.FacultyId == faculty.Id);
                        return exists == null;
                    }
                    return true;
                })
                .WithMessage(ValidatorTranform.Must("type", TeacherEntity.GetType()));
        }
    }
}
