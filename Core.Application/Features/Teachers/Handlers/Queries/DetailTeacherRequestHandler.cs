using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Teachers.Requests.Queries;
using MediatR;
using AutoMapper;
using Core.Application.Transform;
using Core.Domain.Entities;
using System.Net;
using Core.Application.Responses;
using Microsoft.EntityFrameworkCore;

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
                var query = _unitOfWork.Repository<Teacher>().GetByIdInclude(request.Id);

                if (request.IsAllDetail)
                {
                    query = _unitOfWork.Repository<Teacher>().AddInclude(query, x => x.Department);
                }
                else
                {
                    if (request.IsGetDepartment == true)
                    {
                        query = _unitOfWork.Repository<Teacher>().AddInclude(query, x => x.Department);
                    }
                }

                var findTeacher = await query.SingleAsync();

                if (findTeacher is null)
                {
                    return Result<TeacherDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.Id.ToString()),
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
