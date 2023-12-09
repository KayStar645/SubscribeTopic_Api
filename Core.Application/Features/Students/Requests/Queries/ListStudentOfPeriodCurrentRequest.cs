using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Student;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using IndustryEntity = Core.Domain.Entities.Industry;
using MajorEntity = Core.Domain.Entities.Major;

namespace Core.Application.Features.Students.Requests.Queries
{
    public class ListStudentOfPeriodCurrentRequest : ListBaseRequest<FriendDto>
    {
        public bool? isGetMajor { get; set; }

        public int? industryId { get; set; }

        public int? majorId { get; set; }
    }

    public class ListStudentOfPeriodCurrentValidator : AbstractValidator<ListStudentOfPeriodCurrentRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListStudentOfPeriodCurrentValidator(IUnitOfWork unitOfWork, int? majorId, int? industryId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<FriendDto>());

            if (majorId != null)
            {
                RuleFor(x => x.majorId)
                .MustAsync(async (majorId, token) =>
                {
                    var exists = await _unitOfWork.Repository<MajorEntity>()
                        .FirstOrDefaultAsync(x => x.Id == majorId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("majorId"));
            }
            else if (industryId != null)
            {
                RuleFor(x => x.industryId)
                .MustAsync(async (industryId, token) =>
                {
                    var exists = await _unitOfWork.Repository<IndustryEntity>()
                        .FirstOrDefaultAsync(x => x.Id == industryId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("industryId"));
            }
        }
    }
}
