using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Duty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using DutyEntity = Core.Domain.Entities.Duty;

namespace Core.Application.Features.Duties.Requests.Queries
{
    public class ListDutyRequest : ListBaseRequest<DutyDto>
    {
        public bool? isGetFaculty { get; set; }

        public bool? isGetDepartment { get; set; }

        public bool? isGetTeacher { get; set; }

        public string? type { get; set; }
    }

    public class ListDutyValidator : AbstractValidator<ListDutyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListDutyValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<DutyDto>());

            RuleFor(x => x.type)
                .Must(type => DutyEntity.GetType().Contains(type))
                .WithMessage(ValidatorTransform.Must("type", DutyEntity.GetType()));
        }
    }
}
