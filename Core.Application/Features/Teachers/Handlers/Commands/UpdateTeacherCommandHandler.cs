using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher.Validators;
using Core.Application.Features.Teachers.Requests.Commands;
using MediatR;
using Core.Domain.Entities;
using Core.Application.DTOs.Teacher;
using System.Net;
using Core.Application.Transform;
using Core.Application.Responses;

namespace Core.Application.Features.Teachers.Handlers.Commands
{
    public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Result<TeacherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTeacherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<TeacherDto>> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateTeacherDtoValidator();
            var validationResult = await validator.ValidateAsync(request.UpdateTeacherDto);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<TeacherDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findTeacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.UpdateTeacherDto.Id);

                if (findTeacher is null)
                {
                    return Result<TeacherDto>.Failure(
                        ValidatorTranform.ExistsValue("Id", request.UpdateTeacherDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var requestTeacher = _mapper.Map<Teacher>(request.UpdateTeacherDto);

                var newTeacher = await _unitOfWork.Repository<Teacher>().UpdateAsync(requestTeacher);
                await _unitOfWork.Save(cancellationToken);

                var teacherDto = _mapper.Map<TeacherDto>(newTeacher);

                return Result<TeacherDto>.Success(teacherDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }            
        }
    }
}
