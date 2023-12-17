﻿using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;
using TeacherEntity = Core.Domain.Entities.Teacher;
using MajorEntity = Core.Domain.Entities.Major;
using System.Linq;

namespace Core.Application.DTOs.Thesis.Validators
{
    public class UpdateThesisDtoValidator : AbstractValidator<UpdateThesisDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateThesisDtoValidator(IUnitOfWork unitOfWork, int? minQuantity, int? currentId, List<int?> pInstructionsId, List<int?> pReviewsId)
        {
            _unitOfWork = unitOfWork;

            Include(new ThesisDtoValidator(_unitOfWork, minQuantity));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<ThesisEntity>()
                                      .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190))
                .MustAsync(async (name, token) =>
                {
                    var exists = await _unitOfWork.Repository<ThesisEntity>()
                                      .FirstOrDefaultAsync(x => x.Name == name && x.Id != currentId);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("name"));

            RuleFor(x => x.Summary)
                .NotEmpty().WithMessage(ValidatorTransform.Required("summary"))
                .MaximumLength(6000).WithMessage(ValidatorTransform.MaximumLength("summary", 6000));

            RuleFor(x => x.ThesisInstructionsId)
                .MustAsync(async (instructionsId, token) =>
                {
                    // Kiểm tra không có phần tử trùng nhau trong mảng
                    if (instructionsId != null && instructionsId.Distinct().Count() != instructionsId.Count)
                    {
                        return false;
                    }

                    // Kiểm tra không có phần tử trùng nhau giữa 2 mảng
                    if (pReviewsId != null && instructionsId != null && pReviewsId.Intersect(instructionsId).Any())
                    {
                        return false;
                    }

                    // Kiểm tra điều kiện cũ
                    foreach (var instructionId in instructionsId)
                    {
                        var thesis = await _unitOfWork.Repository<TeacherEntity>()
                            .FirstOrDefaultAsync(x => x.Id == instructionId);
                        if (thesis == null)
                        {
                            return false;
                        }
                    }

                    return true;
                })
                .WithMessage("Giảng viên không được đồng thời hướng dẫn và phản biện một đề tài!");

            RuleFor(x => x.ThesisReviewsId)
                .MustAsync(async (reviewsId, token) =>
                {
                    // Kiểm tra không có phần tử trùng nhau trong mảng
                    if (reviewsId != null && reviewsId.Distinct().Count() != reviewsId.Count)
                    {
                        return false;
                    }

                    // Kiểm tra không có phần tử trùng nhau giữa 2 mảng
                    if (pInstructionsId != null && reviewsId != null && pInstructionsId.Intersect(reviewsId).Any())
                    {
                        return false;
                    }

                    // Kiểm tra điều kiện cũ
                    foreach (var reviewId in reviewsId)
                    {
                        var thesis = await _unitOfWork.Repository<TeacherEntity>()
                            .FirstOrDefaultAsync(x => x.Id == reviewId);
                        if (thesis == null)
                        {
                            return false;
                        }
                    }

                    return true;
                })
                .WithMessage("Giảng viên không được đồng thời hướng dẫn và phản biện một đề tài!");




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
            .WithMessage(ValidatorTransform.NotExistsValueInTable("thesisMajors", "major"));
        }
    }
}
