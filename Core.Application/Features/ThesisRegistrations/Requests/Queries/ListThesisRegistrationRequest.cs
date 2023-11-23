using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.ThesisRegistration;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using FacultiesEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.Features.ThesisRegistrations.Requests.Queries
{
    public class ListThesisRegistrationRequest : ListBaseRequest<ThesisRegistrationDto>
    {
        public bool? isGetGroup { get; set; }

        public bool? isGetThesis { get; set; }

        public int? facultyId { get; set; }
    }

    public class ListThesisRegistrationDtoValidator : AbstractValidator<ListThesisRegistrationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListThesisRegistrationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<ThesisRegistrationDto>());

            RuleFor(x => x.facultyId)
                .MustAsync(async (facultyId, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultiesEntity>()
                        .FirstOrDefaultAsync(x => x.Id == facultyId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("facultyId"));
        }
    }
}