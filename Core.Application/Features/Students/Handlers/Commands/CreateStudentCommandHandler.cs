using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.Student.Validators;
using Core.Application.Features.Students.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Students.Handlers.Commands
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentRequest, Result<StudentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<StudentDto>> Handle(CreateStudentRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateStudentDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.CreateStudentDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<StudentDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var student = _mapper.Map<Student>(request.CreateStudentDto);

                var newStudent = await _unitOfWork.Repository<Student>().AddAsync(student);
                await _unitOfWork.Save(cancellationToken);

                var studentDto = _mapper.Map<StudentDto>(newStudent);

                return Result<StudentDto>.Success(studentDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<StudentDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}