﻿using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.DTOs.RegistrationPeriod.Validators
{
    public class RegistrationPeriodDtoValidator : AbstractValidator<IRegistrationPeriodDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RegistrationPeriodDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Semester)
                .Must(semester => CommonTranform.GetListSemester().Any(value => value == semester))
                .WithMessage(ValidatorTransform.Must("semester", CommonTranform.GetListSemester()));

            RuleFor(x => x.FacultyId)
                .MustAsync(async (id, token) =>
                {
                    var existsFaculty = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return existsFaculty != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("facultyId", "faculty"));
        }
    }
}
