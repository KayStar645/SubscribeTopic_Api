using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.JobResults;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using JobEntity = Core.Domain.Entities.Job;
using StudentEntity = Core.Domain.Entities.Student;

namespace Core.Application.Features.JobResults.Requests.Queries
{
    public class ListJobResultsRequest : ListBaseRequest<JobResultsDto>
    {
        public bool? isGetForJob {  get; set; }

        public bool? isGetStudentBy {  get; set; }

        public int? jobId { get; set; }

        public int? studentId { get; set; }
    }

    public class ListJobResultsDtoValidator : AbstractValidator<ListJobResultsRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListJobResultsDtoValidator(IUnitOfWork unitOfWork, int? thesisId, int? studentId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<JobResultsDto>());

            RuleFor(x => x.jobId)
                .MustAsync(async (jobId, token) =>
                {
                    if (jobId != null)
                    {
                        var exists = await _unitOfWork.Repository<JobEntity>()
                        .FirstOrDefaultAsync(x => x.Id == jobId);
                        return exists != null;
                    }
                    return false;
                })
                .WithMessage(id => ValidatorTransform.MustIn("jobId"));

            RuleFor(x => x.studentId)
                .MustAsync(async (studentId, token) =>
                {
                    if (studentId != null)
                    {
                        var exists = await _unitOfWork.Repository<StudentEntity>()
                        .FirstOrDefaultAsync(x => x.Id == studentId);
                        return exists != null;
                    }
                    return true;
                })
                .WithMessage(id => ValidatorTransform.MustIn("studentId"));
        }
    }
}
