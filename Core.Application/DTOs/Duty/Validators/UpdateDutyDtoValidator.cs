﻿using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using DutyEntity = Core.Domain.Entities.Duty;

namespace Core.Application.DTOs.Duty.Validators
{
    public class UpdateDutyDtoValidator : AbstractValidator<UpdateDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateDutyDtoValidator(IUnitOfWork unitOfWork, int? pCurrentId)
        {
            _unitOfWork = unitOfWork;

            Include(new DutyDtoValidator(_unitOfWork));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190))
            .MustAsync(async (name, token) =>
            {
                var exists = await _unitOfWork.Repository<DutyEntity>()
                                    .FirstOrDefaultAsync(x => x.Name == name && x.Id != pCurrentId);
                return exists == null;
            }).WithMessage(ValidatorTransform.Exists("name"));
        }
    }
}