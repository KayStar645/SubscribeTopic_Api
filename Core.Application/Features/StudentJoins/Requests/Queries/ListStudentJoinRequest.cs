using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.StudentJoin;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;
using IndustryEntity = Core.Domain.Entities.Industry;
using MajorEntity = Core.Domain.Entities.Major;
using PeriodEntity = Core.Domain.Entities.RegistrationPeriod;

namespace Core.Application.Features.StudentJoins.Requests.Queries
{
    public class ListStudentJoinRequest : ListBaseRequest<StudentJoinDto>
    {
        public bool isGetStudent { get; set; }

        public bool isGetRegistrationPeriod { get; set; }

        public int facultyId { get; set; }

        public int industryId { get; set; }

        public int majorId { get; set; }

        public int periodId { get; set; }
    }

    public class StudentJoinDtoValidator : AbstractValidator<ListStudentJoinRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentJoinDtoValidator(IUnitOfWork unitOfWork, int majorId, int industryId, int periodId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<StudentJoinDto>());

            if (majorId != null)
            {
                RuleFor(x => x.majorId)
                .MustAsync(async (majorId, token) =>
                {
                    var exists = await _unitOfWork.Repository<MajorEntity>()
                        .FirstOrDefaultAsync(x => x.Id == majorId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("majorId"));
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
                .WithMessage(id => ValidatorTranform.MustIn("industryId"));
            }
            else
            {
                RuleFor(x => x.facultyId)
                .MustAsync(async (facultyId, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                        .FirstOrDefaultAsync(x => x.Id == facultyId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("facultyId"));
            }

            if(periodId != null)
            {
                RuleFor(x => x.periodId)
                .MustAsync(async (periodId, token) =>
                {
                    var exists = await _unitOfWork.Repository<PeriodEntity>()
                        .FirstOrDefaultAsync(x => x.Id == periodId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("periodId"));
            }    
        }
    }
}
