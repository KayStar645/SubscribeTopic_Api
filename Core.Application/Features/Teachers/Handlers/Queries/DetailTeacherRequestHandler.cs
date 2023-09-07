using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Teachers.Requests.Queries;
using MediatR;
using AutoMapper;
using Core.Application.Transform;
using Core.Domain.Entities;
using System.Net;
using Core.Application.Responses;

namespace Core.Application.Features.Teachers.Handlers.Queries
{
    public class DetailTeacherRequestHandler : IRequestHandler<DetailTeacherRequest, Result<TeacherDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailTeacherRequestHandler(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<TeacherDto>> Handle(DetailTeacherRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var findTeacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.Id);

                if (findTeacher is null)
                {
                    return Result<TeacherDto>.Failure(
                        ValidatorTranform.ExistsValue("Id", request.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var teacherDto = _mapper.Map<TeacherDto>(findTeacher);

                return Result<TeacherDto>.Success(teacherDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
