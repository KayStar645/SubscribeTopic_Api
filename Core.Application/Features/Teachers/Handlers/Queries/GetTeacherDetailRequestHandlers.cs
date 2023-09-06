using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Teachers.Requests.Queries;
using MediatR;
using AutoMapper;
using Shared;
using Core.Application.Transform;
using Core.Domain.Entities;
using System.Net;

namespace Core.Application.Features.Teachers.Handlers.Queries
{
    public class GetTeacherDetailRequestHandlers : IRequestHandler<GetTeacherDetailRequest, Result<TeacherDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetTeacherDetailRequestHandlers(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<TeacherDto>> Handle(GetTeacherDetailRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.Id);

                if (teacher is null)
                {
                    return Result<TeacherDto>.Failure(
                        ValidatorTranform.ExistsValue("Id", request.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var teacherDto = _mapper.Map<TeacherDto>(teacher);

                return Result<TeacherDto>.Success(teacherDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
