using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Job.Validators
{
    public class UpdateJobDtoValidator : AbstractValidator<UpdateJobDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateJobDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            Include(new JobDtoValidator(_unitOfWork));
        }
    }
}
