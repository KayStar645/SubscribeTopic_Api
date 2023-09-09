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
using Core.Application.Services;

namespace Core.Application.Features.Teachers.Handlers.Commands
{
    public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherRequest, Result<TeacherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTeacherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<TeacherDto>> Handle(UpdateTeacherRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateTeacherDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.UpdateTeacherDto);

            if (validationResult.IsValid == false)
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
                        ValidatorTranform.NotExistsValue("Id", request.UpdateTeacherDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findTeacher.CopyPropertiesFrom(request.UpdateTeacherDto);

                var newTeacher = await _unitOfWork.Repository<Teacher>().UpdateAsync(findTeacher);
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
