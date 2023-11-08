using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Thesis;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;
using FacultiesEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.Features.Thesiss.Requests.Queries
{
    public class ListThesisRequest : ListBaseRequest<ThesisDto>
    {
        public bool? isGetIssuer { get; set; }

        public bool? isGetThesisInstructions { get; set; }

        public bool? isGetThesisReviews { get; set; }

        public bool? isGetThesisMajors { get; set; }

        public string? type { get; set; }

        public int? facultyId { get; set; }

        public int? departmentId { get; set; }
    }

    public class ListThesisDtoValidator : AbstractValidator<ListThesisRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListThesisDtoValidator(IUnitOfWork unitOfWork, int? departmentId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<ThesisDto>());

            if (departmentId != null)
            {
                RuleFor(x => x.departmentId)
                .MustAsync(async (departmentId, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>()
                        .FirstOrDefaultAsync(x => x.Id == departmentId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("departmentId"));
            }
            else
            {
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
}
