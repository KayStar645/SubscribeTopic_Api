using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Major;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FacultyEntity = Core.Domain.Entities.Faculty;
using IndustryEntity = Core.Domain.Entities.Industry;

namespace Core.Application.Features.Majors.Requests.Queries
{
    public class ListMajorRequest : ListBaseRequest<MajorDto>
    {
        public bool isGetIndustry { get; set; }

        public int facultyId { get; set; }

        public int industryId { get; set; }
    }

    public class MajorDtoValidator : AbstractValidator<ListMajorRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MajorDtoValidator(IUnitOfWork unitOfWork, int industryId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<MajorDto>());

            if(industryId != null)
            {
                RuleFor(x => x.industryId)
                .MustAsync(async (industryId, token) =>
                {
                    var exists = await _unitOfWork.Repository<IndustryEntity>()
                        .FirstOrDefaultAsync(x => x.Id == industryId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("industryId"));
            }   
            else
            {
                RuleFor(x => x.facultyId)
                .MustAsync(async (facultyId, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                        .GetAllInclude(x => x.Id == facultyId)
                        .FirstOrDefaultAsync();
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("facultyId"));
            }
        }
    }
}
