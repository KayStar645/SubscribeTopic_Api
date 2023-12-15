using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Point;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.Features.Points.Requests.Queries
{
    public class ListPointOfFacultyRequest : ListBaseRequest<PointDto>
    {
        public int? facultyId { get; set; }
    }

    public class ListPointOfFacultyValidator : AbstractValidator<ListPointOfFacultyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListPointOfFacultyValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<PointDto>());

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
