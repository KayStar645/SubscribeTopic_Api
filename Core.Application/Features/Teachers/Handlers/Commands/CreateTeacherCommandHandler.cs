using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Teacher.Validators;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Domain.Entities;
using MediatR;
using Shared;
using System.Net;

namespace Core.Application.Features.Teachers.Handlers.Commands
{
    public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, Result<TeacherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTeacherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<TeacherDto>> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateTeacherDtoValidator();
            var validationResult = await validator.ValidateAsync(request.TeacherDto);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<TeacherDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var teacher = _mapper.Map<Teacher>(request.TeacherDto);

                teacher = await _unitOfWork.Repository<Teacher>().AddAsync(teacher);
                await _unitOfWork.Save(cancellationToken);

                var teacherDto = _mapper.Map<TeacherDto>(teacher);

                return Result<TeacherDto>.Success(teacherDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
