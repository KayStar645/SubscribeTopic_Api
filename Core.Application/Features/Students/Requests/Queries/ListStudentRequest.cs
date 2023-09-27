using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Student;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using MajorEntity = Core.Domain.Entities.Major;
using IndustryEntity = Core.Domain.Entities.Industry;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.Features.Students.Requests.Queries
{
    public class ListStudentRequest : ListBaseRequest<StudentDto>
    {
        public bool isGetMajor { get; set; }

        public int facultyId { get; set; }

        public int industryId { get; set; }

        public int majorId { get; set; }
    }

    public class StudentDtoValidator : AbstractValidator<ListStudentRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentDtoValidator(IUnitOfWork unitOfWork, int majorId, int industryId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<StudentDto>());

            if(majorId != null)
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
            else if(industryId != null)
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
        }
    }
}
