using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Job.Validators
{
    public class CreateJobDtoValidator : AbstractValidator<CreateJobDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateJobDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            Include(new JobDtoValidator(_unitOfWork));
        }
    }
}
