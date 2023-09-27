using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.Features.RegistrationPeriods.Requests.Queries
{
    public class ListRegistrationPeriodRequest : ListBaseRequest<RegistrationPeriodDto>
    {
        public bool IsGetFaculty { get; set; }

        public int facultyId { get; set; }
    }

    public class RegistrationPeriodDtoValidator : AbstractValidator<ListRegistrationPeriodRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegistrationPeriodDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<RegistrationPeriodDto>());

            RuleFor(x => x.facultyId)
                .MustAsync(async (facultyId, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                        .FirstOrDefaultAsync(x => x.Id == facultyId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("facultyId"));
        }
    }
}
