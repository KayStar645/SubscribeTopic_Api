using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.FacultyDuty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.Features.FacultyDuties.Requests.Queries
{
    public class ListFacultyDutyRequest : ListBaseRequest<FacultyDutyDto>
    {
        public bool isGetFaculty { get; set; }

        public int facultyId { get; set; }
    }

    public class FacultyDutyDtoValidator : AbstractValidator<ListFacultyDutyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacultyDutyDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<FacultyDutyDto>());

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
