using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.Student.Validators;
using Core.Application.Features.Students.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Students.Handlers.Commands
{
    public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentRequest, Result<StudentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<StudentDto>> Handle(UpdateStudentRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateStudentDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.UpdateStudentDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<StudentDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findStudent = await _unitOfWork.Repository<Student>().GetByIdAsync(request.UpdateStudentDto.Id);

                if (findStudent is null)
                {
                    return Result<StudentDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.UpdateStudentDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findStudent.CopyPropertiesFrom(request.UpdateStudentDto);

                var newStudent = await _unitOfWork.Repository<Student>().UpdateAsync(findStudent);
                await _unitOfWork.Save(cancellationToken);

                var studentDto = _mapper.Map<StudentDto>(newStudent);

                return Result<StudentDto>.Success(studentDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<StudentDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
