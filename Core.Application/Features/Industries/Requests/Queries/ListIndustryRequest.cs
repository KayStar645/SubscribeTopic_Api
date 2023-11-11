using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Industry;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.Features.Industries.Requests.Queries
{
    public class ListIndustryRequest : ListBaseRequest<IndustryDto>
    {
        public bool isGetFaculty { get; set; }

        public int facultyId { get; set; }
    }

    public class IndustryDtoValidator : AbstractValidator<ListIndustryRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndustryDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<IndustryDto>());

            RuleFor(x => x.facultyId)
                .MustAsync(async (facultyId, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                        .FirstOrDefaultAsync(x => x.Id == facultyId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("facultyId"));
        }
    }
}
